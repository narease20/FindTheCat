using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public Vector3 crouchAmount = new Vector3(1, 0.5f, 1);
    private Vector3 regularSize;
    public float charContRegHeight;

    public float speed = 12f, gravity = 9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [SerializeField, Range(0.1f, 1.35f), Tooltip("The distance checked for an object above the player when they are crouched.")]
    public float heightMeasureDistance = 0.5f;

    CapsuleCollider cylinder;

    Vector3 velocity;
    bool isGrounded;

    private void Start()
    {
        regularSize = transform.localScale;
        charContRegHeight = controller.height;
        //cylinder = GetComponentInChildren<CapsuleCollider>();
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
        transform.localScale = crouchAmount;
        controller.Move(new Vector3(0, transform.position.y - crouchAmount.y, 0));
        controller.height = 0.95f;
    }
    void CrouchEnd()
    {
        transform.localScale = regularSize;
        controller.Move(new Vector3(0, transform.position.y + crouchAmount.y, 0));
        controller.height = charContRegHeight;
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
