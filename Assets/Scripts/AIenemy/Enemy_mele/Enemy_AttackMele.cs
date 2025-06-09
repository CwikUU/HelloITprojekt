using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackMele : MonoBehaviour
{
    [SerializeField] private bool mele = false;
    [SerializeField] private float attackCooldown;
    [SerializeField] private bool giant = false;
    [SerializeField] private float throwCooldown;
    [SerializeField] private float throwDistance;
    [SerializeField] public float stunTime;
    [HideInInspector] private bool canStunned = false;

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
        sword = GetComponentInChildren<Sword>();
        swordCollider = sword.GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        sword.isGiant = giant;
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
            melController.agent.isStopped = true;
            melController.isAsttack = true;
            targetPosition = melController.targetpos;
            animator.SetBool("isAttacking", true);
            attackTimer = attackCooldown;
        }

        if (giant && throwTimer <= 0 && melController.distanceToPlayer <= throwDistance)
        {
            animator.SetBool("isThrowing", true);
            throwTimer = throwCooldown;
        }
    }

    public void DrawSword()
    {
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
}
