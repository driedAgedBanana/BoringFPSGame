using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunAiming : MonoBehaviour
{
    public Transform gunItSelf;
    public Transform cam;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;

    public KeyCode ADSKey = KeyCode.Mouse1;
    public float aimingSpeed = 2f;

    [SerializeField] private bool isAiming = false;
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Image ADSCrosshairImage; //probably use it for later on

    private void Start()
    {
        gunItSelf.position = originalWeaponPosition.position;
        gunItSelf.rotation = originalWeaponPosition.rotation;
    }

    private void Update()
    {
        AimingMoment();
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
}
