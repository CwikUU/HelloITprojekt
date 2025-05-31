using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyAI : MonoBehaviour
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
    private Transform target;
    private bool end = false;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
        startPosition = transform.position;

    }

    private void Start()
    {
        
        Debug.Log("Enemy AI started at position: " + startPosition);
        StartCoroutine(EnemyRoutine());
    }

    private void Update()
    {
        currentPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("wszedl ");
            end = false;
            target = other.transform;
            StopAllCoroutines(); // Stop the roaming coroutine
            Debug.Log("wszedl2 " + state + end);
            state = State.Chasing;
            StartCoroutine(EnemyRoutine()); // Start the chasing coroutine
            Debug.Log("wszedl3 " + state + end);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("wyszedl");
            end =false; 
            target = null;
            StopAllCoroutines(); // Stop the chasing coroutine
            Debug.Log("wyszedl2 "+state+end);
            state = State.Returning;
            StartCoroutine(EnemyRoutine()); // Start the returning coroutine
            Debug.Log("wyszedl3 " + state + end);
        }
    }

    private IEnumerator EnemyRoutine()
    {
        while (true)
        {
            if (state == State.Roaming)
            {
                Debug.Log("Enemy is roaming.");
                //roamPosition = GetRoamingPosition();
                //enemyPathfinding.MoveTo(roamPosition);


                ////if (enemyPosition.x > startPosition.x + howFar || enemyPosition.x < startPosition.x - howFar || enemyPosition.y > startPosition.y + howFar || enemyPosition.y < startPosition.y - howFar)
                ////{
                ////    state = State.Returning;
                ////    //Debug.Log("Enemy is out of roaming zone, returning to start position.");
                ////}
                yield return null;
                state = State.Waiting;
            }

            else if (state == State.Waiting)
            {
                Debug.Log("Enemy is waiting.");
                //enemyPathfinding.MoveTo(Vector2.zero); // Stop moving
                yield return new WaitForSeconds(waitTime);
                state = State.Roaming;
            }

            else if (state == State.Returning)
            {
                Debug.Log("Enemy is returning to start position.");

                Returning();
                
                //yield return new WaitUntil(() => end); // Continue returning until the enemy reaches the start position
                
                end = false;
                //state = State.Roaming; // After returning, go back to roaming
            }

            else if (state == State.Chasing)
            {
                Debug.Log("Enemy is chasing the player." + state);
                Chasing();
                //yield return new WaitUntil(() => end); // Continue chasing until the player is out of range
                
                Debug.Log("Enemy has stopped chasing the player." + state);
            }
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(
            Random.Range(-roamRadius, roamRadius),
            Random.Range(-roamRadius, roamRadius)
        );

    }
    
    private void Chasing()
    {
        //Debug.Log("sciga" + state);
        //if (Vector2.Distance(currentPosition, target.position) > stopDistance)
        //{
        //    float step = speed * Time.deltaTime;
        //    transform.position = Vector2.MoveTowards(currentPosition, target.position, step);
        //}
        if (Vector2.Distance(transform.position, startPosition) > stopDistance)
        {
            Debug.Log("Moving towards start position");
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, startPosition, step);
            Debug.Log("Speed: " + speed + end);
        }
        else
        {
            Debug.Log("Enemy has returned to start position.");
            end = true;
            state = State.Roaming; // After returning, go back to roaming
        }
        //end = true;

    }

    private void Returning()
    {
        Debug.Log("Distance to start position: " + Vector2.Distance(transform.position, startPosition));
        Debug.Log("Speed: " + speed + state);

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
        Debug.Log("sciga" + state);
        if (Vector2.Distance(currentPosition, target.position) > stopDistance)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(currentPosition, target.position, step);
        }
    }
}
