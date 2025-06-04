using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public TMP_Text healtText;

    private void Start()
    {
        currentHealth = maxHealth;
        healtText.text = "HP: " + currentHealth /*+ "Health: " + maxHealth*/;
    }

    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TakeDamage(1);
            }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healtText.text = "HP: " + currentHealth;
        
    }
}
        
        
        

            
