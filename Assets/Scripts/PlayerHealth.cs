using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int damageAmount = 10;
    public float timeBetweenDamage = 5f;

    //private bool isDead;

    public HealthBar healthBar;
    public static event Action OnPlayerDeath;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        InvokeRepeating("ApplyPeriodicDamage", timeBetweenDamage, timeBetweenDamage);
        GameManager.OnRestart += RestartHealth;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TakeDamage(20);
        //}
        if (currentHealth <= 0 )
        {
            //isDead = true;
            OnPlayerDeath.Invoke();
        }
    }

    public void RestartHealth()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth);
        //isDead = false;
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Current health: " + currentHealth);

        healthBar.SetHealth(currentHealth);
    }

    void ApplyPeriodicDamage()
    {
        TakeDamage(damageAmount);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Player healed " + amount + ". Current health: " + currentHealth);
        healthBar.SetHealth(currentHealth);
    }
}
