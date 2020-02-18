using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_TouchpadAction = null; 

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    private Interactable m_CloseInteractable = null;
    public List<Interactable> m_ContactInteractables = new List<Interactable>();

    public float maxGrabTimer = 2.5f;
    public float grabTimer = 0.0f;

    public float sizeDownAmount = 0.2f;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
        grabTimer = Mathf.Clamp(grabTimer, 0, maxGrabTimer);
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {

            if(m_CurrentInteractable != null && m_TouchpadAction.GetStateDown(m_Pose.inputSource))
            {
                m_CurrentInteractable.Action();
                return;
            }

            grabTimer = maxGrabTimer;
            Pickup();
        }

        if (m_GrabAction.GetStateDown(m_Pose.inputSource) && grabTimer == 0.0f)
        {
            Drop();
        }

        if(grabTimer != 0.0f)
        {
            grabTimer -= Time.deltaTime;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            //m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
            m_CloseInteractable = other.gameObject.GetComponent<Interactable>();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            //m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
            m_CloseInteractable = null;
        }
    }

    // The interactable is attached to the hand
    public void Pickup()
    {

        m_CurrentInteractable = GetNearestInteractable();

        // Already Held Check
        if (m_CurrentInteractable && m_CurrentInteractable.activeHand)
        {
            m_CurrentInteractable.activeHand.Drop();
        }

        // Grabbed Check
        if (m_CurrentInteractable)
        {
            // Position
            m_CurrentInteractable.ApplyOffset(transform);

            if (m_CurrentInteractable.sizeDown)
            {
                m_CurrentInteractable.transform.localScale -= new Vector3(sizeDownAmount, sizeDownAmount, sizeDownAmount);
            }

            // Attach
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            m_Joint.connectedBody = targetBody;

            // Set Active Hand
            m_CurrentInteractable.activeHand = this;

            if(m_CurrentInteractable.GetComponent<Collider>())
            {
                m_CurrentInteractable.GetComponent<Collider>().enabled = false;
            }
        }
        else
        {
            return;
        }

    }

    // The interactable is unattached from the interactable. If the hand is moving then force is applied to the interactable 
    public void Drop()
    {
        // Null Check
        if (m_CurrentInteractable)
        {
            // Apply Velocity
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            
            if (m_CurrentInteractable.sizeDown)
            {
                m_CurrentInteractable.transform.localScale += new Vector3(sizeDownAmount, sizeDownAmount, sizeDownAmount);
            }
            targetBody.velocity = m_Pose.GetVelocity() * m_CurrentInteractable.throwPower + transform.forward;
            targetBody.angularVelocity = m_Pose.GetAngularVelocity() * m_CurrentInteractable.throwPower + transform.forward;
            //targetBody.AddForce(m_Pose.GetVelocity());
            

            // Detach
            m_Joint.connectedBody = null;

            if (!m_CurrentInteractable.GetComponent<Collider>().enabled)
            {
                m_CurrentInteractable.GetComponent<Collider>().enabled = true;
            }

            // Clear Variables
            m_CurrentInteractable.activeHand = null;
            m_CurrentInteractable = null;

        }
        else
        {
            return;
        }

    }

    // Returns the closest interactable to the hand
    private Interactable GetNearestInteractable()
    {
        Interactable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        /*
        foreach(Interactable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }
        */
        if (m_CloseInteractable != null)
        {
            distance = (m_CloseInteractable.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = m_CloseInteractable;
            }

            return nearest;
        }

        return null;
    }
}
