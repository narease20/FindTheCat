using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRC : MonoBehaviour
{
    //public float gravity = 30.0f;

    [SerializeField, Range(0.01f, 0.95f), Tooltip("Amount of force the player exerts onto the controller's touchpad.")]
    public float sensitivity = 0.05f;

    [Header("Movement Speed")]

    public float maxSpeed = 3.25f;
    public float moveAccel = 1.35f;
    public float moveDeccel = 0.9f;

    private float speed = 0.0f;


    [Header("Jump Variables")]

    public float jumpHeight = 10f;

    [Header("Inputs")]

    public SteamVR_Action_Boolean confirmHeightPress = null;
    public SteamVR_Action_Boolean jumpPress = null;
    public SteamVR_Action_Boolean movePress = null;
    public SteamVR_Action_Vector2 moveValue = null;

    private CharacterController characterController = null;
    private Transform head;


    [Header("Height")]

    [SerializeField, Range(0.45f, 0.95f)]
    public float minHeadHeight = 0.85f;
    [SerializeField, Range(1, 2)]
    public float maxHeadHeight = 2f;

    [Tooltip("The player's set standard standing height.")]
    public float playerStandardHeight;

    bool heightSet = false;

    [SerializeField, Range(0, 1), Tooltip("The standard height for a crouch.")]
    public float heightReductionAmount = 0.3f;
    [SerializeField, Range(0.1f, 0.95f), Tooltip("The percentage amount under their set height the player has to go to acchive a crouch state.")]
    public float heightPercentageCrouch = 0.9f;
    [SerializeField, Range(0.1f, 0.95f), Tooltip("The distance checked for an object above the player when they are crouched.")]
    public float heightMeasureDistance = 0.5f;

    [Header("Gameplay Data")]
    [Tooltip("Grab Style changes how throwing objects works. True makes it so the player has to press the button again to throw, while false makes the player have to hold down the button to hold the object.")]
    public bool grabStyle = true;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHeight();
        CalculateMovement();
        ConfirmHeight();
        //CheckCrouch();
        //Jump();
    }

    // Function that changes player's representation in the game based on their head's local height
    private void HandleHeight()
    {
        // Get heads local space
        float headHeight = Mathf.Clamp(head.localPosition.y, minHeadHeight, maxHeadHeight);
        CheckCrouch(headHeight);
        //characterController.height = headHeight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        // Move capsule in local space
        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;

        // Apply
        characterController.center = newCenter;
        //characterController.height = headHeight / 1.3f;

    }

    // Calculates player movement
    private void CalculateMovement()
    {
        // Figure out Movement Orientation
        Quaternion orintation = CalculateOrientation();

        Vector3 movement = Vector3.zero;
        /*
        if (movePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            speed = 0;
        }
        */

        // Add and clamp then multiply with the orientation
        if (moveValue.axis.magnitude == 0)
        {
            if (speed > 0.1f)
            {
                speed -= moveDeccel;
            }
            else
            {
                speed = 0;
            }
        }
        if (moveValue.axis.magnitude > 0)
        {
            speed += moveValue.axis.magnitude * sensitivity * moveAccel;
        }

        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        movement += orintation * (speed * Vector3.forward);

        Jump(movement);

        // Gravity - Said to fall too fast
        movement.y += Physics.gravity.y - Time.deltaTime; //gravity * Time.fixedDeltaTime;

        // Apply
        characterController.Move(movement * Time.deltaTime);

    }

    // Figures out the player's orientation from the play space's sensors
    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(moveValue.axis.x, moveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        // Orintation
        Vector3 orientationEuler = new Vector3(0, head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }

    // When the player presses a button, their normal height is recorded
    void ConfirmHeight()
    {
        if (confirmHeightPress.GetStateDown(SteamVR_Input_Sources.Any) && characterController.height > minHeadHeight && characterController.height < maxHeadHeight)
        {
            playerStandardHeight = characterController.height;
            heightSet = true;
            Debug.Log("Height Set!");
        }

    }

    // HEY CHECK IF THIS CAUSES CAMERA TO CLIP THROUGH WALLS AND CELLINGS
    //Checks to see if the height is in a certain range, if it is then make the character controller's height small. If not then set the height to whatever it should be
    void CheckCrouch(float headHeight)
    {
        if (heightSet)
        {
            if (headHeight < playerStandardHeight * heightPercentageCrouch)
            {
                characterController.height = heightReductionAmount;
                // Puts out a line above the Character controller's position, however its a bit sunken into it. Offset or use height instead?
                Debug.DrawRay(new Vector3(transform.position.x + characterController.center.x, characterController.height - transform.position.y + characterController.center.y, transform.position.z + characterController.center.z), transform.TransformDirection(Vector3.up) * heightMeasureDistance, Color.green);
            }
            else
            {
                characterController.height = headHeight;
            }
        }
        else
        {
            characterController.height = headHeight;
        }
    }

    // 
    Vector3 Jump(Vector3 movement)
    {
        if (jumpPress.GetStateDown(SteamVR_Input_Sources.Any))
        {
            movement = new Vector3(movement.x, jumpHeight, movement.z);
            return movement;

        }
        return movement;
    }

    bool CheckAbovePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x + characterController.center.x, characterController.height - transform.position.y + characterController.center.y, transform.position.z + characterController.center.z), transform.TransformDirection(Vector3.up), out hit, heightMeasureDistance))//Physics.Raycast(characterController.transform.position, transform.TransformDirection(Vector3.up), out hit, heightMeasureDistance))
        {
            //Debug.DrawRay(characterController.transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.black);
            Debug.DrawRay(new Vector3(transform.position.x + characterController.center.x, characterController.height - transform.position.y + characterController.center.y, transform.position.z + characterController.center.z), transform.TransformDirection(Vector3.up) * heightMeasureDistance, Color.green);
            return true;
        }

        return false;
    }
}
