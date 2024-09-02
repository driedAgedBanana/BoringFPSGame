using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemySpawner;

    [SerializeField] private float interval = 3f;
    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > interval)
        {
            timer = 0f;
            Instantiate(enemySpawner, transform.position, transform.rotation);
        }
    }
}
