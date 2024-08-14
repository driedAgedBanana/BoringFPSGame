using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float bulletCollisionTime = 0.01f;
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject, bulletCollisionTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject, bulletCollisionTime);
    }
}
