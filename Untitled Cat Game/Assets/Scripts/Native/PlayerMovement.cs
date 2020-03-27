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

    private void Start()
    {
        regularSize = cylBody.transform.localScale;//transform.localScale;
        charContRegHeight = controller.height;
        myRigidBody = GetComponentInChildren<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(new Vector3(transform.localPosition.x, controller.height + transform.localPosition.y - crouchAmount.y, transform.localPosition.z), transform.TransformDirection(Vector3.up) * heightMeasureDistance, Color.black);
        if (Input.GetButtonDown("Crouch"))
        {
            CrouchStart();
        }
        // If under an object and let go of crouch player forced to stay in crouch until they press the button again
        if (Input.GetButtonUp("Crouch") && !CheckAbovePlayer())
        {
            //transform.position = new Vector3(transform.position.x, transform.position.y + 1000, transform.position.z);
            CrouchEnd();
        }

        Movement();
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

    void CrouchStart()
    {
        cameraLoc.position = new Vector3(cameraLoc.position.x, cameraLoc.position.y - 0.5f, cameraLoc.position.z);
        myBody.height = 0.5f;
        myRigidBody.height = 0.5f;
        controller.Move(new Vector3(0, transform.position.y - crouchAmount.y, 0));
        controller.height = 0.95f;
    }
    void CrouchEnd()
    {
        cameraLoc.position = new Vector3(cameraLoc.position.x, cameraLoc.position.y + 0.5f, cameraLoc.position.z);
        controller.Move(new Vector3(0, transform.position.y + crouchAmount.y, 0));
        controller.height = charContRegHeight;
        myBody.height = 1f;
        myRigidBody.height = 2f;
    }


    bool CheckAbovePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.localPosition.x, controller.height + transform.localPosition.y - crouchAmount.y, transform.localPosition.z), transform.TransformDirection(Vector3.up), out hit, heightMeasureDistance))//Physics.Raycast(characterController.transform.position, transform.TransformDirection(Vector3.up), out hit, heightMeasureDistance))
        {
            //Debug.DrawRay(characterController.transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.black);
            Debug.DrawRay(new Vector3(transform.localPosition.x, controller.height + transform.localPosition.y - crouchAmount.y, transform.localPosition.z), transform.TransformDirection(Vector3.up) * heightMeasureDistance, Color.green);
            return true;
        }

        return false;
    }
}
