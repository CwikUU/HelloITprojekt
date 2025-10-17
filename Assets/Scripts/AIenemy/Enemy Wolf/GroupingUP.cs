using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    private EnemyAIController_Mele enemyController;
    [HideInInspector] public Transform target;
    public float timer = 55f;
    public List<GameObject> gameObjects = new List<GameObject>();
    List<float> times = new List<float>();
    bool dontSee = false;
    bool isFollowing = false;



    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponentInParent<EnemyAIController_Mele>();
    }

    // Update is called once per frame
    void Update()
    {
        target = null;
        target = enemyController.target;

        if (target == null)
        {
            timer += Time.deltaTime; // Increment timer when no target is found
            if (!dontSee)
            {
                if (!isFollowing)
                {
                    enemyController.StopAllCoroutines(); // Stop all coroutines if no target is found
                }
                WhoFollow();
                dontSee = true; 
            }
        }
        else
        {
            timer = 0f; // Reset timer when a target is found
            StartCoroutine(enemyController.Chasing()); // Start following the target
            dontSee = false;
            isFollowing = false; // Reset isFollowing when a target is found
        }
    }

    private void WhoFollow()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            if (gameObjects[i] != null)
            {
                Group group = gameObjects[i].GetComponentInChildren<Group>();
                if (group != null)
                {

                    float otherTimer = group.timer;

                    if (otherTimer > timer)
                    {
                        enemyController.StopAllCoroutines(); // Stop all coroutines for this enemy
                        StartCoroutine(enemyController.Waiting());
                        break; // Exit the loop once a valid target is found
                    }

                    if (otherTimer < timer)
                    {
                        Follow(gameObjects[i].transform);
                        dontSee = false; // Reset dontSee if a valid target is found
                        isFollowing = true; // Set isFollowing to true when following a target
                        break;
                    }
                }
            }
        }
    }

    private void Follow(Transform position)
    {
        enemyController.agent.SetDestination(position.position);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObjects.Contains(collision.gameObject))
        {
            gameObjects.Add(collision.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (gameObjects.Contains(collision.gameObject))
        {
            gameObjects.Remove(collision.gameObject);
        }
    }
}
