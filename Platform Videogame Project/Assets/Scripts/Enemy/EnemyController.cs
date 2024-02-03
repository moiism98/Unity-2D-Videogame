using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameController gameController;

    [Header("Enemy Action")]
    [SerializeField] private EnemyAction enemyAction = EnemyAction.dino;
    public static event Action<int> OnEnemyDie;
    [SerializeField] private int enemyScore = 100;
    private int damage = 1;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float bounceForce = 15f;
    private float direction;
    [SerializeField] private bool fliped = false;

    [Header("Enemy Death")]
    [SerializeField] private GameObject dieAnimation;
    [SerializeField] GameObject hitPoint;
    [SerializeField] GameObject heartPrefab;
    [SerializeField] [Range(0, 100)] private int dropProbability;

    [Header("Enemy Attack")]
    #region 
    [HideInInspector] public Transform damagePoint;
    [HideInInspector] public DamageCollider damageCollider = DamageCollider.circle;

        #region
        [HideInInspector] public float damagePointRadius = .25f;
        #endregion

        #region
        [HideInInspector] public CapsuleDirection2D capsuleDirection = CapsuleDirection2D.Horizontal;
        [HideInInspector] public Vector2 capsuleSize;
        #endregion

        #region
        [HideInInspector] public Vector2 boxSize;
        #endregion

    #endregion

    [Header("Layers checks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    [Header("Animations")]
    [SerializeField] private Animator animator;
    private PlayerController player;
    private AudioManager audioManager;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        player = FindObjectOfType<PlayerController>();

        audioManager = FindObjectOfType<AudioManager>();

        SetStartDirection();
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", direction);

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        HitPlayer();   
    }

    private void FixedUpdate()
    {
        MoveEnemy();
    }

    private void MoveEnemy()
    {
        switch(enemyAction)
        {
            case EnemyAction.dino: GetComponent<DinoBehaviour>().Move(); break;

            case EnemyAction.frog: StartCoroutine(GetComponent<FrogBehaviour>().Move()); break;

            case EnemyAction.opossum: GetComponent<OpossumBehaviour>().Move(); break;
        }
    }

    /// <summary>
    /// This method will be triggered when the player kills an enemy by jumping on it.
    /// We need the player's rigidbody to apply a bounce force on him.
    /// </summary>
    /// <param name="playerRb"></param>
    public void Die(Rigidbody2D playerRb)
    {
        playerRb.velocity = new Vector2(playerRb.velocity.x, this.GetBounceForce());

        Destroy(gameObject);

        Instantiate(dieAnimation, transform.position, Quaternion.identity);

        audioManager.PlaySound("Enemy Dead");

        DropHeart();

        OnEnemyDie.Invoke(enemyScore);

        gameController.ShowEarnedScore(enemyScore, transform);
    }

    /// <summary>
    /// This method will be triggered whenever the player kills an enemy with arrows or crates.
    /// </summary>
    public void Die()
    {
        Destroy(gameObject);

        Instantiate(dieAnimation, transform.position, Quaternion.identity);

        audioManager.PlaySound("Enemy Death");

        DropHeart();

        OnEnemyDie.Invoke(enemyScore);

        gameController.ShowEarnedScore(enemyScore, transform);
    }

    /// <summary>
    /// Calculate a heart spawn probability on enemy death.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void DropHeart()
    {
        int randomProbability = UnityEngine.Random.Range(0, 101);

        if(randomProbability <= dropProbability)
        {
            Vector2 dropSpawnPoint = new Vector2(transform.position.x, transform.position.y - .3f);
        
            Instantiate(heartPrefab, dropSpawnPoint, Quaternion.identity, GameObject.FindGameObjectWithTag("Level").transform);
        }
    }

    /// <summary>
    /// This method will take damage to the player whenever an enemy hits it.
    /// It's going to trigger a coroutine, the player has an invulnerability time.
    /// </summary>
    private void HitPlayer()
    {
        Collider2D player = null;

        switch(damageCollider) // each enemy has his own damage collider based on its shape.
        {
            case DamageCollider.circle: player = Physics2D.OverlapCircle(transform.position, damagePointRadius, playerLayer); break;
            case DamageCollider.capsule: player = Physics2D.OverlapCapsule(transform.position, capsuleSize, capsuleDirection, 0f, playerLayer); break;
            case DamageCollider.box: player = Physics2D.OverlapBox(transform.position, boxSize, playerLayer); break;
        }
        

        if(player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();

            if(playerController != null)
                StartCoroutine(playerController.TakeDamage(damage));
        }
    }

    /// <summary>
    /// Apply a velocity on the enemy to make it move.
    /// </summary>
    /// <param name="rb"></param>
    public void ApplyVelocity(Rigidbody2D rb)
    {
        rb.velocity = new Vector2(GetDirection() * GetMoveSpeed(), 0);
    }

    /// <summary>
    /// Apply a velocity on the enemy to make it move. This method is design for enemies which can jump.
    /// </summary>
    /// <param name="rb"></param>
    /// <param name="jumpForce"></param>
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
        bool wall = false;

        if(checker != null && Physics2D.OverlapBox(checker.position, checkerSize, 0f, checkLayer))
            wall = true;
        
        return wall;
    }

    /// <summary>
    /// Detects ground to reset enemies behaviour. This functions only activates when the enemy it's on platforms.
    /// </summary>
    /// <returns></returns>
    public bool GroundDetected(Transform checker, LayerMask checkLayer, float distance)
    {
        bool ground = false;

        if(checker != null && Physics2D.Raycast(checker.position, Vector2.down, distance, checkLayer))
            ground = true;

        return ground;
    }

    /// <summary>
    /// Set walls and falls player's detector point using the dinosaur's corresponding direction. 
    /// </summary>
    /// <returns></returns>
    public Transform GetUsedChecker(Transform[] checkers)
    {
        Transform checker;

        checker = Array.Find(checkers, chk => MathF.Truncate(transform.position.x - chk.transform.position.x).Equals(this.GetDirection() * -1));
        
        return checker;
    }

    public void ChangeEnemyDirection()
    {
        SetDirection(GetDirection() * -1);
    }

    private void SetStartDirection()
    {
        if(fliped) 
            this.SetDirection(1);
        else 
            this.SetDirection(-1);
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

    public DamageCollider GetDamageCollider()
    {
        return this.damageCollider;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        switch(damageCollider)
        {
            case DamageCollider.circle: Gizmos.DrawWireSphere(damagePoint.position, damagePointRadius); break;
            case DamageCollider.capsule: Gizmos.DrawWireCube(damagePoint.position, capsuleSize); break;
            case DamageCollider.box: Gizmos.DrawWireCube(damagePoint.position, boxSize); break;
        }
        
    }
}
