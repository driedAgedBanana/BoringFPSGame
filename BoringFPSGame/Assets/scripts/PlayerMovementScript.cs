using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    private float playerHeight = 2;

    [Header("Keybinds")]
    public KeyCode JumpKey = KeyCode.Space;

    [Header("Movement")]
    public float moveSpeed = 6f;
    
    float movementMultiplier = 10f;
    private float Drag = 6f;

    private float HorizontalMovement;
    private float VerticalMovement;

    [Header("Jumping")]
    private bool isGrounded;
    public float JumpForce;

    private Vector3 MoveDirection;

    public Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
        print(isGrounded);

        MovementInput();
        ControlDrag();

        if(Input.GetKeyDown(JumpKey) && isGrounded)
        {
            PlayerJump();
        }
    }

    private void MovementInput()
    {
        HorizontalMovement = Input.GetAxisRaw("Horizontal");
        VerticalMovement = Input.GetAxisRaw("Vertical");

        //moving towards the direction where the player is looking
        MoveDirection = transform.forward * VerticalMovement + transform.right * HorizontalMovement;
    }

    private void PlayerJump()
    {
        rb.AddForce(transform.up * JumpForce, ForceMode.Impulse);
    }

    private void ControlDrag()
    {
        rb.drag = Drag;
    }

    private void FixedUpdate()
    {
        MovingPlayer();
    }

    private void MovingPlayer()
    {
        rb.AddForce(MoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
    }
}
