using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Action")]
    [SerializeField] private EnemyAction enemyAction = EnemyAction.walk;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    private float direction = 1;

    [Header("Timers")]
    [SerializeField] private float moveTime = 5f;
    [SerializeField] private float stopMove = 1.5f;

    [Header("Layers checks")]
    [SerializeField] private LayerMask groundMask;

    #pragma warning disable // Could be null!
    [SerializeField] private Transform groundCheck;

    #pragma warning disable // Could be null!
    [SerializeField] private Vector2 groundCheckSize;

    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool moveComplete;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        moveComplete = true;
    }

    private void Update()
    {
        //direction = Mathf.Sign(player.position.x);

        animator.SetFloat("Horizontal", direction);

        animator.SetFloat("Speed", Mathf.Abs(direction));

        animator.SetFloat("Fall", rb.velocity.y);
    }

    private void FixedUpdate()
    {
        Move();

        //rb.velocity = new Vector2(direction, 0) * moveSpeed;
    }

    private void Move()
    {
        switch(enemyAction)
        {
            case EnemyAction.walk: StartCoroutine(EnemyWalk()); break;
            case EnemyAction.jump: StartCoroutine(EnemyJump()); break;
        }
        
    }

    private IEnumerator EnemyWalk()
    {
        if(moveComplete)
        {
            moveComplete = false;

            rb.velocity = rb.velocity = new Vector2(direction, 0) * moveSpeed;

            yield return new WaitForSeconds(moveTime);

            rb.velocity = Vector2.zero;

            yield return new WaitForSeconds(stopMove);

            SetDirection(direction * -1);

            rb.velocity = new Vector2(direction, 0) * moveSpeed;

            moveComplete = true;
        }
    }

    private IEnumerator EnemyJump()
    {
        if(moveComplete && isGrounded())
        {

            moveComplete = false;

            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            animator.SetTrigger("Jump");

            yield return new WaitForSeconds(3f);

            moveComplete = true;
        }
    }

    private bool isGrounded()
    {
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundMask)) return true;
        else return false;
    }

    private void SetDirection(float direction)
    {
        this.direction = direction;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
