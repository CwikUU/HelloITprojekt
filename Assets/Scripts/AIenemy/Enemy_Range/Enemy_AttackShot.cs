using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AttackShot : MonoBehaviour
{
    [SerializeField] private Transform shotPoint; // The point from where the shot will be fired
    [SerializeField] private GameObject projectilePrefab; // The projectile prefab to be instantiated
    [SerializeField] private float projectileSpeed = 5f; // Speed of the projectile

    private float attackCooldown;
    private Vector2 target;

    private Animator animator;
    private EnemyAIController_Range rangeAIController;

    void Start()
    {
        rangeAIController = GetComponent<EnemyAIController_Range>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        else
        {
            rangeAIController.attackCDtimer = attackCooldown; // Update the attack cooldown timer in Proba
        }

    }

    public void StartShot()
    {
        animator.SetBool("isShooting", true);
        attackCooldown = rangeAIController.attackCD;
        rangeAIController.attackCDtimer = attackCooldown;
    }

    public void EndShot()
    {
        animator.SetBool("isShooting", false);
        rangeAIController.isAttack = false;
        target = rangeAIController.targetpos;
        Vector2 direction = (target - (Vector2)shotPoint.position).normalized; // Calculate the direction to the target
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Calculate the angle in degrees
        GameObject rock = Instantiate(projectilePrefab, shotPoint.position, Quaternion.Euler(new Vector3(0,0,angle))); // Instantiate the projectile at the shot point
        rock.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed; // Set the projectile's velocity
        rangeAIController.state = EnemyAIController_Range.State.Chasing; // Reset state to chasing after shooting
        StartCoroutine(rangeAIController.Chasing());
    }
}
