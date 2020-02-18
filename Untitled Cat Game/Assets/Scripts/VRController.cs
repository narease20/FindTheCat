using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public float m_Gravity = 30.0f;
    public float m_MaxGravity = 150f;

    [SerializeField, Range(0f, 10f)]
    public float jumpHeight = 2.5f;
    public float jumpCounter = 0.0f;
    public float maxJumpCounter = 5.5f;


    public float m_Sensitivity = 0.1f;

    public float m_MaxSpeed = 2.45f;
    public float maxAcceleration = 0.1f, maxAirAcceleration = 1f;

    public float m_RotateIncrement = 90;
    public SteamVR_Action_Boolean m_RotatePress = null;

    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    public SteamVR_Action_Boolean m_JumpPress = null;

    private float m_Speed = 0.0f;
    public Rigidbody m_rb;
    private float moveAccel = 0.35f;
    private float moveDecel = 1.3f;

    private CharacterController m_CharacterController = null;
    private Transform m_Head = null;

    public float maxHeadHeight = 2.5f;
    public float minHeadHeight = 0.3f;
    Vector3 velocity, desiredVelocity;
    bool desiredJump;
    bool onGround;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    float minGroundDotProduct;
    Vector3 contactNormal;
    SphereCollider m_GroundCollision;
    BoxCollider m_HeadCollision;
    public GameObject vision;

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_rb = GetComponent<Rigidbody>();
        m_GroundCollision = GetComponent<SphereCollider>();
        m_HeadCollision = GetComponent<BoxCollider>();
        OnValidate();
    }

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_Head = SteamVR_Render.Top().head;
        m_rb = GetComponent<Rigidbody>();
        jumpCounter = Mathf.Clamp(jumpCounter, 0, maxJumpCounter);
        m_Gravity = Mathf.Clamp(m_Gravity, 0, m_MaxGravity);

        velocity.y = Mathf.Clamp(velocity.y, 0, m_MaxGravity);
        velocity.x = Mathf.Clamp(velocity.x, -m_MaxSpeed, m_MaxSpeed);
        velocity.z = Mathf.Clamp(velocity.z, -m_MaxSpeed, m_MaxSpeed);
    }

    // Update is called once per frame
    private void Update()
    {
        HandleHeight();
        CalculateMovement();
        SnapRotation();
        desiredJump = m_JumpPress.GetStateDown(SteamVR_Input_Sources.Any);
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        if (m_JumpPress.GetStateDown(SteamVR_Input_Sources.Any) || desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        onGround = false;
    }

    // Function that changes player's representation in the game based on their head's local height
    private void HandleHeight()
    {
        // Get heads local space
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, minHeadHeight, maxHeadHeight);
        float reduceHead;
        //m_CharacterController.height = headHeight;
        m_HeadCollision.center = new Vector3 (0, headHeight, 0);

        // cut in half
        Vector3 newCenter = Vector3.zero;
        //newCenter.y = m_CharacterController.height / 2;
        //newCenter.y += m_CharacterController.skinWidth;

        newCenter.y = m_HeadCollision.center.y / 2;
        newCenter.y += m_HeadCollision.size.y;

        // Move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        /*
        // rotate
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;
        */


        // apply
        //m_CharacterController.center = newCenter;
        //m_CharacterController.height = headHeight / 1.3f;
        m_GroundCollision.center =  new Vector3(newCenter.x, newCenter.y - 0.3f, newCenter.z);
        m_HeadCollision.center = new Vector3(newCenter.x, newCenter.y + headHeight, newCenter.z);
        vision.transform.position = new Vector3(newCenter.x, newCenter.y + headHeight, newCenter.z);
    }

    // Calculates player movement and handles the jump code
    private void CalculateMovement()
    {
        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        // Check if not moving
        if (m_MoveValue.axis.magnitude == 0)
        {
            m_Speed = 0;
            //m_rb.velocity /= moveDecel;
        }

        //Add, clamp
        m_Speed += m_MoveValue.axis.magnitude * m_Sensitivity * moveAccel;
        m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

        //orintation
        movement += orientation * (m_Speed * Vector3.forward);

        // Jump
        if (m_JumpPress.GetState(SteamVR_Input_Sources.Any) && m_CharacterController.isGrounded)// && jumpCounter < maxJumpCounter)
        {
            //m_rb.velocity += new Vector3(0, jumpSpeed, 0);
            //m_CharacterController.Move(new Vector3(0, jumpSpeed, 0));
        }

        /*

        // Gravity
        if(!m_CharacterController.isGrounded && jumpCounter < maxJumpCounter)
        {
            jumpCounter += Time.deltaTime;
        }

        if (!m_CharacterController.isGrounded && jumpCounter == maxJumpCounter)
        {
            movement.y -= m_Gravity + Time.deltaTime;
            jumpCounter = 0;
        }
        */

        //movement.y -= m_Gravity + Time.deltaTime;
        // Apply Speed
        //m_rb.velocity += (movement * Time.deltaTime);
        //m_CharacterController.Move(movement * Time.deltaTime);
        desiredVelocity = new Vector3(movement.x, 0, movement.z) * m_MaxSpeed;

        //transform.localPosition = (new Vector3(movement.x, 0, movement.z) * m_Speed)* Time.deltaTime;

    }

    // Calculates the velocity of the player and applies gravity if they are not on the ground
    private void CalculateVelocity()
    {
        velocity = m_rb.velocity;
        //velocity = m_CharacterController.velocity;
        AdjustVelocity();
        if (!onGround)
        {
            ConstantGravity();
        }
        if (m_MoveValue.axis.magnitude == 0)
        {
            velocity = new Vector3(velocity.x / moveDecel, velocity.y, velocity.z / moveDecel);
        }
        m_rb.velocity = velocity;

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
        float rotation = Mathf.Atan2(m_MoveValue.axis.x, m_MoveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        // Orintation
        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);

    }

    // Allows the player to press a button to quickly turn a set amount, however this breaks object throwing
    private void SnapRotation()
    {
        float snapValue = 0.0f;

        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            snapValue = -Mathf.Abs(m_RotateIncrement);
        }

        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = Mathf.Abs(m_RotateIncrement);
        }

        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
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

    // Uses a vector to figure out how the contacted normal moves the player
    private Vector3 ProjectOnContactPlane (Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
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

        if (m_MoveValue.axis.magnitude > 0)
        {
            velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
        }

        if (m_MoveValue.axis.magnitude == 0 && velocity != Vector3.zero)
        {
            velocity += (xAxis * (newX - currentX) + zAxis * (newZ - currentZ)) / moveDecel;
        }

    }

    // Applies a gravity force on the player over time
    void ConstantGravity()
    {
        velocity += Physics.gravity * Time.deltaTime;
    }

}
