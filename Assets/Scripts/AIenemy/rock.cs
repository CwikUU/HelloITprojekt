using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rock : MonoBehaviour
{
 
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 4f); // Destroy the rock after 4 seconds
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Assuming the player has a method to take damage
            collision.GetComponent<Player_Health>().TakeDamage(1); // Deal 1 damage to the player
            Destroy(gameObject); // Destroy the rock on impact
        }

        if (collision.CompareTag("Collision"))
        {
            Destroy(gameObject); // Destroy the rock when it hits the ground
        }
    }
}
