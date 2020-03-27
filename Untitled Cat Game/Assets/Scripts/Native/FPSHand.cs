using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent((typeof(CapsuleCollider)))]
public class FPSHand : MonoBehaviour
{
    [Header("Inputs")]

    public string grabAction = null;
    public string touchpadAction = null;
    public string switchGrabStyle = null;
    
    private Interactable currentInteractable = null;
    private Interactable closeInteractable = null;

    public PlayerMovement player;
    public Rigidbody playerBody;

    public bool grabMode = true;

    public float throwVelocity;

    public Transform holdLocation;

    private Collider grabbedCollider;
    private Rigidbody targetBody;

    private bool setParent;

    [Header("Timers")]

    [SerializeField, Range(0, 3), Tooltip("The amount of time between a grab with a hand.")]
    public float maxGrabTimer = 2.5f;
    public float grabTimer = 0.0f;
    [SerializeField, Range(0, 0.5f)]
    public float colliderReappearTime = 0.35f;
    [SerializeField, Range(0.01f, 0.95f)]
    public float droptimer = 0.35f;
    public bool canDrop = false;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        playerBody = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Do object action
        if (currentInteractable != null && Input.GetButton(touchpadAction))
        {
            currentInteractable.Action();
        }

        // Grab object if in range
        if (Input.GetButtonDown(grabAction) && currentInteractable == null)
        {
            Pickup();
            canDrop = false;
            StopAllCoroutines();
            StartCoroutine(DropTimer());
        }

        // Drop object
        if (Input.GetButtonDown(grabAction) && currentInteractable && canDrop)
        {
            Drop();
            StopAllCoroutines();
            StartCoroutine(DropTimer());
        }

        if(currentInteractable && !setParent)
        {
            currentInteractable.transform.SetParent(holdLocation);
            setParent = true;
        }

        /*
        if (grabAction.GetStateUp(pose.inputSource) && !player.grabStyle && currentInteractable)
        {
            Drop();
            StopAllCoroutines();
            StartCoroutine(HandColliderRestorer());
        }
        */

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
        if (currentInteractable && currentInteractable.activeFPSHand)
        {
            currentInteractable.activeFPSHand.Drop();
            return;
        }

        // Grabbed Check
        if (currentInteractable)
        {
            // Position
            currentInteractable.ApplyOffset(holdLocation);

            if (currentInteractable.sizeDown)
            {
                currentInteractable.transform.localScale -= currentInteractable.sizeDownAmount;
            }

            // Attach
            AssignColRB();

            TargetBodyInitialization();

            //joint.connectedBody = targetBody;

            // Set Active Hand
            currentInteractable.activeFPSHand = this;


            if (grabbedCollider && grabbedCollider.enabled)
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
            if (currentInteractable.sizeDown)
            {
                currentInteractable.transform.localScale += currentInteractable.sizeDownAmount;
            }

            // Check to see if the object is throwable, else just drop it. Maybe make a public function to provide the player some velocity in the VRC script if the object is a rock wall
            if (currentInteractable.throwable && targetBody)
            {
                // Apply Velocity
                if(playerBody.velocity.magnitude > throwVelocity)
                {
                    targetBody.velocity = playerBody.velocity * currentInteractable.throwPower + transform.forward * throwVelocity;
                    targetBody.angularVelocity = playerBody.angularVelocity * currentInteractable.throwPower + transform.forward * throwVelocity;
                }

                //targetBody.AddForce(pose.GetVelocity());
            }

            // Detach

            if (grabbedCollider && !grabbedCollider.enabled)
            {
                grabbedCollider.enabled = true;
            }

            targetBody.useGravity = true;

            // Clear Variables
            currentInteractable.transform.parent = null;
            setParent = false;
            currentInteractable.activeFPSHand = null;
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

    // Helps the button press to throw fucntion correctly by forcing the game to wait before automatically letting go of the object
    IEnumerator DropTimer()
    {
        yield return new WaitForSeconds(droptimer);
        canDrop = true;
        Debug.Log("CanDrop is " + canDrop);
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
