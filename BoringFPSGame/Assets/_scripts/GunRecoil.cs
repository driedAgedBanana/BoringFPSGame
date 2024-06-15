using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    public GameObject Weapon;

    public float maxRecoil_x = -20.0f;
    public float maxRecoil_y = -10.0f;

    public float maxTransform_x = 1.0f;
    public float maxTransform_z = -1.0f;

    public float recoilSpeed = 10.0f;
    public float recoilItSelf = 0.0f;

    private void Update()
    {
        if (recoilItSelf > 0)
        {
            Quaternion maxRecoil = Quaternion.Euler(Random.Range(transform.localRotation.x, maxRecoil_x), 
                Random.Range(transform.localRotation.y, maxRecoil_y), 
                transform.localRotation.z);

            //dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);

            Vector3 maximumTranslation = new Vector3(Random.Range(transform.localPosition.x, maxTransform_x),
                transform.localPosition.y,
                Random.Range(transform.localPosition.z, maxTransform_z));

            transform.localPosition = Vector3.Slerp(transform.localPosition, maximumTranslation, Time.deltaTime * recoilSpeed);

            recoilItSelf -= Time.deltaTime;
        }
    }
}
