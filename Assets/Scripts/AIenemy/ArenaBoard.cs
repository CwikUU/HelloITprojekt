using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBoard : MonoBehaviour
{
    [SerializeField] private GameObject arenaSpawn;
    private bool interact = false;


    private void Start()
    {
        arenaSpawn.SetActive(false);
    }

    private void Update()
    {
        if (interact && Input.GetKeyDown(KeyCode.E))
        {
            arenaSpawn.SetActive(!arenaSpawn.activeSelf);
            if (arenaSpawn.activeSelf)
            {
                // Additional logic when the arena is activated can be added here
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interact = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interact = false;
            arenaSpawn.SetActive(false);
        }
    }
}
