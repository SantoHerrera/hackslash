using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//tricky tricky
public class Player1Combat : MonoBehaviour
{

    public Animator animator;

    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 20;

   void Start()
    {
        //Get the Animator, which you attach to the GameObject you intend to animate.
        animator = gameObject.GetComponent<Animator>();
        //The current speed of the first Animator state
        // m_CurrentSpeed = m_Animator.GetCurrentAnimatorStateInfo(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))   
        {
            // Debug.Log(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")));
            if(Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("before: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
                Attack();
                Debug.Log("after: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));  
                // //to avoid punching spam
                // if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                // {
                //     return;
                // }
                // Attack();
                // Debug.Log("anim state: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
            }
        }
    }

    void Attack()
    {

         //to avoid punching spam
            // if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            // {
            //     return;
            // }
            // else {
            //     animator.SetTrigger("Attack");
            // }
            // Attack();
        //play an attack annimation
        animator.SetTrigger("Attack");
        //detect enemies in range of attack

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        //damage them
        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Player2Health>().TakeDamage(attackDamage);
            Debug.Log("Attcking player2");
            // Debug.Log(enemy.GetComponent<Player2Health>().currentHealth);
        }
    }

    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
