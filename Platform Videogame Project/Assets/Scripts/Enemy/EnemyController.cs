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

    [Header("Layers checks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Animations")]
    [SerializeField] private Animator animator;
    private PlayerController player;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        player = FindObjectOfType<PlayerController>();

        if(fliped) 
            this.SetDirection(1);
        else 
            this.SetDirection(-1);
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", direction);

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        switch(enemyAction)
        {
            case EnemyAction.dino: GetComponent<DinoBehaviour>().Move(); break;

            case EnemyAction.frog: StartCoroutine(GetComponent<FrogBehaviour>().Move()); break;

            case EnemyAction.opossum: GetComponent<OpossumBehaviour>().Move(); break;
        }
    }

    public void Die(Rigidbody2D playerRb)
    {
        playerRb.velocity = new Vector2(playerRb.velocity.x, this.GetBounceForce());

        Destroy(gameObject);

        Instantiate(dieAnimation, transform.position, Quaternion.identity);

        OnEnemyDie.Invoke(enemyScore);

        gameController.ShowEarnedScore(enemyScore, transform);
    }

    /// <summary>
    /// Apply a velocity on the enemy to make it move.
    /// </summary>
    /// <param name="rb"></param>
    public void ApplyVelocity(Rigidbody2D rb)
    {
        rb.velocity = new Vector2(GetDirection() * GetMoveSpeed(), 0);
    }
    public void ApplyVelocity(Rigidbody2D rb, float jumpForce)
    {
        rb.velocity = new Vector2(GetDirection() * GetMoveSpeed(), jumpForce);
    }

    /// <summary>
    /// Detects walls to reset enemies behaviour.
    /// </summary>
    /// <returns></returns>
    public bool WallDetected(Transform checker, Vector2 checkerSize, LayerMask checkLayer)
    {
        if(checker != null && Physics2D.OverlapBox(checker.position, checkerSize, 0f, checkLayer))
            return true;
        else 
            return false;
    }

    /// <summary>
    /// Detects ground to reset enemies behaviour. This functions only activates when the enemy it's on platforms.
    /// </summary>
    /// <returns></returns>
    public bool GroundDetected(Transform checker, LayerMask checkLayer)
    {
        if(checker != null && Physics2D.Raycast(checker.position, Vector2.down, 2f, checkLayer))
            return true;
        else 
            return false;
    }

    /// <summary>
    /// Set walls and falls player's detector point using the dinosaur's corresponding direction. 
    /// </summary>
    /// <returns></returns>
    public Transform GetUsedChecker(Transform[] checkers)
    {
        Transform checker;

        checker = Array.Find(checkers, chk => MathF.Truncate(transform.position.x - chk.transform.position.x).Equals(this.GetDirection() * -1));

        // checker = Array.Find(checkers, chk => Mathf.Round(transform.position.x - chk.transform.position.x).Equals(this.GetDirection() * -1));
        
        return checker;
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
