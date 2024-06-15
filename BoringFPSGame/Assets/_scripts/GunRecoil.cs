using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public GameObject Weapon;

    public Transform recoilMod;
    public GameObject weapon;
    public float maxRecoil_z = 20f;
    public float recoilSpeed = 10f;
    public float recoil = 0.0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Every time you fire a bullet, add to the recoil. You can set a max recoil if needed.
            recoil += 0.1f;
        }

        Recoiling();
    }

    void Recoiling()
    {
        if (recoil > 0)
        {
            Quaternion maxRecoil = Quaternion.Euler(0, 0, maxRecoil_z);
            // Dampen towards the target rotation
            recoilMod.localRotation = Quaternion.Slerp(recoilMod.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            weapon.transform.localEulerAngles = new Vector3(weapon.transform.localEulerAngles.x, weapon.transform.localEulerAngles.y, recoilMod.localEulerAngles.z);
            recoil -= Time.deltaTime;
        }
        else
        {
            recoil = 0;
            Quaternion minRecoil = Quaternion.Euler(0, 0, 0);
            // Dampen towards the target rotation
            recoilMod.localRotation = Quaternion.Slerp(recoilMod.localRotation, minRecoil, Time.deltaTime * recoilSpeed / 2);
            weapon.transform.localEulerAngles = new Vector3(weapon.transform.localEulerAngles.x, weapon.transform.localEulerAngles.y, recoilMod.localEulerAngles.z);
        }
    }

}
