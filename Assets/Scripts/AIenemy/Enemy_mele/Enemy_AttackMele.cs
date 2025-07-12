using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackMele : MonoBehaviour
{
    [Header("Normal Goblin")]
    [SerializeField] private bool mele = false;
    [SerializeField] private float attackCooldown;

    [Header("Giant Goblin")]
    [SerializeField] private bool giant = false;
    [SerializeField] private float throwCooldown;
    [SerializeField] private float throwDistance;
    [SerializeField] private GameObject boulderPrefab;
    [SerializeField] public float stunTime;
    [HideInInspector] private bool canStunned = false;

    [Header("Normal Wolf")]
    [SerializeField] private bool wolf = false;
    [SerializeField] private float wolfAttackCooldown;
    [SerializeField] private float wolfAttackDashChargingTime;
    [SerializeField] private float wolfAttackDashTime;
    [SerializeField] private float wolfAttackDashSpeed;

    private float attackTimer = 0f;
    private float throwTimer = 0f;
    private Vector2 targetPosition;

    private Animator animator;
    private Sword sword;
    private Collider2D swordCollider;
    private EnemyAIController_Mele melController;
    private Rigidbody2D rb;

    void Start()
    {
        melController = GetComponent<EnemyAIController_Mele>();
        animator = GetComponent<Animator>();
        if (!wolf)
        {
            sword = GetComponentInChildren<Sword>();
            swordCollider = sword.GetComponent<Collider2D>();
            sword.isGiant = giant;  
        }
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (throwTimer > 0)
        {
            throwTimer -= Time.deltaTime;
        }

    }

    public void Attack()
    {
        if (mele && attackTimer <= 0 && melController.distanceToPlayer <= melController.stopDistance)
        {
            melController.StopAllCoroutines();
            melController.agent.isStopped = true;
            melController.isAsttack = true;
            targetPosition = melController.targetpos;
            animator.SetBool("isAttacking", true);
            attackTimer = attackCooldown;
        }

        if (giant && throwTimer <= 0 && melController.distanceToPlayer >= throwDistance)
        {
            melController.StopAllCoroutines();
            melController.rb.velocity = Vector2.zero; // Stop the enemy's movement
            melController.agent.isStopped = true;
            melController.isAsttack = true;
            animator.SetBool("isThrowing", true);
            throwTimer = throwCooldown;
        }

        if (wolf && attackTimer <= 0 && melController.distanceToPlayer <= melController.stopDistance)
        {
            melController.StopAllCoroutines();
            melController.agent.isStopped = true;
            melController.isAsttack = true;
            attackTimer = wolfAttackCooldown;
            // Perform a dash towards the player
            
            StartCoroutine(WolfAttackDash());
        }
    }

    private IEnumerator WolfAttackDash()
    {
        melController.agent.isStopped = true;
        Debug.Log("Wolf Attack Dash Started");
        melController.StopAllCoroutines();
        melController.agent.isStopped = true;
        yield return new WaitForSeconds(wolfAttackDashChargingTime);
        targetPosition =(Vector2)melController.target.position;
        Vector2 direction = (targetPosition - (Vector2)melController.transform.position).normalized;
        rb.velocity = direction * wolfAttackDashSpeed;
        yield return new WaitForSeconds(wolfAttackDashTime);
        rb.velocity = Vector2.zero; // Stop the dash movement
        melController.agent.isStopped = false; // Resume movement
        melController.isAsttack = false; // Reset attack state
        melController.target = null;
        StartCoroutine(melController.Chasing()); // Resume chasing state
        Debug.Log("Wolf Attack Dash Ended");
    }

    public void DrawSword()
    {
        if (sword == null) return; // Ensure sword is not null
        if (swordCollider.enabled == true)
        {
            swordCollider.enabled = false; // Disable the sword collider
        }
        else
        {
            swordCollider.enabled = true; // Enable the sword collider
        }
    }

    public void AttackStep()
    {
        Vector2 direction = (melController.targetpos - (Vector2)melController.transform.position).normalized;
        rb.velocity = direction * melController.speed;
        if (swordCollider.enabled==false)swordCollider.enabled = true; // Enable the sword collider during the attack step
    }

    public void EndAttack()
    {
        melController.agent.isStopped = false;
        melController.isAsttack = false;
        melController.state = EnemyAIController_Mele.State.Chasing;
        animator.SetBool("isAttacking", false);
        StartCoroutine(melController.Chasing());
    }


    public void Stun()
    {
        canStunned = !canStunned;
        sword.Stune = canStunned;
    }

    public void Throw()
    {
        animator.SetBool("isThrowing", false);
        Instantiate(boulderPrefab, melController.targetpos, Quaternion.identity);
        melController.agent.isStopped = false;
        melController.isAsttack = false;
        melController.state = EnemyAIController_Mele.State.Chasing;
        StartCoroutine(melController.Chasing());
    }
}
