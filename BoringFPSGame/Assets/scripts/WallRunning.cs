using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public float wallRunForce;
    public float wallClimbSpeed;
    public float wallRunTime;
    private float wallRunTimer;

    [Header("Inputs")]
    public KeyCode upwardsKey = KeyCode.LeftShift;
    public KeyCode downwardsKey = KeyCode.LeftControl;
    private bool upwardRunning;
    private bool downwardRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Wall Detection")]
    public float wallCheckDistance;
    public float minimumJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool WallLeft;
    private bool WallRight;

    [Header("References")]
    private PlayerController playerController;
    public Transform Orientation;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        checkForWall();
        StateMachines();
    }

    private void FixedUpdate()
    {
        if (playerController.wallrunning)
        {
            WallRunningHandlingMovement();
        }
    }

    private void checkForWall()
    {
        //basically checking if there is any wall nearby
        WallRight = Physics.Raycast(transform.position, Orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        WallLeft = Physics.Raycast(transform.position, -Orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGroundHeightCheck()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight, whatIsGround);
    }

    private void StateMachines()
    {
        //getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardRunning = Input.GetKey(upwardsKey);
        downwardRunning = Input.GetKey(downwardsKey);

        //state 1 - wallrunning moment
        if ((WallLeft || WallRight) && verticalInput > 0 && AboveGroundHeightCheck())
        {
            if (!playerController.wallrunning)
            {
                StartWallRunning();
            }
        }

        //state 3 - nothing happened
        else
        {
            if (playerController.wallrunning)
            {
                StopWallRunning();
            }
        }
    }

    private void StartWallRunning()
    {
        playerController.wallrunning = true;
    }

    private void WallRunningHandlingMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        Vector3 wallNormal = WallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((Orientation.forward - wallForward).magnitude > (Orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //adding forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //upwards/downwards
        if(upwardRunning)
        {
            rb.velocity = new Vector3 (rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }
        else if (downwardRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }

        //using force to push player to the wall
        if (!(WallLeft && horizontalInput > 0) && !(WallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    private void StopWallRunning()
    {
        playerController.wallrunning = false;
    }
}
