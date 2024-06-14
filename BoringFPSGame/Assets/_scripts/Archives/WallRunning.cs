using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float wallRunTime;
    public float maxWallRunningTime;
    private float wallRunTimer;

    [Header("Inputs")]
    public KeyCode upwardsKey = KeyCode.LeftShift;
    public KeyCode downwardsKey = KeyCode.LeftControl;
    public KeyCode wallJumpingKey = KeyCode.Space;
    private bool upwardRunning;
    private bool downwardRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Existing Wallrunning State")]
    private bool existingWall;
    public float ExitWallTime;
    private float exitWallTimer;
    public float offWallPushForce;

    [Header("Gravity")]
    public bool usingGravity;
    public float gravityCounteringForce;

    [Header("Wall Detection")]
    public float wallCheckDistance;
    public float minimumJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool WallLeft;
    private bool WallRight;

    [Header("References")]
    private PlayerController playerController;
    public PlayerCam playerCam;
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
        if ((WallLeft || WallRight) && verticalInput > 0 && AboveGroundHeightCheck() && !existingWall)
        {
            if (!playerController.wallrunning)
            {
                StartWallRunning();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && playerController.wallrunning)
            {
                existingWall = true;
                exitWallTimer = ExitWallTime;

                // Apply a small force to push the player off the wall
                Vector3 wallNormal = WallRight ? rightWallHit.normal : leftWallHit.normal;
                rb.AddForce(wallNormal *offWallPushForce, ForceMode.Impulse); // Adjust the force value as needed
            }

            if (Input.GetKey(wallJumpingKey))
            {
                WallJumping();
            }
        }

        //State 2 - existing wall
        else if (existingWall)
        {
            if (playerController.wallrunning)
            {
                StopWallRunning();
            }

            if (ExitWallTime > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                existingWall = false;
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

        wallRunTimer = maxWallRunningTime;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //Apply epic camera stuff
        playerCam.DoFov(90f);

        if (WallLeft)
        {
            playerCam.DoTilt(-5f);
        }
        if (WallRight)
        {
            playerCam.DoTilt(5f);
        }
    }

    private void WallRunningHandlingMovement()
    {
        rb.useGravity = false;

        Vector3 wallNormal = WallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((Orientation.forward - wallForward).magnitude > (Orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //adding forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        //upwards/downwards
        if (upwardRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
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

        //weaken the gravity
        if (usingGravity)
        {
            rb.AddForce(transform.up * gravityCounteringForce, ForceMode.Force);
        }
    }

    private void StopWallRunning()
    {
        playerController.wallrunning = false;

        //Stop the effect
        playerCam.DoFov(80f);
        playerCam.DoTilt(0f);
    }

    private void WallJumping()
    {
        existingWall = usingGravity;
        exitWallTimer = ExitWallTime;

        Vector3 wallNormal = WallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        // Ensure the player doesn't gain an excessive boost
        Vector3 currentVelocity = rb.velocity;
        rb.velocity = new Vector3(currentVelocity.x, Mathf.Min(currentVelocity.y, 0), currentVelocity.z);

        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
