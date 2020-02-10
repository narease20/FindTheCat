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

    public void Drop()
    {
        // Null Check
        if (m_CurrentInteractable)
        {
            // Apply Velocity
            Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
            targetBody.velocity = m_Pose.GetVelocity() + transform.forward;
            targetBody.angularVelocity = m_Pose.GetAngularVelocity() + transform.forward;
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
