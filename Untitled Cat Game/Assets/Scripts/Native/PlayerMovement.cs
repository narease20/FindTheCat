using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public Vector3 crouchAmount = new Vector3(1, 0.5f, 1);
    private Vector3 regularSize;
    public float charContRegHeight;

    [SerializeField, Range(-100f, 100f)]
    public float speed = 3f, gravity = 9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [SerializeField, Range(0.1f, 1.35f), Tooltip("The distance checked for an object above the player when they are crouched.")]
    public float heightMeasureDistance = 0.5f;

    CapsuleCollider myBody;
    CapsuleCollider myRigidBody;
    public GameObject cylBody;
    public Transform cameraLoc;

    public GameObject handHolder;

    Vector3 velocity;
    bool isGrounded;
    bool isCrouching = false;
    public float crouchTimer;
    float maxCrouchTime = 0.75f;

    private void Start()
    {
        Mathf.Clamp(crouchTimer, 0, maxCrouchTime);
        myBody = GetComponent<CapsuleCollider>();
        regularSize = cylBody.transform.localScale;//transform.localScale;
        charContRegHeight = controller.height;
        myRigidBody = GetComponentInChildren<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(new Vector3(transform.localPosition.x, controller.height + transform.localPosition.y + crouchAmount.y, transform.localPosition.z), transform.TransformDirection(Vector3.up) * heightMeasureDistance, Color.black);

        CrouchHandler();
        Movement();

        if (!CheckAbovePlayer() && isCrouching)
        {
            if (crouchTimer < maxCrouchTime)
            {
                crouchTimer += Time.deltaTime;
            }
            if (crouchTimer == maxCrouchTime)
            {
                CrouchEnd();
                isCrouching = false;
                crouchTimer = 0;
            }
        }
        if (!isCrouching && crouchTimer != 0)
        {
            crouchTimer = 0;
        }
        /*
        if (crouchTimer < maxCrouchTime && !CheckAbovePlayer() && isCrouching)
        {
            crouchTimer += Time.deltaTime;
        }
        if (crouchTimer == maxCrouchTime && !CheckAbovePlayer() && isCrouching)
        {
            CrouchEnd();
            isCrouching = false;
            crouchTimer = 0;
        }

        if (!CheckAbovePlayer() && isCrouching)
        {
            CrouchEnd();
            isCrouching = false;
        }
        */
    }

    void Movement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime * 2;

        controller.Move(velocity * Time.deltaTime);
    }

    void CrouchHandler()
    {
        if (Input.GetButtonDown("Crouch") && !isCrouching && isGrounded)
        {
            CrouchStart();
            isCrouching = true;
        }
        // If under an object and let go of crouch player forced to stay in crouch until they press the button again
        if (Input.GetButtonUp("Crouch") && !CheckAbovePlayer() && isCrouching && isGrounded)
        {
            //transform.position = new Vector3(transform.position.x, transform.position.y + 1000, transform.position.z);
            CrouchEnd();
            isCrouching = false;
        }
    }


    void CrouchStart()
    {
        //cameraLoc.position = new Vector3(cameraLoc.position.x, cameraLoc.position.y - 0.5f, cameraLoc.position.z);
        //myBody.height = 0.5f;
        //myRigidBody.height = 0.5f;
        //cylBody.transform.localScale = new Vector3(regularSize.x, regularSize.y - 0.5f, regularSize.z);
        controller.height = 0.50f;
        controller.Move(new Vector3(0, transform.position.y - crouchAmount.y, 0));
        return;
    }
    void CrouchEnd()
    {
        //cameraLoc.position = new Vector3(cameraLoc.position.x, cameraLoc.position.y + 0.5f, cameraLoc.position.z);
        //myBody.height = 1f;
        //myRigidBody.height = 2f;
        //cylBody.transform.localScale = regularSize;
        controller.Move(new Vector3(0, transform.position.y + controller.center.y, 0));
        controller.height = charContRegHeight;
        return;
    }


    bool CheckAbovePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.localPosition.x, transform.localPosition.y + controller.center.y + crouchAmount.y, transform.localPosition.z), transform.TransformDirection(Vector3.up), out hit, heightMeasureDistance))//Physics.Raycast(characterController.transform.position, transform.TransformDirection(Vector3.up), out hit, heightMeasureDistance))
        {
            //Debug.DrawRay(characterController.transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.black);
            Debug.DrawRay(new Vector3(transform.localPosition.x, transform.localPosition.y + controller.center.y + crouchAmount.y, transform.localPosition.z), transform.TransformDirection(Vector3.up) * heightMeasureDistance, Color.red);
            return true;
        }

        return false;
    }
}
