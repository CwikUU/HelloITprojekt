using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyAIController_Range : MonoBehaviour
{
    public enum State
    {
        Roaming,
        Waiting,
        Returning,
        Chasing,
        Attacking,
    }
    [Range(0, 10)]
    [SerializeField] private float speed; // Speed of the enemy
    [Range(0, 10)]
    [SerializeField] private int waitTime; // Time to wait at each roaming position
    [SerializeField] private bool draw; // Whether to draw the roaming area in the editor
    [SerializeField] private float howFarX, howFarY;// How far the enemy can roam from the start position
    [Range(0, 5)]
    [SerializeField] private float stopDistance;
    [SerializeField] private float enemyPOV; // Enemy's point of view distance
    [SerializeField] private LayerMask wallLayer;
    private Enemy_AttackShot enemyShot;
    public GameObject player;
    public LayerMask playerLayer;
    public State state;
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
    private Animator anim;
    float lastX;
    [SerializeField] public float attackCD;
    [HideInInspector] public float attackCDtimer;
    private Coroutine waitingCoroutine;
    private float facingDirection = 1f;
    [HideInInspector]public Vector2 targetpos;
    [SerializeField] private Transform turret;
    [SerializeField] private bool arena; // If true, roaming area is centered around (0,0), otherwise around startPosition

    private void Awake()
    {
        enemyShot = GetComponent<Enemy_AttackShot>();
        lastPlayerPosition = transform.position;
        state = State.Roaming;
        startPosition = transform.position;
        pathfinding = FindObjectOfType<Pathfinding>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        
        //Debug.Log("Enemy AI started at position: " + startPosition);
    }

    private void Update()
    {
        currentPosition = transform.position;
        //Debug.Log(state+"stastsatasdtdastasdtadstdasg");
        //Debug.Log("target position: " + currentPosition + roamPosition+inThis+state+ Vector2.Distance(currentPosition, roamPosition));
        EnemyRoutine();
        CheckForPlayer(); // Check for player in the trigger area

        float curX = transform.position.x;
        float delta = curX - lastX;
        if (Mathf.Abs(delta) > .01f)
        {
            if ((delta > 0 && facingDirection == 1) || (delta < 0 && facingDirection == -1))
            {
                facingDirection *= -1; // Flip the facing direction
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
                turret.localScale = scale;
            }
        }
        lastX = curX;

        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Rotate the shot point to face the target
        }
        else if (lastPlayerPosition != null && roamPosition == Vector2.zero)
        {
            Vector2 direction = (lastPlayerPosition - (Vector2)transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Rotate the shot point to face the last known player position
        }
        else if (roamPosition != Vector2.zero)
        {
            Vector2 direction = (roamPosition - (Vector2)transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Rotate the shot point to face the roaming position
        }
    }

    public void EnemyRoutine()
    {
        switch (state)
        {   
            case State.Roaming:
                Roaming();

                break;
            case State.Waiting:
                break;
            case State.Returning:
                Returning();
                break;
            case State.Chasing:
                StopCoroutine(Waiting()); // Stop waiting coroutine if chasing
                Chasing();
                break;
            case State.Attacking:
                break;
        }   
    }

    private void CheckForPlayer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyPOV, playerLayer);
        if (hitColliders.Length > 0)
        {
            Transform playerTransform = hitColliders[0].transform;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, playerTransform.position, wallLayer);
            if (hit.collider == null)
            {
                enemyPOV = 8f;
                inThis = true;
                target = playerTransform;
                if (state != State.Chasing && state != State.Attacking) state = State.Chasing;

                waypointIndex = 0; // Reset the waypoint index when entering the trigger
                                   //Debug.Log("wszedl ");
            }
            else
            {
                enemyPOV = 5f;
                if (target != null) lastPlayerPosition = new Vector2(target.position.x, target.position.y);
                inThis = false;
                target = null; // If there's a wall between the enemy and the player, set target to null
            }
        }

        if (hitColliders.Length == 0)
        {
            enemyPOV = 5f;
            if (target!=null) lastPlayerPosition = new Vector2(target.position.x, target.position.y);
            target = null;
            inThis = false;
            //Debug.Log("wyszedl");
        }
    }

    

    private IEnumerator Waiting()
    {
        //Debug.Log("Waiting state: " );
        yield return new WaitForSeconds(waitTime); // Czekaj okre�lony czas
        waitingTime = 0f;
        state = State.Roaming;
        roamPosition = Vector2.zero;
        waitingCoroutine = null;
        //Debug.Log("Enemy has finished waiting and is now roaming again." + state);
    }

    private void Roaming()
    {
        state = State.Roaming;
        //Debug.Log("Roaming state: ");
        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
            waitingCoroutine = null;
        }

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
                //Debug.Log("Enemy is roaming towards waypoint: " + waypointIndex + " at position: " + waypoints[waypointIndex]);
            if (Vector2.Distance(currentPosition, waypoints[waypointIndex]) < 0.1f)
                {
                    waypointIndex++;
                    //Debug.Log("Enemy has reached the waypoint: " + waypointIndex);
                }

                if (Vector2.Distance(currentPosition, roamPosition) < 0.1f)
                {
                   

                }
            }

        if (waypoints != null && waypointIndex >= waypoints.Length)
        {
            //Debug.Log("Enemy has reached the roaming position.");
            state = State.Waiting;
            if (waitingCoroutine == null)
                waitingCoroutine = StartCoroutine(Waiting());
            waypoints = null;
            roamPosition = Vector2.zero; // Reset roam position when reaching the roaming position
        }


    }

    private void Chasing()
    {
        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
            waitingCoroutine = null;
        }
        waypointIndex = 0;

        if (target != null)
        {
            float distanceToPlayer = Vector2.Distance(currentPosition, target.position);

            if (distanceToPlayer - .5f < stopDistance)
            {
                // Odsuwanie si� od gracza
                Vector2 directionAway = (currentPosition - (Vector2)target.position).normalized;
                float step = speed * Time.deltaTime;
                transform.position = (Vector2)transform.position + directionAway * step;
            }
            else
            {
                // Standardowe pod��anie za graczem
                waypoints = pathfinding.FindPatch(transform.position, target.position);

                if (distanceToPlayer > stopDistance && inThis)
                {
                    if (waypoints != null && waypointIndex < waypoints.Length)
                    {
                        float step = speed * Time.deltaTime;
                        transform.position = Vector2.MoveTowards(currentPosition, waypoints[waypointIndex], step);
                        if (Vector2.Distance(currentPosition, waypoints[waypointIndex]) < 0.1f)
                        {
                            waypointIndex++;
                        }
                    }
                }
            }

            if (attackCDtimer <= 0)
            {
                targetpos = target.position;
                enemyShot.StartShot();
            }
        }
        else
        {
            waypoints = pathfinding.FindPatch(transform.position, lastPlayerPosition);
        }

        if (!inThis)
        {
            if (waypoints != null && waypointIndex < waypoints.Length)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(currentPosition, waypoints[waypointIndex], step);

                if (Vector2.Distance(currentPosition, waypoints[waypointIndex]) < 0.1f)
                {
                    waypointIndex++;
                }
            }

            if (waypoints != null && waypointIndex >= waypoints.Length)
            {
                roamPosition = Vector2.zero;
                waypoints = null;
                waypointIndex = 0;
                state = State.Roaming;
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
            state = State.Roaming; // After returning, go back to roaming
        }

    }


    public Vector2 GetRoamingPosition()
    {
        if (arena)
        {
            return new Vector2(
                Random.Range(-howFarX, howFarX),
                Random.Range(-howFarY, howFarY)
            );
        }
        else
        {
            return new Vector2(
                Random.Range(startPosition.x - howFarX, startPosition.x + howFarX),
                Random.Range(startPosition.y - howFarY, startPosition.y + howFarY)
            );
        }
    }

    private void OnDrawGizmos()
    {
        if (draw)
        {
            if (arena)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(new Vector2(0, 0), new Vector2(howFarX * 2, howFarY * 2)); // Draw a wireframe cube around the roaming area
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(startPosition, new Vector2(howFarX * 2, howFarY * 2)); // Draw a wireframe cube around the roaming area
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyPOV); // Draw a sphere around the enemy to visualize the point of view
        }
    }
}
