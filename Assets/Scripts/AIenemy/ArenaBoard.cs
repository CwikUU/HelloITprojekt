using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaBoard : MonoBehaviour
{
    [SerializeField] private GameObject arenaSpawn;
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameObject Spawner;
    private bool interact = false;


    private void Start()
    {
        arenaSpawn.SetActive(false);
        Spawner.SetActive(false); // Ensure the Spawner is inactive at the start
    }

    private void Update()
    {
        if (interact && Input.GetKeyDown(KeyCode.E))
        {
            arenaSpawn.SetActive(!arenaSpawn.activeSelf);
            if (arenaSpawn.activeSelf)
            {
                Menu.SetActive(true);
                Spawner.SetActive(false);
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
            Menu.SetActive(true);
            Spawner.SetActive(false); 
        }
    }

    public void OpenSpawn()
    {
        Menu.SetActive(false); // Optionally deactivate the menu if needed
        Spawner.SetActive(true); // Activate the Spawner when the button is pressed
    }
}
