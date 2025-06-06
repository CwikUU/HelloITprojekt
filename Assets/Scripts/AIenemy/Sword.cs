using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private bool Player;

    private Collider2D sword;

    private void Start()
    {
        sword = GetComponent<Collider2D>();
        sword.enabled = false; // Disable the sword collider initially
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Player)
        {
            if (collision.CompareTag("Enemy"))
            {
                Debug.Log("Enemy hit!");
                Enemy_Health enemyHealth = collision.GetComponent<Enemy_Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(1);
                }
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                Player_Health playerHealth = collision.GetComponent<Player_Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
            }
        }
    }

    public void DrawSword()
    {
        if (sword.enabled == true)
        {
            sword.enabled = false; // Disable the sword collider
        }
        else
        {
            sword.enabled = true; // Enable the sword collider
        }
    }
}
