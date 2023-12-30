using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameController gameController;

    [Header("Enemy Action")]
    [SerializeField] private EnemyAction enemyAction = EnemyAction.walk;
    [SerializeField] private GameObject dieAnimation;
    public static event Action<int> OnEnemyDie;
    [SerializeField] private int enemyScore = 100;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float bounceForce = 2f;
    private float direction = 1;

    [Header("Timers")]
    [SerializeField] private float moveTime = 5f;
    [SerializeField] private float stopMove = 1.5f;

    [Header("Layers checks")]
    [SerializeField] private LayerMask groundLayer;

    #pragma warning disable // Could be null!
    [SerializeField] private Transform groundCheck;

    #pragma warning disable // Could be null!
    [SerializeField] private Vector2 groundCheckSize;
    
    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool moveComplete;
    private PlayerController player;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        player = FindObjectOfType<PlayerController>();

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
    }

    public void Die(Rigidbody2D playerRb)
    {
        playerRb.velocity = new Vector2(playerRb.velocity.x, this.GetBounceForce());

        Destroy(gameObject);

        Instantiate(dieAnimation, transform.position, Quaternion.identity);

        OnEnemyDie.Invoke(enemyScore);

        gameController.ShowEarnedScore(enemyScore, transform);
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

            rb.velocity = new Vector2(direction * moveSpeed, jumpForce);

            animator.SetTrigger("Jump");

            yield return new WaitForSeconds(stopMove);

            SetDirection(direction * -1);

            rb.velocity = new Vector2(direction * moveSpeed, jumpForce);

            animator.SetTrigger("Jump");

            moveComplete = true;
        }
    }

    private bool isGrounded()
    {
        if(Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer)) return true;
        else return false;
    }

    public void SetDirection(float direction)
    {
        this.direction = direction;
    }

    public float GetBounceForce()
    {
        return this.bounceForce;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
