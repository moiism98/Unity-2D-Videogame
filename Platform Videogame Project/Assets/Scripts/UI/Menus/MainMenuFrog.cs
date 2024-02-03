using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuFrog : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private bool isJumping = false;
    private float direction = -1;
    private Vector2 groundCheckSize = new Vector2(.3f, .1f);
    void Start()
    {
        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", direction);

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        animator.SetFloat("Fall", rb.velocity.y);
    }

    private void FixedUpdate()
    {
        StartCoroutine(Jump());
    }

    /// <summary>
    /// The frog will jump eternally.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Jump()
    {
        if(!isJumping && IsGrounded())
        {
            isJumping = true;

            animator.SetTrigger("Jump");

            rb.velocity = new Vector2(direction * moveSpeed, jumpForce);

            yield return new WaitForSeconds(3.5f);

            direction *= -1;

            animator.SetTrigger("Jump");

            rb.velocity = new Vector2(direction * moveSpeed, jumpForce);

            isJumping = false;
        }
    }

    /// <summary>
    /// Detects if the frog is touching the ground to jump again.
    /// </summary>
    /// <returns></returns>
    private bool IsGrounded()
    {
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
            return true;
        else 
            return false;
    }
}
