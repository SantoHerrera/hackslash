using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Combat : MonoBehaviour
{

    public Animator animator;

    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 20;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Attack();
        }
        
    }

    void Attack()
    {
        //play an attack annimation
        animator.SetTrigger("Attack");
        //detect enemies in range of attack

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Player1Health>().TakeDamage(attackDamage);
            Debug.Log("Attcking player1");
            enemy.GetComponent<Player1Health>().HasHealth();

            Debug.Log("works " + enemy.GetComponent<Player1Health>().HasHealth());

            
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


}
