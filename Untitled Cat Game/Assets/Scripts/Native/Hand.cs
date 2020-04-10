using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    [Header("Inputs")]

    public SteamVR_Action_Boolean grabAction = null;
    public SteamVR_Action_Boolean touchpadAction = null;
    public SteamVR_Action_Boolean switchGrabStyle = null;

    private SteamVR_Behaviour_Pose pose = null;
    private FixedJoint joint = null;

    private Interactable currentInteractable = null;
    private Interactable closeInteractable = null;
    //public List<Interactable> contactInteractables = new List<Interactable>();

    private BoxCollider handCollider;

    public VRC player;

    public bool grabMode = true;

    private Collider grabbedCollider;
    private Rigidbody targetBody;

    [Header("Timers")]

    [SerializeField, Range(0, 3), Tooltip("The amount of time between a grab with a hand.")]
    public float maxGrabTimer = 2.5f;
    public float grabTimer = 0.0f;
    [SerializeField, Range(0, 0.5f)]
    public float colliderReappearTime = 0.35f;
    [SerializeField, Range(0.01f, 0.15f)]
    public float droptimer = 0.15f;
    public bool canDrop = false;

    private void Awake()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        joint = GetComponent<FixedJoint>();
        //Mathf.Clamp(grabTimer, 0, maxGrabTimer);
        handCollider = GetComponent<BoxCollider>();
        //player = GetComponent<VRC>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Do object action
        if (currentInteractable != null && touchpadAction.GetStateDown(pose.inputSource))
        {
            currentInteractable.Action();
            return;
        }

        if (switchGrabStyle.GetStateDown(SteamVR_Input_Sources.Any))
        {
            SwitchStyles();
        }

        // Grab object if in range
        if (grabAction.GetStateDown(pose.inputSource))
        {
            Pickup();
            canDrop = false;
            StopAllCoroutines();
            StartCoroutine(DropTimer());
        }

        // Drop object
        if (grabAction.GetStateDown(pose.inputSource) && player.grabStyle && currentInteractable && canDrop)
        {
            Drop();
            StopAllCoroutines();
            StartCoroutine(HandColliderRestorer());
        }

        if (grabAction.GetStateUp(pose.inputSource) && !player.grabStyle && currentInteractable)
        {
            Drop();
            StopAllCoroutines();
            StartCoroutine(HandColliderRestorer());
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>())//.CompareTag("Interactable"))
        {
            //contactInteractables.Add(other.gameObject.GetComponent<Interactable>());
            closeInteractable = other.gameObject.GetComponent<Interactable>();

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Interactable>())//.CompareTag("Interactable"))
        {
            //contactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
            closeInteractable = null;
        }
    }

    // The interactable is attached to the hand
    public void Pickup()
    {

        currentInteractable = GetNearestInteractable();

        // Already Held Check
        if (currentInteractable && currentInteractable.activeHand)
        {
            currentInteractable.activeHand.Drop();
            return;
        }

        // Grabbed Check
        if (currentInteractable)
        {
            // Position
            currentInteractable.ApplyOffset(transform);


            if (currentInteractable.sizeDown)
            {
                currentInteractable.transform.localScale -= currentInteractable.sizeDownAmount;
            }

            AssignColRB();

            TargetBodyInitialization();

            // Attach
            joint.connectedBody = targetBody;

            // Set Active Hand
            currentInteractable.activeHand = this;

            if(grabbedCollider)
            {
                grabbedCollider.enabled = false;
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
        if (currentInteractable)
        {
            handCollider.enabled = false;

            if (currentInteractable.sizeDown)
            {
                currentInteractable.transform.localScale += currentInteractable.sizeDownAmount;
            }

            // Check to see if the object is throwable, else just drop it. Maybe make a public function to provide the player some velocity in the VRC script if the object is a rock wall
            if (currentInteractable.throwable)
            {
                // Apply Velocity
                targetBody.velocity = pose.GetVelocity() * currentInteractable.throwPower + transform.forward;
                targetBody.angularVelocity = pose.GetAngularVelocity() * currentInteractable.throwPower + transform.forward;
                //targetBody.AddForce(pose.GetVelocity());
            }

            targetBody.useGravity = true;

            // Detach
            joint.connectedBody = null;

            if (grabbedCollider && !grabbedCollider.enabled)
            {
                grabbedCollider.enabled = true;
            }

            // Clear Variables
            currentInteractable.activeHand = null;
            currentInteractable = null;

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

        if (closeInteractable != null)
        {
            distance = (closeInteractable.transform.position - transform.position).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = closeInteractable;
            }

            return nearest;
        }

        return null;
    }

    // Waits after the object is thrown the puts the player's hand back
    IEnumerator HandColliderRestorer()
    {
        yield return new WaitForSeconds(colliderReappearTime);
        handCollider.enabled = true;
        Debug.Log("Hand collider should be " + handCollider.enabled);
    }

    // Helps the button press to throw fucntion correctly by forcing the game to wait before automatically letting go of the object
    IEnumerator DropTimer()
    {
        yield return new WaitForSeconds(droptimer);
        canDrop = true;
        Debug.Log("CanDrop is " + canDrop);
    }

    // Switchs grab Styles
    public void SwitchStyles()
    {
        player.SwitchGrabStyles();
    }
    void AssignColRB()
    {
        grabbedCollider = currentInteractable.GetComponent<Collider>();
        targetBody = currentInteractable.GetComponent<Rigidbody>();
    }
    void TargetBodyInitialization()
    {
        targetBody.useGravity = false;
        targetBody.velocity = new Vector3(0,0,0);
        targetBody.angularVelocity = new Vector3(0, 0, 0);
    }
}
