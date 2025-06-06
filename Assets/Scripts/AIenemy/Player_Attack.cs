using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.5f;

    private Animator animator;
    private Sword sword;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        sword = GetComponentInChildren<Sword>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0)&& attackCooldown<=0 )//
        {
            animator.SetBool("isAttacking", true);
        }
    }


    public void Attack()
    {
        animator.SetBool("isAttacking", false);
        attackCooldown = 0.5f; // Reset cooldown after attack
    }

    public void DrawSword()
    {
        sword.DrawSword(); // Call the DrawSword method from the Sword script
    }

}
