using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerMovementScript : MonoBehaviour
{
    float playerHeight = 2f;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float airMultiplier = 0.4f;
    float movementMultiplier = 10f;

    [Header("Leaning")]
    private Quaternion initialRotation;
    public float amt, slerpAMT;
    [SerializeField] private KeyCode leanLeft = KeyCode.Q;
    [SerializeField] private KeyCode leanRight = KeyCode.E;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    public bool isGrounded { get; private set; }

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();
        LeaningMoment();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    void LeaningMoment()
    {

        if (Input.GetKey(leanLeft))
        {
            Quaternion newLeanRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + amt);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, newLeanRotation, Time.deltaTime * slerpAMT);
        }
        else if (Input.GetKey(leanRight))
        {
            Quaternion newLeanRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - amt);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, newLeanRotation, Time.deltaTime * slerpAMT);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, initialRotation, Time.deltaTime * slerpAMT);
        }
    }
}
