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
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode ADSKey = KeyCode.Mouse1;

    [Header("Aiming Mechanics")]
    public float aimingSpeed = 12f;
    public Transform gunItSelf;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;

    [SerializeField] private bool isAiming = false;
    [SerializeField] private Image crosshairImage;
    //[SerializeField] private Image ADSCrosshairImage; //probably use it for later on

    [Header("ADS FOV")]
    [SerializeField] private float StartFov;
    [SerializeField] private float ADSFov;

    public float AimFOV { get; private set; }

    [Header("Shooting Mechanics")]
    public Transform shootingPoint;
    public GameObject bullet;
    public float Power;
    [SerializeField] private float bulletTimeAlive = 4f;

    [Header("Ammo Counting")]
    public int MaxAmmo = 8;
    public int CurrentAmmo;
    public int TotalAmmo = 160;
    public float ReloadingTime = 2.5f;
    private bool isReloading = false;

    public TextMeshProUGUI ammoText;

    [Header("SFX")]
    public AudioClip SFX;
    private AudioSource shootingSFX;

    public AudioClip ReloadSFX;
    private AudioSource reloadingSFX;

    [Header("Recoil")]
    public GameObject weapon;
    public float recoil = 0.0f;
    public float maxRecoil = 5f;
    public float aimingRecoil = 0.0f;
    public float MaxAimingRecoil = 3f;
    Vector3 currentRecoil;
    Vector3 currentAimingRecoil;

    [SerializeField] private bool ADSRecoil = false;

    //when hipfire
    public float maxRecoil_z = 20f;
    public float recoilSpeed = 10f;

    private void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").transform;

        gunItSelf.position = originalWeaponPosition.position;
        gunItSelf.rotation = originalWeaponPosition.rotation;

        if (SFX == null)
        {
            Debug.Log("You haven't assigned the SFX through the inspector!");
            this.enabled = false;
        }

        shootingSFX = gameObject.AddComponent<AudioSource>();
        shootingSFX.playOnAwake = false;
        shootingSFX.clip = SFX;
        shootingSFX.Stop();

        reloadingSFX = gameObject.AddComponent<AudioSource>();
        reloadingSFX.playOnAwake = false;
        reloadingSFX.clip = ReloadSFX;
        reloadingSFX.Stop();

        CurrentAmmo = MaxAmmo;
    }

    private void Update()
    {
        if (isReloading)
            return;

        if (Input.GetKeyDown(KeyCode.R) && CurrentAmmo < MaxAmmo || CurrentAmmo == 0)
        {
            StartCoroutine(Reloading());
            reloadingSFX.Play();
            return;
        }

        AimingMoment();
        recoiling();

        if (Input.GetKeyDown(shootKey))
        {
            if (CurrentAmmo > 0) // Ensure shooting happens only if there's ammo
            {
                shootingSFX.Play();
                ShootingMoment();
            }
        }

        if (isAiming)
        {
            AimingRecoil();
        }
    }

    private void AimingMoment()
    {
        if (Input.GetKeyDown(ADSKey))
        {
            isAiming = true;
        }
        else if (Input.GetKeyUp(ADSKey))
        {
            isAiming = false;
        }

        if (isAiming)
        {
            gunItSelf.position = Vector3.Lerp(gunItSelf.position, aimingPosition.position, Time.deltaTime * aimingSpeed);
            gunItSelf.rotation = Quaternion.Lerp(gunItSelf.rotation, aimingPosition.rotation, Time.deltaTime * aimingSpeed);
            crosshairImage.gameObject.SetActive(false);
        }
        else
        {
            gunItSelf.position = Vector3.Lerp(gunItSelf.position, originalWeaponPosition.position, Time.deltaTime * aimingSpeed);
            gunItSelf.rotation = Quaternion.Lerp(gunItSelf.rotation, originalWeaponPosition.rotation, Time.deltaTime * aimingSpeed);
            crosshairImage.gameObject.SetActive(true);
        }
    }

    private void ShootingMoment()
    {
        GameObject bulletItSelf = Instantiate(bullet, shootingPoint.position, transform.rotation);
        bulletItSelf.GetComponent<Rigidbody>().AddForce(shootingPoint.forward * Power, ForceMode.VelocityChange);
        Destroy(bulletItSelf, bulletTimeAlive);

        currentRecoil = currentRecoil + new Vector3(Random.Range(-recoil, recoil), Random.Range(-recoil, recoil), 0);

        currentAimingRecoil = currentAimingRecoil + new Vector3(Random.Range(-aimingRecoil, -MaxAimingRecoil), Random.Range(-aimingRecoil, MaxAimingRecoil), 0);

        if (CurrentAmmo > 0)
        {
            CurrentAmmo--;
            UpdateAmmoGUI();
        }
    }

    private void recoiling()
    {
        transform.localEulerAngles = currentRecoil;
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, Time.deltaTime * 4);
    }

    private void AimingRecoil()
    {
        transform.localEulerAngles = currentAimingRecoil;
        currentAimingRecoil = Vector3.Lerp(currentAimingRecoil, Vector3.zero, Time.deltaTime * 4);
    }

    private System.Collections.IEnumerator Reloading()
    {
        isAiming = false;

        if (TotalAmmo <= 0)
        {
            yield break;
        }

        isReloading = true;

        yield return new WaitForSeconds(ReloadingTime);

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

    private void UpdateAmmoGUI()
    {
        ammoText.text = $"{CurrentAmmo} / {TotalAmmo}";
    }

}