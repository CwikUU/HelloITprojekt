using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public int heal = 10;
    public float speedMultiplier = 6;
    public float speedDuration = 5;

 

    public virtual void UseItem()
    {
        Debug.Log("Using item " + ID);
        if (ID == 0)
        {
            PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(heal);
            }
        }
        else if (ID == 1)
        {
            PlayerMovement playerSpeed = FindObjectOfType<PlayerMovement>();
            if (playerSpeed != null)
            {
                playerSpeed.BoostSpeed(speedMultiplier,speedDuration);
            }
        }

        InventoryController inventoryController = FindObjectOfType<InventoryController>();
        if (inventoryController != null)
        {
            inventoryController.RemoveItem(gameObject);
        }
    }
}
