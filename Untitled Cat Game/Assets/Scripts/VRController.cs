using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public float gravity = 30.0f;
    public float maxGravity = 150f;

    [SerializeField, Range(0f, 10f)]
    public float jumpHeight = 2.5f;
    public float jumpCounter = 0.0f;
    public float maxJumpCounter = 5.5f;


    public float sensitivity = 0.1f;

    public float maxSpeed = 2.45f;
    public float maxAcceleration = 0.1f, maxAirAcceleration = 1f;

    public float rotateIncrement = 90;
    public SteamVR_Action_Boolean rotatePress = null;

    public SteamVR_Action_Boolean movePress = null;
    public SteamVR_Action_Vector2 moveValue = null;

    public SteamVR_Action_Boolean jumpPress = null;

    private float speed = 0.0f;
    private Rigidbody rb;
    private float moveAccel = 0.35f;
    private float moveDecel = 0.3f;

    private CharacterController characterController = null;
    private Transform head = null;

    public float maxHeadHeight = 2.5f;
    public float minHeadHeight = 0.3f;
    Vector3 velocity, desiredVelocity;
    bool desiredJump;
    bool onGround;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    float minGroundDotProduct;
    Vector3 contactNormal;
    //SphereCollider groundCollision;
    //BoxCollider headCollision;
    CapsuleCollider player;
    public GameObject PlayArea;
    public GameObject Camera;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        //groundCollision = GetComponent<SphereCollider>();
        //headCollision = GetComponent<BoxCollider>();
        player = GetComponent<CapsuleCollider>();
        OnValidate();
    }

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    // Start is called before the first frame update
    private void Start()
    {
        head = SteamVR_Render.Top().head;
        rb = GetComponent<Rigidbody>();
        jumpCounter = Mathf.Clamp(jumpCounter, 0, maxJumpCounter);
        gravity = Mathf.Clamp(gravity, 0, maxGravity);

        velocity.y = Mathf.Clamp(velocity.y, 0, maxGravity);
        velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        velocity.z = Mathf.Clamp(velocity.z, -maxSpeed, maxSpeed);
    }

    // Update is called once per frame
    private void Update()
    {
        HandleHeight();
        CalculateMovement();
        CheckGround();
        //SnapRotation();
        desiredJump = jumpPress.GetStateDown(SteamVR_Input_Sources.Any);
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        if (jumpPress.GetStateDown(SteamVR_Input_Sources.Any) || desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        //onGround = false;
    }

    // SOMETHING WRONG HERE THAT MAKES PLAYER FLY
    // Function that changes player's representation in the game based on their head's local height
    private void HandleHeight()
    {
        // Get heads local space
        float headHeight = Mathf.Clamp(head.localPosition.y, minHeadHeight, maxHeadHeight);
        //m_CharacterController.height = headHeight;
        //headCollision.center = new Vector3 (0, headHeight, 0);
        player.height = headHeight;

        // cut in half
        Vector3 newCenter = Vector3.zero;
        //newCenter.y = m_CharacterController.height / 2;
        //newCenter.y += m_CharacterController.skinWidth;

        //newCenter.y = headCollision.center.y / 2;
        //newCenter.y += headCollision.size.y;

        newCenter.y = player.height / 2;
        newCenter.y += player.center.y;

        // Move capsule in local space
        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;

        /*
        // rotate
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;
        */


        // apply
        //m_CharacterController.center = newCenter;
        //m_CharacterController.height = headHeight / 1.3f;

        //groundCollision.center =  new Vector3(newCenter.x, newCenter.y - 0.3f, newCenter.z);
        //headCollision.center = new Vector3(newCenter.x, newCenter.y, newCenter.z);

        //PlayArea.transform.position = new Vector3(newCenter.x, newCenter.y - 0.3f, newCenter.z);

        player.center = new Vector3(newCenter.x, 0.3f, newCenter.z);
        player.height = headHeight;

        //Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y - 0.2f, Camera.transform.position.z);

        PlayArea.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
    }

    // Calculates player movement
    private void CalculateMovement()
    {
        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        // Check if not moving
        if (moveValue.axis.magnitude == 0)
        {
            speed = 0;
            rb.velocity /= moveDecel;
        }

        if(moveValue.axis.magnitude > 0.2f)
        {
            //Add, clamp
            speed += moveValue.axis.magnitude * moveAccel;
            speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

            //orintation
            movement += orientation * (speed * Vector3.forward);

            //movement.y -= gravity + Time.deltaTime;
            // Apply Speed
            rb.velocity += (movement * Time.deltaTime);
        }
        /*
        //Add, clamp
        speed += moveValue.axis.magnitude * moveAccel;
        speed = Mathf.Clamp(speed, -maxSpeed, maxSpeed);

        //orintation
        movement += orientation * (speed * Vector3.forward);

        //movement.y -= gravity + Time.deltaTime;
        // Apply Speed
        rb.velocity += (movement * Time.deltaTime);
        //m_CharacterController.Move(movement * Time.deltaTime);
        //desiredVelocity = new Vector3(movement.x, 0, movement.z); //* maxSpeed;
        */
        //transform.localPosition = (new Vector3(movement.x, 0, movement.z) * speed)* Time.deltaTime;

    }

    // Calculates the velocity of the player and applies gravity if they are not on the ground
    private void CalculateVelocity()
    {
        velocity = rb.velocity;
        //velocity = m_CharacterController.velocity;
        AdjustVelocity();
        /*
        if (!onGround)
        {
            ConstantGravity();
        }
        */
        ConstantGravity();
        if (moveValue.axis.magnitude < 0.2f)
        {
            velocity = new Vector3(velocity.x / moveDecel, velocity.y, velocity.z / moveDecel);
            if (velocity.magnitude < 0.5f)
            {
                velocity = new Vector3(0, velocity.y, 0);
            }
        }
        rb.velocity = velocity;

    }

    private void Jump()
    {
        if (onGround)
        {
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if(alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }
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

    // Allows the player to press a button to quickly turn a set amount, however this breaks object throwing
    private void SnapRotation()
    {
        float snapValue = 0.0f;

        if (rotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            snapValue = -Mathf.Abs(rotateIncrement);
        }

        if (rotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = Mathf.Abs(rotateIncrement);
        }

        transform.RotateAround(head.position, Vector3.up, snapValue);
        //transform.RotateAround(m_connected.transform.position, Vector3.up, snapValue);

    }
    
    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    // Checks the collisions the player collides with (on collision enter and stay) and extracts the normal
    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                onGround = true;
                contactNormal = normal;
            }
        }
    }

    void EvaluateCollision(Vector3 normal)
    {
        if (normal.y >= minGroundDotProduct)
        {
            onGround = true;
            contactNormal = normal;
        }
    }

    // Uses a vector to figure out how the contacted normal moves the player
    private Vector3 ProjectOnContactPlane (Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    private void CheckGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(head.transform.position, transform.TransformDirection(Vector3.down), out hit, maxHeadHeight))
        {
            Debug.DrawRay(head.transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.black);
            EvaluateCollision(hit.normal);
        }
        else
        {
            onGround = false;
        }
    }


    // Takes the player's current velocity and input to move them in the direction specified
    void AdjustVelocity()
    {
        
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        //velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);

        if (moveValue.axis.magnitude > 0)
        {
            velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        /*
        if (moveValue.axis.magnitude == 0 && velocity != Vector3.zero)
        {
            velocity += (xAxis * (newX - currentX) + zAxis * (newZ - currentZ)) / moveDecel;
        }
        */
    }

    // Applies a gravity force on the player over time
    void ConstantGravity()
    {
        velocity.y += Physics.gravity.y * Time.deltaTime;
    }

}
