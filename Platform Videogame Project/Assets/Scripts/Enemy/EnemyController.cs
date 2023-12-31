using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameController gameController;

    [Header("Enemy Action")]
    [SerializeField] private EnemyAction enemyAction = EnemyAction.dino;
    [SerializeField] private GameObject dieAnimation;
    public static event Action<int> OnEnemyDie;
    [SerializeField] private int enemyScore = 100;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float bounceForce = 15f;
    private float direction;
    [SerializeField] private bool fliped = false;

    [Header("Timers")]
    private float moveTime = 5f;
    private float stopMove = 1.5f;

    [Header("Layers checks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Animations")]
    [SerializeField] private Animator animator;
    private bool moveComplete;
    private PlayerController player;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        player = FindObjectOfType<PlayerController>();

        moveComplete = true;

        if(fliped) 
            this.SetDirection(1);
        else 
            this.SetDirection(-1);
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", direction);

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

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
            case EnemyAction.dino: GetComponent<DinoBehaviour>().Move(); break;

            case EnemyAction.frog: StartCoroutine(GetComponent<FrogBehaviour>().Move()); break;
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

    public void ChangeEnemyDirection()
    {
        SetDirection(GetDirection() * -1);
    }

    public void SetDirection(float direction)
    {
        this.direction = direction;
    }
    public Rigidbody2D GetRigidbody2D()
    {
        return this.rb;
    }

    public LayerMask GetGroundLayer()
    {
        return this.groundLayer;
    }

    public float GetDirection()
    {
        return this.direction;
    }

    public float GetBounceForce()
    {
        return this.bounceForce;
    }

    public float GetMoveSpeed()
    {
        return this.moveSpeed;
    }
}
