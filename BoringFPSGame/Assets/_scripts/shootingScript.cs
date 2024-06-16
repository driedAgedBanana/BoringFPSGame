using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class shootingScript : MonoBehaviour
{
    [Header("Inputs")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode ADSKey = KeyCode.Mouse1;

    [Header("Aiming Mechanics")]
    public float aimingSpeed = 12f;
    public Transform gunItSelf;
    public Transform cam;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;

    [SerializeField] private bool isAiming = false;
    [SerializeField] private Image crosshairImage;
    //[SerializeField] private Image ADSCrosshairImage; //probably use it for later on

    [Header("Shooting Mechanics")]
    public Transform shootingPoint;
    public GameObject bullet;
    public float Power;
    [SerializeField] private float bulletTimeAlive = 5f;

    [SerializeField] private Transform mainCamera;

    [Header("Recoil")]
    public Transform recoilSlot;
    public GameObject weapon;
    public float recoil = 0.0f;

    [SerializeField] private bool ADSRecoil = false;

    //when hipfire
    public float maxRecoil_z = 20f;
    public float recoilSpeed = 10f;
    //when aiming
    public float ADSMaxRecoil_z = 15f;
    public float ADSRecoilSpeed = 20f;

    private void Start()
    {

        mainCamera = GameObject.FindWithTag("MainCamera").transform;
        //gunRecoil = mainCamera.GetComponent<GunRecoil>();

        gunItSelf.position = originalWeaponPosition.position;
        gunItSelf.rotation = originalWeaponPosition.rotation;
    }

    private void Update()
    {
        AimingMoment();
        Recoiling();

        if (Input.GetKeyDown(shootKey))
        {
            RaycastHit hit;
            if(Physics.Raycast(mainCamera.position, mainCamera.forward, out hit))
            {
                if (Vector3.Distance(mainCamera.position, hit.point) >1)
                {
                    shootingPoint.LookAt(hit.point);
                }
            }
            else
            {
                shootingPoint.LookAt(mainCamera.position + (mainCamera.forward));
            }
            ShootingMoment();
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
        bulletItSelf.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.right) * Power, ForceMode.VelocityChange);
        recoil += 0.1f;
        Destroy(bulletItSelf, bulletTimeAlive);
    }

    private void Recoiling()
    {
        if (recoil > 0)
        {
            Quaternion maxRecoil = Quaternion.Euler(0, 0, maxRecoil_z);
            // Dampen towards the target rotation
            recoilSlot.localRotation = Quaternion.Slerp(recoilSlot.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            weapon.transform.localEulerAngles = new Vector3(weapon.transform.localEulerAngles.x, weapon.transform.localEulerAngles.y, recoilSlot.localEulerAngles.z);
            recoil -= Time.deltaTime;

            if (isAiming)
            {
                Quaternion ADSmaxRecoil = Quaternion.Euler(0, 0, ADSMaxRecoil_z);
                // Dampen towards the target rotation
                recoilSlot.localRotation = Quaternion.Slerp(recoilSlot.localRotation, ADSmaxRecoil, Time.deltaTime * ADSRecoilSpeed);
                weapon.transform.localEulerAngles = new Vector3(weapon.transform.localEulerAngles.x, weapon.transform.localEulerAngles.y, recoilSlot.localEulerAngles.z);
                recoil -= Time.deltaTime;
                ADSRecoil = true;
            }
        }
        else
        {
            recoil = 0;
            Quaternion minRecoil = Quaternion.Euler(0, 0, 0);
            // Dampen towards the target rotation
            recoilSlot.localRotation = Quaternion.Slerp(recoilSlot.localRotation, minRecoil, Time.deltaTime * recoilSpeed / 2);
            weapon.transform.localEulerAngles = new Vector3(weapon.transform.localEulerAngles.x, weapon.transform.localEulerAngles.y, recoilSlot.localEulerAngles.z);

            if (isAiming)
            {
                recoil = 0;
                Quaternion ADSminRecoil = Quaternion.Euler(0, 0, 0);
                // Dampen towards the target rotation
                recoilSlot.localRotation = Quaternion.Slerp(recoilSlot.localRotation, ADSminRecoil, Time.deltaTime * ADSRecoilSpeed / 2);
                weapon.transform.localEulerAngles = new Vector3(weapon.transform.localEulerAngles.x, weapon.transform.localEulerAngles.y, recoilSlot.localEulerAngles.z);
                ADSRecoil = false;
            }
        }
    }
}
