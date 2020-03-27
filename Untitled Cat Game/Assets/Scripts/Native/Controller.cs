using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Inputs")]
    public Input confirmHeightPress = null;
    public Input jumpPress = null;
    public Input movePress = null;
    public Vector2 moveValue;

    [SerializeField, Range(0.01f, 0.95f), Tooltip("Amount of force the player exerts onto the controller's touchpad.")]
    public float sensitivity = 0.05f;

    private CharacterController characterController = null;
    [HideInInspector]
    public Transform head;

    [Header("Movement Speed")]
    public float maxSpeed = 3.25f;
    public float moveAccel = 1.35f;
    public float moveDeccel = 0.9f;

    private float speed = 0.0f;


    [Header("Jump Variables")]
    public float jumpHeight = 20.5f;

    [Header("Height")]
    [SerializeField, Range(0.45f, 0.95f)]
    public float minHeadHeight = 0.85f;
    [SerializeField, Range(1, 2)]
    public float maxHeadHeight = 1.35f;

    [Tooltip("The player's set standard standing height.")]
    public float playerStandardHeight;

    bool heightSet = false;

    [SerializeField, Range(0, 1), Tooltip("The standard height for a crouch.")]
    public float heightReductionAmount = 0.5f;
    [SerializeField, Range(0.1f, 0.95f), Tooltip("The percentage amount under their set height the player has to go to acchive a crouch state.")]
    public float heightPercentageCrouch = 0.9f;
    [SerializeField, Range(0.1f, 1.35f), Tooltip("The distance checked for an object above the player when they are crouched.")]
    public float heightMeasureDistance = 0.65f;

    [Header("Gameplay Data")]
    [Tooltip("Grab Style changes how throwing objects works. True makes it so the player has to press the button again to throw, while false makes the player have to hold down the button to hold the object.")]
    public bool grabStyle = true;

    [Header("Camera Variables")]
    public Camera cam;
    public float maxX = 360f;
    public float maxY = 45f;
    public float camSensitivity = 50f;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Replace with camera
        head = cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //HandleHeight();
        CheckCrouch();
        CalculateMovement();
    }

    void RecieveInput()
    {
        moveValue.x = Input.GetAxisRaw("Horizontal");
        moveValue.y = Input.GetAxisRaw("Vertical");
        //something something smoothly decrease if not pressing
        if (moveValue.magnitude < 0.05f)
        {
            moveValue = new Vector2(0,0);
        }
        if (moveValue.magnitude < 0.01f)
        {
            moveValue = new Vector2(0, 0);
        }
    }

    void HandleHeightEX()
    {
        head = characterController.transform;
    }

    // Function that changes player's representation in the game based on their head's local height
    // CHECK IN CASE SINCE THERES NO HEAD
    private void HandleHeight()
    {
        // Get heads local space
        float headHeight = Mathf.Clamp(head.localPosition.y, minHeadHeight, maxHeadHeight);

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = characterController.height / 2;
        newCenter.y += characterController.skinWidth;

        // Move capsule in local space
        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;

        // Apply
        characterController.center = newCenter;
    }

    // Calculates player movement
    private void CalculateMovement()
    {
        RecieveInput();
        // Figure out Movement Orientation
        Quaternion orintation = CalculateOrientation();

        Vector3 movement = Vector3.zero;

        // Add and clamp then multiply with the orientation
        if (moveValue.magnitude == 0)
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
        if (moveValue.magnitude > 0)
        {
            speed += moveValue.magnitude * sensitivity * moveAccel;
        }

        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        movement += orintation * (speed * Vector3.forward);

        // Gravity - Said to fall too fast
        movement.y += Physics.gravity.y - Time.deltaTime; //gravity * Time.fixedDeltaTime;

        // Apply
        characterController.Move(movement * Time.deltaTime);

    }

    // Figures out the player's orientation from the play space's sensors
    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(moveValue.x, moveValue.y);
        rotation *= Mathf.Rad2Deg;

        // Orintation
        Vector3 orientationEuler = new Vector3(0, head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }

    // HEY CHECK IF THIS CAUSES CAMERA TO CLIP THROUGH WALLS AND CELLINGS
    //Checks to see if the height is in a certain range, if it is then make the character controller's height small. If not then set the height to whatever it should be
    void CheckCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            characterController.height = heightReductionAmount;
        }
        if (Input.GetButtonUp("Crouch") && !CheckAbovePlayer())
        {
            characterController.height = maxHeadHeight;
        }
        else
        {
            if (!CheckAbovePlayer())
            {
                characterController.height = maxHeadHeight;
            }
        }
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

    public void CameraRotation()
    {
        float rotationX = Input.GetAxis("Mouse X") * camSensitivity * Time.deltaTime;
        float rotationY = Input.GetAxis("Mouse Y") * camSensitivity * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -maxX, maxX);
        rotationY = Mathf.Clamp(rotationY, -maxY, maxY);
        cam.transform.localEulerAngles += new Vector3(-rotationY, rotationX, 0);
    }

    public void SwitchGrabStyles()
    {
        if (grabStyle)
        {
            grabStyle = false;
        }
        if (!grabStyle)
        {
            grabStyle = true;
        }
    }

}
