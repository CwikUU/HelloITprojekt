using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boulder : MonoBehaviour
{
    private Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Health player = collision.GetComponent<Player_Health>();
            if (player != null)
            {
                player.TakeDamage(4); // Adjust damage value as needed
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Destroy(gameObject, 3f);

        animator.SetTrigger("isFalling");
    }

}
