using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementScript : MonoBehaviour
{
    private float healthAmount = 100f; // Moved from HealthManagerScript

    [Header("Movement Speed")]
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;

    [Header("Crouching")]
    public float crouchHeight = 0.4f;
    private bool isCrouching = false;
    private bool CanStandUp = true;
    private Vector3 originalCamPos;
    [SerializeField] private float crouchCamPosOffset = 0.5f;
    private float originalHeight;
    private float crouchTransitionSpeed = 5f; // Speed of transition
    private Coroutine crouchRoutine;

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

    [Header("Health Management")]
    public Image HealthBar; // Moved from HealthManagerScript

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

        originalHeight = playerCollider.height;
        originalCamPos = cam.transform.localPosition;

        // Initialize health-related UI
        HealthBar.fillAmount = healthAmount / 100;
    }

    private void Update()
    {
        Movement();
        leaningMoment();
        camMovement();
        HandleCrouch();
    }

    private void camMovement()
    {
        transform.Rotate(transform.up * Input.GetAxis("Mouse X") * rotationSpeed);

        camX -= Input.GetAxis("Mouse Y") * rotationSpeed;

        camX = Mathf.Clamp(camX, -75, 75);

        cam.transform.localEulerAngles = new Vector3(camX, 0, 0);
    }

    private void Movement()
    {
        Vector3 MovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            MovementInput *= sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && isCrouching)
        {
            isCrouching = false;
            MovementInput *= sprintSpeed;
            if (crouchRoutine != null) StopCoroutine(crouchRoutine);
            crouchRoutine = StartCoroutine(SmoothStandUp());
        }
        else if (isCrouching)
        {
            MovementInput *= crouchSpeed;
        }
        else
        {
            MovementInput *= speed;
        }

        Vector3 MoveDir = (transform.forward * MovementInput.z + transform.right * MovementInput.x) / 2;
        rb.velocity = new Vector3(MoveDir.x, rb.velocity.y, MoveDir.z);

        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.3f);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isCrouching)
            {
                if (crouchRoutine != null) StopCoroutine(crouchRoutine);
                crouchRoutine = StartCoroutine(SmoothStandUp());
            }
            else
            {
                if (crouchRoutine != null) StopCoroutine(crouchRoutine);
                crouchRoutine = StartCoroutine(SmoothCrouch());
            }
        }
    }

    private IEnumerator SmoothCrouch()
    {
        isCrouching = true;
        float elapsedTime = 0f;
        Vector3 targetCamPos = new Vector3(originalCamPos.x, originalCamPos.y - crouchCamPosOffset, originalCamPos.z);
        float targetHeight = crouchHeight;

        while (elapsedTime < crouchTransitionSpeed)
        {
            playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, (elapsedTime / crouchTransitionSpeed));
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetCamPos, (elapsedTime / crouchTransitionSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerCollider.height = targetHeight;
        cam.transform.localPosition = targetCamPos;
    }

    private IEnumerator SmoothStandUp()
    {
        isCrouching = false;
        float elapsedTime = 0f;
        float targetHeight = originalHeight;

        while (elapsedTime < crouchTransitionSpeed)
        {
            playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, (elapsedTime / crouchTransitionSpeed));
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, originalCamPos, (elapsedTime / crouchTransitionSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        playerCollider.height = targetHeight;
        cam.transform.localPosition = originalCamPos;
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
        if (other.gameObject.CompareTag("Teleport"))
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemies"))
        {
            TakingDamage(10);
        }
        if(collision.gameObject.CompareTag("Aid"))
        {
            Destroy(collision.gameObject);
            Healing(10);
        }
        if(collision.gameObject.CompareTag("RedAid"))
        {
            Destroy(collision.gameObject);
            Healing(30);
        }
    }

    // Health management methods
    public void TakingDamage(float damage)
    {
        healthAmount -= damage;
        HealthBar.fillAmount = healthAmount / 100;
    }

    public void Healing(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        HealthBar.fillAmount = healthAmount / 100;
    }
}
