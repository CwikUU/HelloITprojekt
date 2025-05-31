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
    [Range(0, 10)]
    [SerializeField] private float speed; // Speed of the enemy
    [Range(0, 10)]
    [SerializeField] private int waitTime; // Time to wait at each roaming position
    [Range(0, 10)]
    [SerializeField] private float howFar;// How far the enemy can roam from the start position
    [Range(0, 5)]
    [SerializeField] private float stopDistance;
    public GameObject player;
    private State state;
    private Vector2 startPosition;
    private Vector2 roamPosition;
    private Vector2 currentPosition;
    private Vector2 lastPlayerPosition;
    private Transform target;
    private bool inThis = false;
    private float waitingTime = 0f;

    private void Awake()
    {
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
        Debug.Log("target position: " + currentPosition + roamPosition+inThis+state+ Vector2.Distance(currentPosition, roamPosition));
        EnemyRoutine();
    }

    public void EnemyRoutine()
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

    private void Waiting()
    {
        Debug.Log("Waiting state: " );
        if (waitingTime < waitTime)
        {
            waitingTime += Time.deltaTime;
        }
        else
        {
            waitingTime = 0f;
            state = State.Roaming; // After waiting, go back to roaming
            roamPosition = Vector2.zero;
        }
    }

    private void Roaming()
    {
        state = State.Roaming;
        Debug.Log("Roaming state: ");

        if (roamPosition == Vector2.zero)
        {
            roamPosition = GetRoamingPosition();
        }

        if (roamPosition != Vector2.zero || Vector2.Distance(currentPosition, startPosition) == 0)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(currentPosition, roamPosition, step);

            if (Vector2.Distance(currentPosition, roamPosition) < 0.1f)
            {
                Debug.Log("Enemy has reached the roaming position.");
                state = State.Waiting;
                
            }

        }
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
                state = State.Returning;
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

        if (Vector2.Distance(transform.position, roamPosition) > 0.1f)
        {
            Debug.Log("Moving towards start position");
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, roamPosition, step);
        }
        else
        {
            Debug.Log("Enemy has returned to start position.");
            roamPosition = Vector2.zero;
            state = State.Roaming; // After returning, go back to roaming
        }

    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(
            Random.Range(startPosition.x-howFar, startPosition.x + howFar),
            Random.Range(startPosition.y - howFar, startPosition.y + howFar)
        );

    }
}
