using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private bool Player;
    [HideInInspector] public bool isGiant;
    private float timer = 0f;
    private Collider2D sword;
    [HideInInspector]public bool Stune = false;

    private void Start()
    {
        sword = GetComponent<Collider2D>();
        sword.enabled = false; // Disable the sword collider initially
    }
    private void Update()
    {
        timer += Time.deltaTime;
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
                sword.enabled = false; // Disable the sword collider after hitting the player
            }
        }

        if (isGiant)
        {

            if (collision.CompareTag("Player"))
            {
                
                if (Stune)
                {

                }
                Player_Health playerHealth = collision.GetComponent<Player_Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(2); // Deal more damage for giant sword
                }
                sword.enabled = false; // Disable the sword collider after hitting the player
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
