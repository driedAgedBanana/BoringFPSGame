using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class PlayerMovementScript : MonoBehaviour
{
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 10;
    public float rotationSpeed = 2;
    public float leaningAmount = 20f;
    bool isGrounded = false;
    Rigidbody rb;
    private Camera cam;
    private float camX;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Movement();
        leaningMoment();
        camMovement();
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

        rb.velocity = transform.forward * MovementInput.z + transform.right * MovementInput.x + transform.up * rb.velocity.y;

        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.3f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    private void leaningMoment()
    {
        float leanInput = Input.GetAxis("Lean Axes");
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, leanInput * leaningAmount);
    }

    private void camMovement()
    {
        transform.Rotate(transform.up * Input.GetAxis("Mouse X") * rotationSpeed);

        camX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        camX = Mathf.Clamp(camX, -90, 90);

        cam.transform.localEulerAngles = new Vector3(camX, 0, 0);
    }
}
