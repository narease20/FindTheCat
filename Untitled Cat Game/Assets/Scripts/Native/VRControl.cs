using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRControl : MonoBehaviour
{
    private Transform head = null;

    public float maxHeadHeight = 2.5f;
    public float minHeadHeight = 0.5f;

    public SteamVR_Action_Vector2 moveValue = null;

    private CapsuleCollider player;
    public SteamVR_Camera vision;

    private Rigidbody rb;

    public float sensitivity = 0.1f;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    float minGroundDotProduct;

    private bool onGround;
    Vector3 contactNormal;
    Vector3 velocity;
    Transform LeftHand;
    Transform RightHand;

    // Start is called before the first frame update
    void Start()
    {
        head = SteamVR_Render.Top().head;
        rb = GetComponent<Rigidbody>();
        player = GetComponent<CapsuleCollider>();
        vision = GetComponent<SteamVR_Camera>();
        LeftHand = GetComponentInChildren<Transform>();
        RightHand = GetComponentInChildren<Transform>();

        velocity.y = Mathf.Clamp(velocity.y, 0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        HandleHeight();
        CheckGround();
    }

    private void FixedUpdate()
    {
        if (!onGround)
        {
            Gravity();
        }
        CalculateMovement();
    }

    private void HandleHeight()
    {
        // Get heads local space
        float headHeight = Mathf.Clamp(head.localPosition.y, minHeadHeight, maxHeadHeight);
        player.height = headHeight;

        // cut in half
        Vector3 newCenter = Vector3.zero;
        
        newCenter.y = headHeight / 2;
        //newCenter.y += player.center.y;

        // Move capsule in local space
        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;

        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        player.center = new Vector3(newCenter.x, newCenter.y - 1, newCenter.z);
        player.height = headHeight;
        //LeftHand.transform.position = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y - 1, player.transform.localPosition.z);
        //RightHand.transform.position = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y - 1, player.transform.localPosition.z);
        //player.center = new Vector3(0, newCenter.y - 1, 0);

    }


    private void CalculateMovement()
    {
        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        if (moveValue.axis.magnitude < sensitivity)
        {
            // Slow player down
            rb.velocity /= 1.2f;
            if(rb.velocity.magnitude < 0.1f || rb.velocity.magnitude > 100)
            {
                rb.velocity = Vector3.zero;
            }
        }
        if (moveValue.axis.magnitude > sensitivity)
        {
            // speed player up
            velocity += orientation * (moveValue.axis.magnitude * Vector3.forward);
        }

        //rb.MovePosition(velocity);
        rb.velocity = velocity;
        /*
        // Check if not moving
        if (moveValue.axis.magnitude == 0)
        {
            speed = 0;
            rb.velocity /= moveDecel;
        }

        if (moveValue.axis.magnitude > 0.2f)
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



    // Figures out the player's orientation from the play space's sensors
    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(moveValue.axis.x, moveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        // Orintation
        Vector3 orientationEuler = new Vector3(0, head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);

    }


    private void CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.transform.position, transform.TransformDirection(Vector3.up), out hit, maxHeadHeight + 0.2f))
        {
            Debug.DrawRay(head.transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.black);
            EvaluateCollision(hit.normal);
            Debug.Log(onGround);
            velocity.y = 0;
        }
        else
        {
            onGround = false;
        }
    }
    void EvaluateCollision(Vector3 normal)
    {
        if (normal.y >= minGroundDotProduct)
        {
            onGround = true;
            contactNormal = normal;
            
            //Debug.Log(contactNormal.x + ", " + contactNormal.y + ", " + contactNormal.z);
        }
    }
    
    void Gravity()
    {
        velocity += (Physics.gravity * Time.deltaTime) / 7;
        //rb.useGravity = true;
        //rb.velocity = velocity;
    }


}
