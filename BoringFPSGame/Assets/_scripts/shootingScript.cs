using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingScript : MonoBehaviour
{
    public KeyCode shootKey = KeyCode.Mouse0;
    public Transform shootingPoint;
    public GameObject bullet;
    public float Power;
    [SerializeField] private float bulletTimeAlive = 5f;

    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            GameObject bulletItSelf = Instantiate(bullet, shootingPoint.position, transform.rotation);
            bulletItSelf.GetComponent<Rigidbody>().AddForce(transform.TransformDirection(Vector3.right) * Power, ForceMode.VelocityChange);
            Destroy(bulletItSelf, bulletTimeAlive);
        }
    }
}
