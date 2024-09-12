using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class shootingScript : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Camera cam;

    [Header("Inputs")]
    public KeyCode shootKey = KeyCode.Mouse0;  // Key to shoot
    public KeyCode ADSKey = KeyCode.Mouse1;   // Key to aim down sights (ADS)

    [Header("Aiming Mechanics")]
    public float aimingSpeed = 12f;  // Speed of transitioning to aiming position
    public Transform gunItSelf;      // Transform of the gun
    public Transform aimingPosition; // Transform of the aiming position
    public Transform originalWeaponPosition; // Transform of the original weapon position

    [SerializeField] private bool isAiming = false; // Indicates if the player is aiming
    [SerializeField] private Image crosshairImage;  // Crosshair image to show/hide

    [Header("ADS FOV")]
    [SerializeField] private float StartFov;  // Field of view when not aiming
    [SerializeField] private float ADSFov;    // Field of view when aiming

    public float AimFOV { get; private set; } // Current aiming FOV

    [Header("Shooting Mechanics")]
    public Transform shootingPoint; // Point from which bullets are fired
    public GameObject bullet;       // Bullet prefab
    public float Power;            // Bullet force
    [SerializeField] private float bulletTimeAlive = 4f; // Lifetime of the bullet

    [Header("Ammo Counting")]
    public int MaxAmmo = 8;          // Maximum ammo in the weapon
    public int CurrentAmmo;         // Current ammo in the weapon
    public int TotalAmmo = 160;     // Total ammo available
    public float ReloadingTime = 2.5f; // Time required to reload
    private bool isReloading = false; // Indicates if the weapon is reloading

    public TextMeshProUGUI ammoText; // UI text to display ammo count

    public GameObject ammoClip; // Ammo clip prefab (for future use)
    public GameObject ammoBox;  // Ammo box prefab (for future use)

    [Header("SFX")]
    public AudioClip SFX; // Shooting sound effect
    private AudioSource shootingSFX; // AudioSource for shooting sound

    public AudioClip ReloadSFX; // Reload sound effect
    private AudioSource reloadingSFX; // AudioSource for reloading sound

    [Header("Recoil")]
    public GameObject weapon; // Weapon object to apply recoil to
    public float recoil = 0.0f; // Base recoil amount
    public float maxRecoil = 5f; // Maximum recoil amount
    public float aimingRecoil = 0.0f; // Recoil amount while aiming
    public float MaxAimingRecoil = 3f; // Maximum aiming recoil amount
    Vector3 currentRecoil; // Current recoil value
    Vector3 currentAimingRecoil; // Current aiming recoil value

    [SerializeField] private bool ADSRecoil = false; // Flag to handle ADS recoil

    // Recoil values when hip-firing
    public float maxRecoil_z = 20f;
    public float recoilSpeed = 10f;

    private void Start()
    {
        // Initialize main camera and weapon positions
        mainCamera = GameObject.FindWithTag("MainCamera").transform;
        gunItSelf.position = originalWeaponPosition.position;
        gunItSelf.rotation = originalWeaponPosition.rotation;

        // Check if SFX is assigned
        if (SFX == null)
        {
            Debug.Log("You haven't assigned the SFX through the inspector!");
            this.enabled = false; // Disable the script if SFX is not assigned
        }

        // Set up audio sources
        shootingSFX = gameObject.AddComponent<AudioSource>();
        shootingSFX.playOnAwake = false;
        shootingSFX.clip = SFX;
        shootingSFX.Stop();

        reloadingSFX = gameObject.AddComponent<AudioSource>();
        reloadingSFX.playOnAwake = false;
        reloadingSFX.clip = ReloadSFX;
        reloadingSFX.Stop();

        // Initialize ammo counts
        CurrentAmmo = MaxAmmo;
    }

    private void Update()
    {
        // Skip updating if reloading
        if (isReloading)
            return;

        // Handle reloading
        if (Input.GetKeyDown(KeyCode.R) && CurrentAmmo < MaxAmmo || CurrentAmmo == 0)
        {
            StartCoroutine(Reloading());
            reloadingSFX.Play();
            return;
        }

        // Handle aiming and recoil
        AimingMoment();
        recoiling();

        // Handle shooting
        if (Input.GetKeyDown(shootKey))
        {
            if (CurrentAmmo > 0) // Ensure shooting happens only if there's ammo
            {
                shootingSFX.Play();
                ShootingMoment();
            }
        }

        // Apply aiming recoil if aiming
        if (isAiming)
        {
            AimingRecoil();
        }
    }

    private void AimingMoment()
    {
        // Toggle aiming state
        if (Input.GetKeyDown(ADSKey))
        {
            isAiming = true;
        }
        else if (Input.GetKeyUp(ADSKey))
        {
            isAiming = false;
        }

        // Smoothly transition gun position and rotation
        if (isAiming)
        {
            gunItSelf.position = Vector3.Lerp(gunItSelf.position, aimingPosition.position, Time.deltaTime * aimingSpeed);
            gunItSelf.rotation = Quaternion.Lerp(gunItSelf.rotation, aimingPosition.rotation, Time.deltaTime * aimingSpeed);
            crosshairImage.gameObject.SetActive(false); // Hide crosshair while aiming
        }
        else
        {
            gunItSelf.position = Vector3.Lerp(gunItSelf.position, originalWeaponPosition.position, Time.deltaTime * aimingSpeed);
            gunItSelf.rotation = Quaternion.Lerp(gunItSelf.rotation, originalWeaponPosition.rotation, Time.deltaTime * aimingSpeed);
            crosshairImage.gameObject.SetActive(true); // Show crosshair while not aiming
        }
    }

    private void ShootingMoment()
    {
        // Instantiate and shoot the bullet
        GameObject bulletItSelf = Instantiate(bullet, shootingPoint.position, transform.rotation);
        bulletItSelf.GetComponent<Rigidbody>().AddForce(shootingPoint.forward * Power, ForceMode.VelocityChange);
        Destroy(bulletItSelf, bulletTimeAlive);

        // Apply recoil
        currentRecoil = currentRecoil + new Vector3(Random.Range(-recoil, recoil), Random.Range(-recoil, recoil), 0);
        currentAimingRecoil = currentAimingRecoil + new Vector3(Random.Range(-aimingRecoil, -MaxAimingRecoil), Random.Range(-aimingRecoil, MaxAimingRecoil), 0);

        // Decrease ammo count and update UI
        if (CurrentAmmo > 0)
        {
            CurrentAmmo--;
            UpdateAmmoGUI();
        }
    }

    private void recoiling()
    {
        // Apply regular recoil
        transform.localEulerAngles = currentRecoil;
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, Time.deltaTime * 4);
    }

    private void AimingRecoil()
    {
        // Apply aiming recoil
        transform.localEulerAngles = currentAimingRecoil;
        currentAimingRecoil = Vector3.Lerp(currentAimingRecoil, Vector3.zero, Time.deltaTime * 4);
    }

    private System.Collections.IEnumerator Reloading()
    {
        // Handle reloading process
        isAiming = false;

        if (TotalAmmo <= 0)
        {
            yield break;
        }

        isReloading = true;

        yield return new WaitForSeconds(ReloadingTime);

        // Reload ammo
        int AmmoToReload = MaxAmmo - CurrentAmmo;
        if (TotalAmmo >= AmmoToReload)
        {
            CurrentAmmo += AmmoToReload;
            TotalAmmo -= AmmoToReload;
        }
        else
        {
            CurrentAmmo += TotalAmmo;
            TotalAmmo = 0;
        }

        isReloading = false;
        UpdateAmmoGUI();
    }

    public void UpdateAmmoGUI()
    {
        // Update the ammo count UI
        ammoText.text = $"{CurrentAmmo} / {TotalAmmo}";
    }

    public void PickupAmmoClip(int ammoAmount)
    {
        // Pickup ammo clip and update total ammo
        if (TotalAmmo < 160)
        {
            TotalAmmo = Mathf.Min(TotalAmmo + ammoAmount, 160);
            UpdateAmmoGUI();
        }
    }

    public void PickupAmmoBox(int boxAmount)
    {
        // Pickup ammo box and update total ammo
        int amountToAdd = Mathf.Min(boxAmount, 160 - TotalAmmo);
        TotalAmmo += amountToAdd;
        UpdateAmmoGUI();
    }
}
