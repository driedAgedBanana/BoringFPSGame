using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    private float playerHeight = 2;

    [SerializeField] private Transform orientation;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;
    public KeyCode SprintKey = KeyCode.LeftShift;

    [Header("Movement")]
    public float moveSpeed = 6f;
    float movementMultiplier = 10f;
    [SerializeField] float AirMultiplier = 0.4f;

    private float HorizontalMovement;
    private float VerticalMovement;

    [Header("Sprinting")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float acceleration = 10f;

    [Header("Jumping and ground modifier")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    public float gravityModifier;
    private bool isGrounded;
    private float groundDistance = 0.4f;
    public float JumpForce;

    private RaycastHit slopeHit;

    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    private Vector3 MoveDirection;
    private Vector3 SlopeMoveDirection;

    public Rigidbody rb;
    public Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Physics.gravity *= gravityModifier;

        // Reference to the camera's transform
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); //check the player's bottom
        print(isGrounded);

        MovementInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(JumpKey) && isGrounded)
        {
            PlayerJump();
        }

        SlopeMoveDirection = Vector3.ProjectOnPlane(MoveDirection, slopeHit.normal);
    }

    private void MovementInput()
    {
        HorizontalMovement = Input.GetAxisRaw("Horizontal");
        VerticalMovement = Input.GetAxisRaw("Vertical");

        // Calculate movement direction based on camera's orientation
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Project forward and right vectors onto the XZ plane (ignore Y component)
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        MoveDirection = (orientation.forward * VerticalMovement + orientation.right * HorizontalMovement).normalized;
    }

    private void ControlSpeed()
    {
        if (Input.GetKey(SprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
    }

    private void PlayerJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0 , rb.velocity.z);
        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    private void ControlDrag()
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

    private void FixedUpdate()
    {
        MovingPlayer();
    }

    private void MovingPlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(MoveDirection * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(SlopeMoveDirection * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(MoveDirection * moveSpeed * movementMultiplier * AirMultiplier, ForceMode.Acceleration);
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                //if this is true, we are on a slope
                return true;
            }
            else
            {
                //or else we are on the flat ground
                return false;
            }
        }
        return false;
    }
}
