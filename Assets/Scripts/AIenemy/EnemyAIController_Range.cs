using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

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
    private Animator anim;
    float lastX;
    [SerializeField] public float attackCD;
    [HideInInspector] public float attackCDtimer;
    private Coroutine waitingCoroutine;
    private float facingDirection = 1f;
    [HideInInspector] public Vector2 targetpos;
    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] private Transform turret;
    [SerializeField] private bool arena; // If true, roaming area is centered around (0,0), otherwise around startPosition
    private Rigidbody2D rb;
    [HideInInspector] public bool chasing = false;
    [HideInInspector] public bool isAttack = false;// Flag to check if the enemy is currently chasing the player
    private bool playerDied = false; // Flag to check if the player has died
    private Player_Health playerHealth;

    private void Awake()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Health>();
        enemyShot = GetComponent<Enemy_AttackShot>();
        lastPlayerPosition = transform.position;
        state = State.Roaming;
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        StartCoroutine(Roaming()); // Start the roaming coroutine when the enemy AI starts
        agent.updateRotation = false; // Disable automatic rotation of the NavMeshAgent
        agent.speed = speed; // Set the speed of the NavMeshAgent
    }

    private void Update()
    {
        playerDied = playerHealth.Died;
        currentPosition = transform.position;
        CheckForPlayer(); // Check for player in the trigger area

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
                if (!chasing)
                {
                    chasing = true;
                    StartCoroutine(Chasing());
                }
                target = playerTransform;
                if (state != State.Chasing && state != State.Attacking) state = State.Chasing;
                     
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
        StopCoroutine(Roaming());
        yield return new WaitForSeconds(waitTime); // Czekaj okreœlony czas
        state = State.Roaming;
        roamPosition = Vector2.zero;
        waitingCoroutine = null;
        StartCoroutine(Roaming());
    }

    private IEnumerator Roaming()
    {
        agent.stoppingDistance = 0f; // Reset the stopping distance for the NavMeshAgent
        bool canRoam = false; // Flag to check if a valid roaming position is found
        while (!canRoam)
        {
            roamPosition = GetRoamingPosition();
            Collider2D hit = Physics2D.OverlapCircle(roamPosition, 1.5f, wallLayer); // Check if the roaming position is valid
            canRoam = (hit == null); // If no wall is hit, the position is valid
        }

        if (waitingCoroutine != null)
        {
            StopCoroutine(waitingCoroutine);
            waitingCoroutine = null;
        }

        Debug.Log(Vector2.Distance(transform.position, roamPosition));

        agent.SetDestination(roamPosition); // Set the destination to the roaming position

        while (true)
        {
            if (Vector2.Distance(transform.position, roamPosition) < .1f)
            {
                state = State.Waiting;
                if (waitingCoroutine == null)
                    waitingCoroutine = StartCoroutine(Waiting());
                roamPosition = Vector2.zero; // Reset roam position after reaching it
                yield break; // Exit the coroutine when the roaming position is reached
            }
            yield return null; // Wait for the next frame
        }
    }

    public IEnumerator Chasing()
    {
        anim.SetBool("isAttacking", false); // Reset attack animation when chasing starts
        agent.stoppingDistance = stopDistance; // Set the stopping distance for the NavMeshAgent
        StopCoroutine(Roaming()); // Stop roaming coroutine if chasing
        StopCoroutine(Waiting()); // Stop waiting coroutine if chasing
        isAttack = false; // Reset attack flag when chasing starts

        while (true)
        {
            if (target != null)
            {

                float distanceToPlayer = Vector2.Distance(currentPosition, target.position);

                if (distanceToPlayer > stopDistance)
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                }
                else
                {
                    agent.isStopped = true; // Zatrzymaj ruch agenta
                    rb.velocity = Vector2.zero; // Dla pewnoœci zatrzymaj Rigidbody2D
                }


                if (attackCDtimer <= 0)
            {
                    agent.isStopped = true; // Zatrzymaj ruch agenta
                    rb.velocity = Vector2.zero; // Dla pewnoœci zatrzymaj Rigidbody2D
                    isAttack = true; // Set attack flag when close enough to the player
                    targetpos = target.position;
                    state = State.Attacking; // If close enough to the player, switch to waiting state
                    enemyShot.StartShot();
                    yield break;
            }
        }

        if (!inThis)
        {
                agent.isStopped = false;
                agent.stoppingDistance = 0f;
                agent.SetDestination(lastPlayerPosition); // Set the destination to the last known player position

                if (playerDied)
                {
                    playerDied = false; // Reset the playerDied flag
                    roamPosition = Vector2.zero; // Reset roam position when returning

                    state = State.Roaming; // Idzie do innego miejsca w patrolu
                    StartCoroutine(Roaming()); // Start roaming again after returning
                    chasing = false; // Reset chasing flag when returning to last known position

                    yield break; // Exit the coroutine when the enemy has returned to the last known player position
                }

                if (Vector2.Distance(transform.position, lastPlayerPosition) < .1f)
                {

                    roamPosition = Vector2.zero; // Reset roam position when returning
                                               
                    state = State.Roaming; // Idzie do innego miejsca w patrolu
                    StartCoroutine(Roaming()); // Start roaming again after returning
                    chasing = false; // Reset chasing flag when returning to last known position
                    yield break; // Exit the coroutine when the enemy has returned to the last known player position
                }
            }
            yield return null;
        }
    }

    private void Returning()
    {
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
