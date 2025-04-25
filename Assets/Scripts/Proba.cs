using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Proba : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Waiting,
        Returning,
        Chasing,
    }

    [SerializeField] private float speed; // Speed of the enemy
    [SerializeField] private float roamRadius; // Radius within which the enemy roams
    [SerializeField] private float roamTime; // Speed of the enemy while roaming
    [SerializeField] private float waitTime; // Time to wait at each roaming position
    [SerializeField] private float howFar;// How far the enemy can roam from the start position
    [SerializeField] private float stopDistance;
    public GameObject player; 
    private State state;
    private EnemyPathfinding enemyPathfinding;
    private Vector2 startPosition;
    private Vector2 roamPosition;
    private Vector2 currentPosition;
    private Vector2 lastPlayerPosition;
    private Transform target;
    private bool inThis = false;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
        startPosition = transform.position;
    }

    private void Start()
    {
        
        Debug.Log("Enemy AI started at position: " + startPosition);
        
    }

    private void Update()
    {
        currentPosition = transform.position;
        Debug.Log("target position: " + currentPosition + lastPlayerPosition+inThis+state+ Vector2.Distance(currentPosition, lastPlayerPosition));
        EnemyRoutine();
    }

    private void EnemyRoutine()
    {
        switch (state)
        {   
            case State.Roaming:
                Roaming();
                break;
            case State.Waiting:
                Waiting();
                break;
            case State.Returning:
                Returning();
                break;
            case State.Chasing:
                Chasing();
                break;
        }   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            inThis = true;
            target = other.transform;
            state = State.Chasing;
            Debug.Log("wszedl ");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            lastPlayerPosition = new Vector2(target.position.x,target.position.y);
            target = null;
            inThis = false; 
            Debug.Log("wyszedl");
        }
    }

    

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(
            Random.Range(-roamRadius, roamRadius),
            Random.Range(-roamRadius, roamRadius)
        );

    }

    private void Waiting()
    {
        Debug.Log("Waiting for " + waitTime + " seconds.");
    }

    private void Roaming()
    {
        Debug.Log("Roaming state: ");
    }

    private void Chasing()
    {
        Debug.Log("sciga" + state);
        

        if (!inThis)
        {
            
            Debug.Log("znikl");
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(currentPosition, lastPlayerPosition, step);
            
            if (Vector2.Distance(currentPosition, lastPlayerPosition) == 0f )
            {
                state = State.Roaming;
                Debug.Log("Enemy has stopped chasing the player." + state);
            }
        }

        if (Vector2.Distance(currentPosition, target.position) > stopDistance && inThis)
        {
            Debug.Log("scigam");
            float step = speed * Time.deltaTime;
            
            transform.position = Vector2.MoveTowards(currentPosition, target.position, step);
        }
    }

    private void Returning()
    {
        //Debug.Log("Distance to start position: " + Vector2.Distance(transform.position, startPosition));
        //Debug.Log("Speed: " + speed + state);

        //if (Vector2.Distance(transform.position, startPosition) > 0.1f)
        //{
        //    Debug.Log("Moving towards start position");
        //    float step = speed * Time.deltaTime;
        //    transform.position = Vector2.MoveTowards(transform.position, startPosition, step);
        //    Debug.Log("Speed: " + speed + end);
        //}
        //else
        //{
        //    Debug.Log("Enemy has returned to start position.");
        //    end = true;
        //    state = State.Roaming; // After returning, go back to roaming
        //}
      
    }
}
