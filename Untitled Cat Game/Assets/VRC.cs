using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRC : MonoBehaviour
{
    public float gravity = 30.0f;
    public float sensitivity = 0.1f;
    public float maxSpeed = 4.25f;
    public float moveAccel = 1.35f;
    public float moveDeccel = 0.9f;

    public SteamVR_Action_Boolean confirmHeightPress = null;
    public SteamVR_Action_Boolean movePress = null;
    public SteamVR_Action_Vector2 moveValue = null;

    private float speed = 0.0f;

    private CharacterController characterController = null;
    private Transform head;

    public float minHeadHeight = 0.85f;
    public float maxHeadHeight = 2f;
    public float playerStandardHeight;
    //public bool heightSet;

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
        CheckCrouch();
        Jump();
    }

    // Function that changes player's representation in the game based on their head's local height
    private void HandleHeight()
    {
        // Get heads local space
        float headHeight = Mathf.Clamp(head.localPosition.y, minHeadHeight, maxHeadHeight);
        characterController.height = headHeight;

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
        if (moveValue.axis.magnitude == 0)
        {
            if(speed > 0.1f)
            {
                speed -= moveDeccel;
            }
            else
            {
                speed = 0;
            }
        }

        // Add and clamp then multiply with the orientation
        speed += moveValue.axis.magnitude * sensitivity * moveAccel;
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        movement += orintation * (speed * Vector3.forward);

        // Gravity
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
        if (confirmHeightPress.GetStateUp(SteamVR_Input_Sources.Any) && characterController.height > minHeadHeight)
        {
            playerStandardHeight = characterController.height;
        }
       
    }

    //Checks to see if the height is in a certain range, if it is then make the character controller's height small. If not then set the height to whatever it should be
    void CheckCrouch()
    {
        
    }

    void Jump()
    {

    }

}
