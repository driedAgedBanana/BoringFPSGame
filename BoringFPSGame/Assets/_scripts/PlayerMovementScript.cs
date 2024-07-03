using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    [Header("Movement Speed")]
    public float speed = 5f;
    public float sprintSpeed = 10f;

    [Header("Jumping")]
    public float jumpForce = 10;
    bool isGrounded = false;

    [Header("Leaning")]
    public float rotationSpeed = 2;
    public float amt;
    public float slerpAMT;
    public float leaningAmount = 20f;
    public float leaningSpeed = 15f;

    [Header("Special for teleportation icon")]
    public Image teleportationIcon;

    //[Header("Crouching and Sliding Manager")]
    //public float crouchSpeed = 3f;
    //public float slideSpeed = 20f;

    //private bool isSliding = false;
    //private Vector3 slideDirection;
    //[SerializeField] private float slidingTimer = 0f;
    //public float MaxSlidingTimer = 2.5f;
    //private float originalHeight;
    //public float crouchHeight = 1f;

    Rigidbody rb;
    private Camera cam;
    private float camX;

    private CapsuleCollider playerCollider;

    public GameObject target;

    private bool isClimbing = false;

    private Quaternion startRotation;
    private Quaternion targetLeanRotation;

    private void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        playerCollider = GetComponentInChildren<CapsuleCollider>();

        startRotation = transform.localRotation;
        targetLeanRotation = startRotation;

        //teleportationIcon.gameObject.SetActive(false);

        //originalHeight = GetComponent<Collider>().bounds.size.y;
    }

    private void Update()
    {
        Movement();
        leaningMoment();
        camMovement();
    }

    private void camMovement()
    {
        transform.Rotate(transform.up * Input.GetAxis("Mouse X") * rotationSpeed);

        camX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        camX = Mathf.Clamp(camX, -90, 90);

        cam.transform.localEulerAngles = new Vector3(camX, 0, 0);
    }

    private void Movement()
    {
        Vector3 MovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            MovementInput *= sprintSpeed;
        }
        else
        {
            MovementInput *= speed;
        }

        Vector3 MoveDir = (transform.forward * MovementInput.z + transform.right * MovementInput.x) / 2;
        rb.velocity = new Vector3(MoveDir.x, rb.velocity.y, MoveDir.z);

        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.3f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    private void leaningMoment()
    {
        float leanInput = Input.GetAxis("Lean Axes");

        if (leanInput < 0) // Lean right
        {
            targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, -leaningAmount);
        }
        else if (leanInput > 0) // Lean left
        {
            targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, leaningAmount);
        }
        else // No lean
        {
            targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0 * leaningSpeed * Time.deltaTime);
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLeanRotation, Time.deltaTime * leaningSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Teleport"))
        {
            teleportationIcon.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Teleport"))
        {
            teleportationIcon.gameObject.SetActive(false);
        }
    }
}
