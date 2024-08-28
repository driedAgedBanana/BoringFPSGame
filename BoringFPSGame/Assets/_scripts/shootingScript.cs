using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [Header("SFX")]
    public AudioClip SFX;
    private AudioSource shootingSFX;

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
        //gunRecoil = mainCamera.GetComponent<GunRecoil>();

        gunItSelf.position = originalWeaponPosition.position;
        gunItSelf.rotation = originalWeaponPosition.rotation;

        if (SFX == null)
        {
            Debug.Log("You haven't assigned the SFX through the inspector!");
            this.enabled = false;
        }

        shootingSFX = GetComponent<AudioSource>();
        shootingSFX.playOnAwake = false;
        shootingSFX.clip = SFX;
        shootingSFX.Stop();
    }

    private void Update()
    {
        AimingMoment();
        recoiling();

        if (Input.GetKeyDown(shootKey))
        {
            shootingSFX.Play();
            ShootingMoment();
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

}
