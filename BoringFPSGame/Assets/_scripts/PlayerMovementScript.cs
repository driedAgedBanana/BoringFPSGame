using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class PlayerMovementScript : MonoBehaviour
{
    private shootingScript Shooting;

    private float healthAmount = 100f;

    [Header("Movement Speed")]
    public float speed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;

    [Header("Crouching")]
    public TMP_Text warningText;

    public float crouchHeight = 0.4f;
    private bool isCrouching = false;
    private bool CanStandUp = true;
    private Vector3 originalCamPos;
    [SerializeField] private float crouchCamPosOffset = 0.5f;
    private float originalHeight;
    private float crouchTransitionSpeed = 5f; // Speed of transition
    private Coroutine crouchRoutine;
    private Coroutine warningRoutine;

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

    [Header("For ammo and munitions")]
    public GameObject ammoClip;
    public GameObject ammoBox;
    private int TotalAmmo;

    [Header("Flashlight Settings")]
    public Light flashLight;
    public Image batteryIndicator;
    public bool drainOverTime;
    public float maxBrightness;
    public float minBrightness;
    public float drainingRate;

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

        batteryIndicator.fillAmount = flashLight.intensity / maxBrightness;

        Shooting = gameObject.GetComponentInChildren<shootingScript>();
        if (Shooting == null)
        {
            Debug.LogError("Shooting script not found on the GameObject.");
        }
    }

    private void Update()
    {
        Movement();
        leaningMoment();
        camMovement();
        HandleCrouch();
        handleFlashlight();
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

    #region it's a nightmare

    private void HandleCrouch()
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = Vector3.up;

        float rayLength = 3f; // Adjust this length if needed

        // Perform the raycast to check for obstacles above the player
        bool isObstructed = Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hitInfo, rayLength);

        // Draw the ray for visualization
        Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.yellow);

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isCrouching)
            {
                if (crouchRoutine != null) StopCoroutine(crouchRoutine);
                if (!isObstructed) // Only stand up if not obstructed
                {
                    crouchRoutine = StartCoroutine(SmoothStandUp());
                    HideWarningText(); // Hide warning message
                }
                else
                {
                    Debug.Log("Cannot stand up, obstruction detected!");
                    if (warningRoutine != null) StopCoroutine(warningRoutine);
                    warningRoutine = StartCoroutine(DisplayWarning("Cannot stand up here!")); // Show warning message
                }
            }
            else
            {
                if (crouchRoutine != null) StopCoroutine(crouchRoutine);
                crouchRoutine = StartCoroutine(SmoothCrouch());
                HideWarningText(); // Hide warning message when crouching
            }
        }
    }

    private IEnumerator DisplayWarning(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true); // Make sure the text is active

        // Fade in
        Color textColor = warningText.color;
        for (float t = 0; t <= 1; t += Time.deltaTime / 0.5f) // Adjust duration as needed
        {
            textColor.a = Mathf.Lerp(0, 1, t);
            warningText.color = textColor;
            yield return null;
        }
        textColor.a = 1;
        warningText.color = textColor;

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);

        // Fade out
        for (float t = 0; t <= 1; t += Time.deltaTime / 0.5f) // Adjust duration as needed
        {
            textColor.a = Mathf.Lerp(1, 0, t);
            warningText.color = textColor;
            yield return null;
        }
        textColor.a = 0;
        warningText.color = textColor;

        warningText.gameObject.SetActive(false); // Deactivate text after fading out
    }

    private void HideWarningText()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false); // Hide the text immediately
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

    #endregion

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

    private void handleFlashlight()
    {
        flashLight.intensity = Mathf.Clamp(flashLight.intensity, minBrightness, maxBrightness);
        if(drainOverTime == true && flashLight.enabled == true)
        {
            if(flashLight.intensity > minBrightness)
            {
                flashLight.intensity -= Time.deltaTime * (drainingRate / 1000);
            }
        }

        batteryIndicator.fillAmount = flashLight.intensity / maxBrightness;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(flashLight != null)
            {
                flashLight.enabled = !flashLight.enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            replaceBattery(1f);
        }
    }

    //All mechanic for the flashlight
    private void replaceBattery(float amount)
    {
        flashLight.intensity += amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Shooting == null)
        {
            Debug.LogError("Shooting is null");
            return;
        }

        if (other.gameObject.CompareTag("AmmoClip"))
        {
            Debug.Log("AmmoClip triggered");
            Destroy(other.gameObject);
            Shooting.PickupAmmoClip(10); // Assuming a clip gives 10 ammo
        }
        else if (other.gameObject.CompareTag("AmmoBox"))
        {
            Debug.Log("AmmoBox triggered");
            Destroy(other.gameObject);
            Shooting.PickupAmmoBox(60); // Assuming a box gives 60 ammo
        }
        else if (other.gameObject.CompareTag("Teleport"))
        {
            if (teleportationIcon != null)
            {
                teleportationIcon.gameObject.SetActive(true);
            }
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
