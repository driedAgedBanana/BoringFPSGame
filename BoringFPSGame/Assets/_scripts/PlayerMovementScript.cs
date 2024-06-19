using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 10;
    public float rotationSpeed = 2;
    Quaternion startRotation;
    public float amt;
    public float slerpAMT;
    public float leaningAmount = 20f;
    public float leaningSpeed = 15f;
    bool isGrounded = false;
    Rigidbody rb;
    private Camera cam;
    private float camX;

    public GameObject target;

    private bool isClimbing = false;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        startRotation = transform.localRotation;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
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

    //for climbing
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Climbable")
        {
            if (isClimbing)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

                rb.useGravity = false;
            }
            else
            {
                rb.useGravity = true;
            }
        }
    }

    private void leaningMoment()
    {
        float leanInput = Input.GetAxis("Lean Axes");
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, leanInput * leaningAmount);


        //if (Input.GetKey(KeyCode.Q))
        //{
        //    Quaternion newRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + amt);
        //    transform.localRotation = Quaternion.Slerp(transform.localRotation, newRotation, Time.deltaTime * slerpAMT);
        //}
        //else if (Input.GetKey(KeyCode.E))
        //{
        //    Quaternion newRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z - amt);
        //    transform.localRotation = Quaternion.Slerp(transform.localRotation, newRotation, Time.deltaTime * slerpAMT);
        //}
        //else
        //{
        //    transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, Time.deltaTime * slerpAMT);
        //}
    }
}
