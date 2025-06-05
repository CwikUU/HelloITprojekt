using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float attackCooldown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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

}
