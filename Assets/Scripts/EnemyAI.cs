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

    [SerializeField] private float speed = 2f; // Speed of the enemy
    [SerializeField] private float roamRadius = 5f; // Radius within which the enemy roams
    [SerializeField] private float roamTime = 2f; // Speed of the enemy while roaming
    [SerializeField] private float waitTime = 2f; // Time to wait at each roaming position
    [SerializeField] private float howFar = 2; // How far the enemy can roam from the start position
    public GameObject player; 
    private State state;
    private EnemyPathfinding enemyPathfinding;
    private Vector2 startPosition;
    private Vector2 roamPosition;
    private Vector2 enemyPosition;
    private Transform target;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
    }

    private void Start()
    {
        startPosition = transform.position;
        Debug.Log("Enemy AI started at position: " + startPosition);
        StartCoroutine(EnemyRoutine());
    }

    private void Update()
    {
        enemyPosition = transform.position;
        if (target !=null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, target.position, step);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            target = null;
        }
    }

    private IEnumerator EnemyRoutine()
    {
        while (true)
        {
            if (state == State.Roaming)
            {
                Debug.Log("Enemy is roaming.");
                roamPosition = GetRoamingPosition();
                enemyPathfinding.MoveTo(roamPosition);
                yield return new WaitForSeconds(roamTime);

                if (enemyPosition.x > startPosition.x + howFar || enemyPosition.x < startPosition.x - howFar || enemyPosition.y > startPosition.y + howFar || enemyPosition.y < startPosition.y - howFar)
                {
                    state = State.Returning;
                    //Debug.Log("Enemy is out of roaming zone, returning to start position.");
                }
                
                state = State.Waiting;
            }

            else if (state == State.Waiting)
            {
                Debug.Log("Enemy is waiting.");
                enemyPathfinding.MoveTo(Vector2.zero); // Stop moving
                yield return new WaitForSeconds(waitTime);
                state = State.Roaming;
            }

            else if (state == State.Returning)
            {
                Debug.Log("Enemy is returning to start position.");
                enemyPathfinding.MoveTo(startPosition);
                yield return new WaitForSeconds(5);
                state = State.Waiting;
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

}
