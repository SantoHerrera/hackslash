using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using HealthBar;

public class Player2Health : MonoBehaviour
{

    public int maxHealth = 100;
    int currentHealth;
    public HealthBar healthBar;
    public Animator animators;
    // HealthBar player1 = new HealthBar();

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;    
        healthBar.SetMaxHealth(maxHealth);    
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
            animators.SetTrigger("hasDiedTrigger");
            // return;

        }
        

        Debug.Log("Player2 Heatlh " + currentHealth);
        healthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        //die animation
        Debug.Log("enemy died!");
    }

    public bool HasHealth()
    {
        return currentHealth > 0;
    }
    
}
