using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Collider2D sword;

    private void Start()
    {
        sword = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        //collision = sword.GetComponent<Collider2D>();
        if (collision.CompareTag("Enemy"))
        {
           Debug.Log("Enemy hit!");
            Enemy_Health enemyHealth = collision.GetComponent<Enemy_Health>();
            if (enemyHealth != null)
            {
                //enemyHealth.TakeDamage(1);
            }
        }
    }
}
