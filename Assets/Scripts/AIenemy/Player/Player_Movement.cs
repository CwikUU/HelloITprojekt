using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator animator;
    [SerializeField] private float rollCD = 3; // Cooldown for rolling
    [SerializeField] private Image rollActive;
    [SerializeField] private Transform rollPoint;
    [HideInInspector] public float stuneTimer; // To check if the player is rolling
    [SerializeField] private Transform playerTransform;

    private float rollCd = 0f;
    private List<Vector2> rollPoints = new List<Vector2>();
    private int totalRollPoints = 30;


    private void Start()
    {
        rollActive.color = Color.green;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stuneTimer > 0)
        {
            stuneTimer -= Time.fixedDeltaTime;
            Debug.Log("Stunned for: " + stuneTimer + " seconds");
        }

        if (stuneTimer <= 0)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            animator.SetFloat("horizontal", Mathf.Abs(horizontal));
            animator.SetFloat("vertical", Mathf.Abs(vertical));


            rb.velocity = new Vector2(horizontal, vertical) * speed;

            if (rollCd > 0)
            {
                rollCd -= Time.fixedDeltaTime;
            }
            else
            {
                rollActive.color = Color.green; // Reset color when cooldown is over
            }
            if (Input.GetKey(KeyCode.Space) && rollCd <= 0f)
            {
                rollActive.color = Color.red;
                Roll(playerTransform.position, rollPoint.position);
                rollCd = rollCD;
            }

            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPosition - rb.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
        else
        {
            rb.velocity = Vector2.zero; // Stop movement when stunned
            animator.SetFloat("horizontal", 0);
            animator.SetFloat("vertical", 0);
        }
    }

    private void Roll(Vector2 start, Vector2 end)
    {
        rollPoints.Clear();
        bool canDash = false;

        for (int i = 0; i < totalRollPoints; i++)
        {
            float t = (float)i / (totalRollPoints - 1);
            Vector2 point = Vector2.Lerp(start, end, t);
            rollPoints.Add(point);
        }

        for (int i = 0; i < rollPoints.Count; i++)
        {
            Collider2D hit = Physics2D.OverlapCircle(rollPoints[i], .5f, LayerMask.GetMask("Collision"));
            canDash = (hit == null);

            if (!canDash)
            {
                int safeIndex = Mathf.Max(0, i - 1);
                playerTransform.position = rollPoints[safeIndex]; // Teleport player to roll point
                break; // Exit the loop if a collision is detected
            }
            else if (i == rollPoints.Count - 1) // If we reach the last point without collision
            {
                playerTransform.position = rollPoints[i]; // Teleport player to roll point
                break;
            }
        }
    }
}
        
