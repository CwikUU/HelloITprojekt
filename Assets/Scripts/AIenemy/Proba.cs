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
    [SerializeField] private bool draw; // Whether to draw the roaming area in the editor
    [SerializeField] private float howFarX, howFarY;// How far the enemy can roam from the start position
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
    private Pathfinding pathfinding;
    private Vector2[] waypoints;
    private int waypointIndex = 0;
    private bool isReturning = false;

    private void Awake()
    {
        state = State.Roaming;
        startPosition = transform.position;
        pathfinding = FindObjectOfType<Pathfinding>();
    }

    private void Start()
    {
        
        Debug.Log("Enemy AI started at position: " + startPosition);
    }

    private void Update()
    {
        currentPosition = transform.position;
        //Debug.Log("target position: " + currentPosition + roamPosition+inThis+state+ Vector2.Distance(currentPosition, roamPosition));
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

            waypoints = pathfinding.FindPatch(transform.position, target.position);
            waypointIndex = 0; // Reset the waypoint index when entering the trigger
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
            waypoints = pathfinding.FindPatch(transform.position, roamPosition); // Get waypoints to the new roaming position
            waypointIndex = 0; // Reset the waypoint index when getting a new roaming position
        }

        
            if (waypoints != null && waypointIndex < waypoints.Length)
            {

                float step = speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(currentPosition, waypoints[waypointIndex], step);
                Debug.Log("Enemy is roaming towards waypoint: " + waypointIndex + " at position: " + waypoints[waypointIndex]);
            if (Vector2.Distance(currentPosition, waypoints[waypointIndex]) < 0.1f)
                {
                    waypointIndex++;
                    Debug.Log("Enemy has reached the waypoint: " + waypointIndex);
                }

                if (Vector2.Distance(currentPosition, roamPosition) < 0.1f)
                {
                   

                }
            }

            if (waypoints != null && waypointIndex >= waypoints.Length)
        {
            Debug.Log("Enemy has reached the roaming position.");
            state = State.Waiting;
            waypoints = null; // Clear waypoints after reaching the roaming position
        }
        

    }

    private void Chasing()
    {
        Debug.Log("sciga" + state);
        

        if (!inThis)
        {
            if (waypoints != null && waypointIndex < waypoints.Length)
            {


                Debug.Log("znikl");
                float step = speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(currentPosition, waypoints[waypointIndex], step);

                if (Vector2.Distance(currentPosition, waypoints[waypointIndex]) < 0.1f)
                {
                    waypointIndex++;
                    Debug.Log("Enemy has reached the waypoint: " + waypointIndex);
                }

                if (Vector2.Distance(currentPosition, lastPlayerPosition) == 0f)
                {
                    isReturning = true;
                    roamPosition = Vector2.zero; // Reset roam position when returning
                    //state = State.Returning; // wraca do miejsca startowego trza zrobic
                    state = State.Roaming; // Idzie do innego miejsca w patrolu
                    Debug.Log("Enemy has stopped chasing the player." + state);
                }
            }
        }

        if (Vector2.Distance(currentPosition, target.position) > stopDistance && inThis)
        {
            if (waypoints != null && waypointIndex < waypoints.Length)
            {

                Debug.Log("scigam");
                float step = speed * Time.deltaTime;

                transform.position = Vector2.MoveTowards(currentPosition, waypoints[waypointIndex], step);
                if (Vector2.Distance(currentPosition, waypoints[waypointIndex]) < 0.1f)
                {
                    waypointIndex++;
                    Debug.Log("Enemy has reached the waypoint: " + waypointIndex);
                }
            }
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
            isReturning = false;
            state = State.Roaming; // After returning, go back to roaming
        }

    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(
            Random.Range(startPosition.x-howFarX, startPosition.x + howFarX),
            Random.Range(startPosition.y - howFarY, startPosition.y + howFarY)
        );

    }

    private void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(startPosition, new Vector2(howFarX * 2, howFarY * 2)); // Draw a wireframe cube around the roaming area
        }
    }
}
