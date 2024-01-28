using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    int currentHealth;
    public HealthBar healthBar;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);    
    }
    
    public void TakeDamage(int damage)
    {

        if(currentHealth <= 0)
        {
            Die();
            return;
        }
        currentHealth -= damage;

        Debug.Log("Player1 Heatlh " + currentHealth);
        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        Debug.Log("enemy died!");
    }

    public bool HasHealth()
    {
        return currentHealth > 0;
    }

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Space))
    //     {
    //         TakeDamage(20);
    //     }
    // }

}
