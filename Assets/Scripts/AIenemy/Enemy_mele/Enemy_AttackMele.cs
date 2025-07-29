using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
    [SerializeField] public bool wolf = false;
    [SerializeField] private bool dashAttack = false;
    [SerializeField] private float attackCooldownWolf;
    [SerializeField] private float attackDashChargingTimeWolf;
    [SerializeField] private float attackDashSpeedWolf;
    public Transform point;

    private float attackTimer = 0f;
    private float throwTimer = 0f;
    private Transform targetPosition;
    private Vector2 behindPoint;
    private Vector2 dashPosition;
    private Vector2 targetPos;
    private List<Vector2> dashPoints = new List<Vector2>();
    private int totalDashPoints = 25;
    private int dashPointIndex = 0;

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

    private void FixedUpdate()
    {

    }
    public void Attack()
    {
        if (mele && attackTimer <= 0 && melController.distanceToPlayer <= melController.stopDistance)
        {
            melController.StopAllCoroutines();
            melController.agent.isStopped = true;
            melController.isAsttack = true;
            targetPos = melController.targetpos;
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
            attackTimer = attackCooldownWolf;
            melController.agent.stoppingDistance = 0f; // Disable stopping distance to allow dash attack
            // Perform a dash towards the player

            StartCoroutine(WolfAttackDash());
        }
    }

    private IEnumerator WolfAttackDash()
    {
        float speedWolfDef = melController.agent.speed;
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();

        melController.end = true;

        Debug.Log("Wolf Attack Dash Started");
        yield return new WaitForSeconds(attackDashChargingTimeWolf); 

        melController.isAsttack = true;
        
        DashLine(melController.transform.position, point.position);
        
        while (Vector2.Distance(melController.transform.position, dashPoints[dashPointIndex]) >= 0.2f)
        {
            Debug.Log(Vector2.Distance(melController.transform.position, dashPoints[dashPointIndex]));
            yield return null; // Wait until the enemy reaches the dash point
        }
        rb.velocity = Vector2.zero; // Stop the dash movement
        melController.isAsttack = false; // Reset attack state
        melController.target = null;
        melController.end = false;
        melController.agent.stoppingDistance = melController.stopDistance; // Reset stopping distance
        StartCoroutine(melController.Chasing()); // Resume chasing state
        melController.agent.speed = speedWolfDef; // Reset the agent speed
        //stopwatch.Stop();
        Debug.Log("Wolf Attack Dash Ended");
    }

    private void DashLine(Vector2 start,Vector2 end)
    {
        dashPoints.Clear();
        bool canDash = false;

        

        for (int i = 0; i < totalDashPoints; i++)
        {
            float t = (float)i / (totalDashPoints - 1);
            Vector2 point = Vector2.Lerp(start, end, t);
            dashPoints.Add(point);
        }

        if (dashPoints.Count == 0)
        {
            dashPointIndex = 0; // Reset the dash point index if the list is empty
            return; // Exit if there are no dash points
        }

        for (int i = 0; i < dashPoints.Count; i++)
        {
            Collider2D hit = Physics2D.OverlapCircle(dashPoints[i], 1f, LayerMask.GetMask("Collision"));
            canDash = (hit == null);

            if (!canDash)
            {
                melController.agent.isStopped = false; // Stop the agent if a collision is 
                melController.agent.speed = attackDashSpeedWolf;
                int safeIndex = Mathf.Max(0 , i - 1); // Ensure we don't go out of bounds
                melController.agent.SetDestination(dashPoints[safeIndex]);
                Debug.DrawLine(dashPoints[0], dashPoints[safeIndex],Color.red,2f);
                dashPointIndex = safeIndex;
                break;
            }else if (i == dashPoints.Count - 1)
            {
                melController.agent.isStopped = false;
                melController.agent.speed = attackDashSpeedWolf;
                melController.agent.SetDestination(dashPoints[i]);
                Debug.DrawLine(dashPoints[0], dashPoints[i], Color.red, 2f);
                dashPointIndex = i;
            }
            //if (i < dashPoints.Count - 1)
            //{
            //    Debug.DrawLine(dashPoints[i], dashPoints[i + 1], Color.red, 2f);
            //}
        }
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
        if (swordCollider.enabled == false) swordCollider.enabled = true; // Enable the sword collider during the attack step
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

