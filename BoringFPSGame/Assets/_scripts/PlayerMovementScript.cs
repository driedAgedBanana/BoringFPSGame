using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovementScript : MonoBehaviour
{
    private shootingScript Shooting;

    public float healthAmount = 100f; // Player's health

    [Header("Movement Speed")]
    public float speed = 5f; // Normal movement speed
    public float sprintSpeed = 10f; // Speed when sprinting
    public float crouchSpeed = 2.5f; // Speed when crouching

    [Header("Crouching")]
    public TMP_Text warningText; // Text to show warnings during crouching

    public float crouchHeight = 0.4f; // Height when crouching
    private bool isCrouching = false; // Check if the player is crouching
    private bool CanStandUp = true; // If the player can stand up while crouching
    private Vector3 originalCamPos; // Original camera position
    [SerializeField] private float crouchCamPosOffset = 0.5f; // Camera offset during crouch
    private float originalHeight; // Original height of the player collider
    private float crouchTransitionSpeed = 5f; // Speed of crouch transition
    private Coroutine crouchRoutine; // Coroutine for crouch transition
    private Coroutine warningRoutine; // Coroutine for warning text display

    [Header("Jumping")]
    public float jumpForce = 10; // Force of the jump
    bool isGrounded = false; // Check if the player is grounded

    [Header("Leaning")]
    public float rotationSpeed = 2; // Speed of camera rotation
    public float amt; // Leaning amount, not used directly
    public float slerpAMT; // Slerp amount, not used directly
    public float leaningAmount = 20f; // Amount to lean left or right
    public float leaningSpeed = 15f; // Speed of leaning transition

    [Header("Special for teleportation icon and interaction text")]
    public Image teleportationIcon; // Icon indicating teleportation point
    public TextMeshProUGUI InteractionText; // Text for interactions, not used in the script

    [Header("Health Management")]
    public Image HealthBar; // Health bar UI element

    [Header("For ammo and munitions")]
    public GameObject ammoClip; // Ammo clip prefab
    public GameObject ammoBox; // Ammo box prefab
    private int TotalAmmo; // Total ammo, not used in the script

    [Header("Flashlight Settings")]
    public Light flashLight; // Flashlight light source
    public Image batteryIndicator; // Battery indicator UI element

    [Header("Special for removing wall on getting key card")]
    public GameObject WallDoor; // Wall or door to remove with key card

    public GameObject FlashlightBattery; // Flashlight battery prefab

    public bool drainOverTime; // Whether the flashlight drains over time
    public float maxBrightness; // Maximum brightness of the flashlight
    public float minBrightness; // Minimum brightness of the flashlight
    public float drainingRate; // Rate at which the flashlight drains

    Rigidbody rb; // Rigidbody component for player movement
    private Camera cam; // Player's camera
    private float camX; // Camera's vertical angle

    private CapsuleCollider playerCollider; // Collider for player

    public GameObject target; // Target for interaction, not used in the script

    private bool isClimbing = false; // Check if the player is climbing, not used in the script

    private Quaternion startRotation; // Initial rotation of the player
    private Quaternion targetLeanRotation; // Target rotation for leaning

    private void Start()
    {
        // Lock cursor to the center of the screen and hide it
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        Time.timeScale = 1f; // Set game time scale to normal

        rb = GetComponent<Rigidbody>(); // Get Rigidbody component
        cam = GetComponentInChildren<Camera>(); // Get Camera component
        playerCollider = GetComponentInChildren<CapsuleCollider>(); // Get CapsuleCollider component

        startRotation = transform.localRotation; // Store the initial rotation
        targetLeanRotation = startRotation; // Set target lean rotation to initial

        originalHeight = playerCollider.height; // Store original collider height
        originalCamPos = cam.transform.localPosition; // Store original camera position

        // Initialize health-related UI
        HealthBar.fillAmount = healthAmount / 100; // Set health bar fill based on healthAmount

        batteryIndicator.fillAmount = flashLight.intensity / maxBrightness; // Set battery indicator fill based on flashlight intensity

        flashLight.enabled = false; // Turn off the flashlight initially

        Shooting = gameObject.GetComponentInChildren<shootingScript>(); // Get the shooting script

        if (Shooting == null)
        {
            Debug.LogError("Shooting script not found on the GameObject."); // Log error if shooting script is not found
        }
    }

    private void Update()
    {
        Movement(); // Handle player movement
        leaningMoment(); // Handle player leaning
        camMovement(); // Handle camera movement
        //HandleCrouch(); // Handle crouching, commented out
        handleFlashlight(); // Handle flashlight behavior
    }

    private void camMovement()
    {
        // Rotate player based on mouse input
        transform.Rotate(transform.up * Input.GetAxis("Mouse X") * rotationSpeed);

        // Adjust camera's vertical angle based on mouse input
        camX -= Input.GetAxis("Mouse Y") * rotationSpeed;
        camX = Mathf.Clamp(camX, -75, 75); // Clamp camera angle to prevent excessive tilting

        cam.transform.localEulerAngles = new Vector3(camX, 0, 0); // Apply vertical angle to the camera
    }

    private void Movement()
    {
        // Get movement input from user
        Vector3 MovementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // Adjust movement speed based on user input
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        {
            MovementInput *= sprintSpeed; // Sprint if shift is held and not crouching
        }
        else if (Input.GetKey(KeyCode.LeftShift) && isCrouching)
        {
            isCrouching = false; // Stand up if shift is held while crouching
            MovementInput *= sprintSpeed;
            if (crouchRoutine != null) StopCoroutine(crouchRoutine); // Stop crouch transition coroutine
            //crouchRoutine = StartCoroutine(SmoothStandUp()); // Start standing up coroutine
        }
        else if (isCrouching)
        {
            MovementInput *= crouchSpeed; // Move slower if crouching
        }
        else
        {
            MovementInput *= speed; // Normal movement speed
        }

        // Calculate movement direction and apply to Rigidbody
        Vector3 MoveDir = (transform.forward * MovementInput.z + transform.right * MovementInput.x) / 2;
        rb.velocity = new Vector3(MoveDir.x, rb.velocity.y, MoveDir.z);

        isGrounded = Physics.Raycast(transform.position, -transform.up, 1.3f); // Check if the player is grounded

        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z); // Apply jump force
        }
    }

    #region Crouching Mechanics

    //private void HandleCrouch()
    //{
    //    Vector3 rayOrigin = transform.position;
    //    Vector3 rayDirection = Vector3.up;

    //    float rayLength = 3f; // Adjust this length if needed

    //    // Perform the raycast to check for obstacles above the player
    //    bool isObstructed = Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hitInfo, rayLength);

    //    // Draw the ray for visualization
    //    Debug.DrawRay(rayOrigin, rayDirection * rayLength, Color.yellow);

    //    if (Input.GetKeyDown(KeyCode.C))
    //    {
    //        if (isCrouching)
    //        {
    //            if (crouchRoutine != null) StopCoroutine(crouchRoutine);
    //            if (!isObstructed) // Only stand up if not obstructed
    //            {
    //                crouchRoutine = StartCoroutine(SmoothStandUp());
    //                HideWarningText(); // Hide warning message
    //            }
    //            else
    //            {
    //                Debug.Log("Cannot stand up, obstruction detected!");
    //                if (warningRoutine != null) StopCoroutine(warningRoutine);
    //                warningRoutine = StartCoroutine(DisplayWarning("Cannot stand up here!")); // Show warning message
    //            }
    //        }
    //        else
    //        {
    //            if (crouchRoutine != null) StopCoroutine(crouchRoutine);
    //            crouchRoutine = StartCoroutine(SmoothCrouch());
    //            HideWarningText(); // Hide warning message when crouching
    //        }
    //    }
    //}

    //private IEnumerator DisplayWarning(string message)
    //{
    //    warningText.text = message;
    //    warningText.gameObject.SetActive(true); // Make sure the text is active

    //    // Fade in
    //    Color textColor = warningText.color;
    //    for (float t = 0; t <= 1; t += Time.deltaTime / 0.5f) // Adjust duration as needed
    //    {
    //        textColor.a = Mathf.Lerp(0, 1, t);
    //        warningText.color = textColor;
    //        yield return null;
    //    }
    //    textColor.a = 1;
    //    warningText.color = textColor;

    //    // Wait for 3 seconds
    //    yield return new WaitForSeconds(3f);

    //    // Fade out
    //    for (float t = 0; t <= 1; t += Time.deltaTime / 0.5f) // Adjust duration as needed
    //    {
    //        textColor.a = Mathf.Lerp(1, 0, t);
    //        warningText.color = textColor;
    //        yield return null;
    //    }
    //    textColor.a = 0;
    //    warningText.color = textColor;

    //    warningText.gameObject.SetActive(false); // Deactivate text after fading out
    //}

    //private void HideWarningText()
    //{
    //    if (warningText != null)
    //    {
    //        warningText.gameObject.SetActive(false); // Hide the text immediately
    //    }
    //}

    //private IEnumerator SmoothCrouch()
    //{
    //    isCrouching = true;
    //    float elapsedTime = 0f;
    //    Vector3 targetCamPos = new Vector3(originalCamPos.x, originalCamPos.y - crouchCamPosOffset, originalCamPos.z);
    //    float targetHeight = crouchHeight;

    //    while (elapsedTime < crouchTransitionSpeed)
    //    {
    //        playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, (elapsedTime / crouchTransitionSpeed));
    //        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetCamPos, (elapsedTime / crouchTransitionSpeed));
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    playerCollider.height = targetHeight;
    //    cam.transform.localPosition = targetCamPos;
    //}

    //private IEnumerator SmoothStandUp()
    //{
    //    isCrouching = false;
    //    float elapsedTime = 0f;
    //    float targetHeight = originalHeight;

    //    while (elapsedTime < crouchTransitionSpeed)
    //    {
    //        playerCollider.height = Mathf.Lerp(playerCollider.height, targetHeight, (elapsedTime / crouchTransitionSpeed));
    //        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, originalCamPos, (elapsedTime / crouchTransitionSpeed));
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    playerCollider.height = targetHeight;
    //    cam.transform.localPosition = originalCamPos;
    //}

    #endregion

    private void leaningMoment()
    {
        // Get lean input from the user
        float leanInput = Input.GetAxis("Lean Axes");

        // Determine target lean rotation based on input
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
            targetLeanRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);
        }

        // Smoothly transition to the target lean rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetLeanRotation, Time.deltaTime * leaningSpeed);
    }

    private void handleFlashlight()
    {
        // Clamp flashlight intensity between min and max brightness
        flashLight.intensity = Mathf.Clamp(flashLight.intensity, minBrightness, maxBrightness);

        // Drain flashlight intensity over time if enabled
        if (drainOverTime && flashLight.enabled)
        {
            if (flashLight.intensity > minBrightness)
            {
                flashLight.intensity -= Time.deltaTime * (drainingRate / 1000);
            }
        }

        // Update battery indicator based on flashlight intensity
        batteryIndicator.fillAmount = flashLight.intensity / maxBrightness;

        // Toggle flashlight on and off when 'F' key is pressed
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (flashLight != null)
            {
                flashLight.enabled = !flashLight.enabled;
            }
        }
    }

    // Replace flashlight battery
    private void replaceBattery(float amount)
    {
        flashLight.intensity += amount; // Increase flashlight intensity
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Shooting == null)
        {
            Debug.LogError("Shooting is null");
            return;
        }

        // Handle different types of pickups
        if (other.gameObject.CompareTag("AmmoClip"))
        {
            Debug.Log("AmmoMagazine triggered");
            Destroy(other.gameObject); // Destroy the ammo clip object
            Shooting.PickupAmmoClip(10); // Add ammo from the clip
        }
        if (other.gameObject.CompareTag("AmmoBox"))
        {
            Debug.Log("AmmoBox triggered");
            Destroy(other.gameObject); // Destroy the ammo box object
            Shooting.PickupAmmoBox(60); // Add ammo from the box
        }
        if (other.gameObject.CompareTag("Battery"))
        {
            Destroy(other.gameObject); // Destroy the battery object
            replaceBattery(10f); // Increase flashlight intensity with battery
        }
        if (other.gameObject.CompareTag("Teleport"))
        {
            if (teleportationIcon != null)
            {
                teleportationIcon.gameObject.SetActive(true); // Show teleportation icon
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide teleportation icon when exiting the trigger zone
        if (other.gameObject.CompareTag("Teleport"))
        {
            teleportationIcon.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision with enemies
        if (collision.gameObject.CompareTag("Enemies"))
        {
            TakingDamage(10); // Apply damage to the player
        }
        if (collision.gameObject.CompareTag("Aid"))
        {
            Destroy(collision.gameObject); // Destroy the aid object
            Healing(10); // Heal the player
        }
        if (collision.gameObject.CompareTag("RedAid"))
        {
            Destroy(collision.gameObject); // Destroy the red aid object
            Healing(30); // Heal the player more
        }
        if (collision.gameObject.CompareTag("KeyCard"))
        {
            Destroy(collision.gameObject); // Destroy the key card object
            Destroy(WallDoor); // Remove the wall door
        }
    }

    // Method to apply damage to the player
    public void TakingDamage(float damage)
    {
        healthAmount -= damage; // Decrease health
        HealthBar.fillAmount = healthAmount / 100; // Update health bar
    }

    // Method to heal the player
    public void Healing(float healingAmount)
    {
        healthAmount += healingAmount; // Increase health
        healthAmount = Mathf.Clamp(healthAmount, 0, 100); // Clamp health to valid range
        HealthBar.fillAmount = healthAmount / 100; // Update health bar
    }
}
