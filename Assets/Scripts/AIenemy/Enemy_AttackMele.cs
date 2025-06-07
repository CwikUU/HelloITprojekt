using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackMele : MonoBehaviour
{
    private float attackCooldown;

    private Animator animator;
    private Sword sword;
    private Collider2D swordCollider;
    private EnemyAIController_Mele melController;
    private Rigidbody2D rb;

    void Start()
    {
        melController = GetComponent<EnemyAIController_Mele>();
        animator = GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();
        swordCollider = sword.GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        else
        {
            melController.attackCDtimer = attackCooldown; // Update the attack cooldown timer in Proba
        }

    }

    public void Attack()
    {
        animator.SetBool("isAttacking", true);
        attackCooldown = melController.attackCD;
        melController.attackCDtimer = attackCooldown;
    }

    public void DrawSword()
    {
        if (swordCollider.enabled == true)
        {
            swordCollider.enabled = false; // Disable the sword collider
            animator.SetBool("isAttacking", false);
            melController.state = EnemyAIController_Mele.State.Chasing;
        }
        else
        {
            swordCollider.enabled = true; // Enable the sword collider
        }
    }

    public void AttackStep()
    {
        //Vector2 direction = (melController.targetpos - (Vector2)melController.transform.position).normalized;
        //rb.velocity = direction * 1; // Move towards the target during the attack
        //Debug.Log(melController.state);
    }
}
