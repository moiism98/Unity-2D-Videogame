using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBehaviour : MonoBehaviour
{
    private EnemyController enemyController;
    private Animator animator;
    private Rigidbody2D frogRb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float jumpForce = 5f;
    private bool moveComplete;

    [Header("Timers")]
    [SerializeField] private float stopMove = 1.5f;
    void Start()
    {
        enemyController = GetComponent<EnemyController>();

        frogRb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        moveComplete = true;
    }

    public void Update()
    {
        animator.SetFloat("Fall", frogRb.velocity.y);
    }

    public IEnumerator Move()
    {
        if(moveComplete && isGrounded())
        {
            moveComplete = false;

            enemyController.ApplyVelocity(frogRb, jumpForce);

            animator.SetTrigger("Jump");

            yield return new WaitForSeconds(stopMove);

            enemyController.ChangeEnemyDirection();

            enemyController.ApplyVelocity(frogRb, jumpForce);

            animator.SetTrigger("Jump");

            moveComplete = true;
        }
    }

    /// <summary>
    /// Check if the frog is on the ground. If the frog is on the ground he can jump.
    /// </summary>
    /// <returns></returns>
    private bool isGrounded()
    {
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, enemyController.GetGroundLayer()))
            return true;
        else 
            return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
