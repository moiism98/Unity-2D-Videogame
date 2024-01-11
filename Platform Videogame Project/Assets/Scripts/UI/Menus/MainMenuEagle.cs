using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEagle : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private float direction;
    private bool isMoving = false;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] float moveTime = 2f;
    private void Start()
    {
        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        direction = 1;
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", direction);
    }

    private void FixedUpdate()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        if(!isMoving)
        {
            isMoving = true;

            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

            yield return new WaitForSeconds(moveTime);

            direction *= -1;

            //rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

            isMoving = false;
        }
    }
}
