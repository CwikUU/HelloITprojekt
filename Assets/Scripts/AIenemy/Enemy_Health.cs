using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    [SerializeField] private float untouchable = .5f;

    private void Update()
    {
        if (untouchable > 0)
        {
            untouchable -= Time.deltaTime;
        }
        else
        {
            untouchable = 0;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (untouchable <= 0)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            if (currentHealth == 0) Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject); // Destroy the enemy game object
    }
}
