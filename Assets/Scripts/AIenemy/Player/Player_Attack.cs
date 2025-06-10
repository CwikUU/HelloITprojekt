using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.5f;
    private float attackCooldownTimer = 0f;

    private Animator animator;
    private Sword sword;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();
        attackCooldownTimer = attackCooldown; // Initialize cooldown timer
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0)&& attackCooldownTimer <= 0 )//
        {
            animator.SetBool("isAttacking", true);
        }
    }


    public void Attack()
    {
        animator.SetBool("isAttacking", false);
        attackCooldownTimer = attackCooldown; // Reset cooldown after attack
    }

    public void DrawSword()
    {
        sword.DrawSword(); // Call the DrawSword method from the Sword script
    }

}
