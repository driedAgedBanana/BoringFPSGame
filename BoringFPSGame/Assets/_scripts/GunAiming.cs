using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAiming : MonoBehaviour
{
    [Header("References")]
    public Transform Gun;
    public Transform camera;
    public Transform aimingPosition;
    public Transform originalWeaponPosition;

    [Header("Aiming")]
    public KeyCode aimKey = KeyCode.Mouse1;
    public float aimingSpeed = 5;

    public Vector3 originalGunPosition;
    public Quaternion originalGunRotation;
    [SerializeField] private bool isAming = false;

    // Start is called before the first frame update
    void Start()
    {
        //originalGunPosition = Gun.localPosition;
        originalGunRotation = Gun.localRotation;

        originalGunRotation = Gun.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAiming();
    }

    private void HandleAiming()
    {
        if (Input.GetKeyDown(aimKey))
        {
            isAming = true;
        }
        else if (Input.GetKeyUp(aimKey))
        {
            isAming = false;
        }

        if (isAming)
        {
            Gun.position = Vector3.Lerp(Gun.position, aimingPosition.position, Time.deltaTime * aimingSpeed);
            Gun.rotation = Quaternion.Lerp(Gun.rotation, aimingPosition.rotation, Time.deltaTime * aimingSpeed);
        }
        else
        {
            Gun.position = Vector3.Lerp(Gun.position, originalGunPosition, Time.deltaTime * aimingSpeed);
            Gun.rotation = Quaternion.Lerp(Gun.rotation, originalGunRotation, Time.deltaTime * aimingSpeed);
        }
    }
}
