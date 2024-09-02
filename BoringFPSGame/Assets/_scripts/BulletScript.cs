using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float bulletCollisionTime = 0.01f;
    [SerializeField] private int bulletDamage = 1; // Added variable for bullet damage

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemies"))
        {
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage);
            }
        }

        Destroy(gameObject, bulletCollisionTime);
    }
}
