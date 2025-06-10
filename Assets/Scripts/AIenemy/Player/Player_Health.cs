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
    private Vector2 startPos;
    [HideInInspector] public bool Died = false; // Variable to track if the player has died

    private GameObject player;
    public TMP_Text healtText;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPos = player.transform.position;
        currentHealth = maxHealth;
        healtText.text = "HP: " + currentHealth /*+ "Health: " + maxHealth*/;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        healtText.text = "HP: " + currentHealth;
        if (currentHealth == 0) Die();
    }

    private void Die()
    {
        
        Died=true;
        Debug.Log("Player has diedahsdgjashjdsadghgjadsjgasdhgasdjhhjasdhgasdhgsadhgsadghasdgh");
        currentHealth = maxHealth;
        healtText.text = "HP: " + currentHealth;
        player.transform.position = startPos; // Resetuj pozycjê gracza

    }
}
