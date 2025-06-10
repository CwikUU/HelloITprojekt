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
    [HideInInspector] public float stuneTimer; // To check if the player is rolling

    private float rollCd = 0f;


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
                StartCoroutine(Roll());
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

    private IEnumerator Roll()
    {
        float originalSpeed = speed;
        speed *= 5;
        yield return new WaitForSeconds(0.1f);
        speed = originalSpeed;
        
    }
}
        
