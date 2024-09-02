using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManagerScript : MonoBehaviour
{
    public Image HealthBar;
    public float healthAmount = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    TakingDamage(0);
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    Healing(0);
        //}
    }

    public void TakingDamage(float damage)
    {
        healthAmount -= damage;
        HealthBar.fillAmount = healthAmount / 100;
    }

    public void Healing(float healingAmount)
    {
        healthAmount += healingAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, 100);

        HealthBar.fillAmount = healthAmount / 100;
    }
}
