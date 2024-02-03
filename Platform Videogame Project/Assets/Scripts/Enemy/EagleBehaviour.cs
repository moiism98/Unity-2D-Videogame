using System.Collections;
using UnityEngine;

public class EagleBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] [Range(5f, 20f)] private float rangeOfVision;
    [SerializeField] [Range(1f, 10f)] private float diveSpeed = 5f;
    [SerializeField] [Range(1f, 10f)] private float recoverySpeed = 3f;
    [SerializeField] [Range(1f, 5f)] private float groundDetectorDistance = 1.25f;
    private bool isAttacking = false;
    private bool resetPosition = false;
    private Vector3 attackDirection;
    private EnemyController enemyController;
    private Rigidbody2D eagleRb;
    private Animator animator;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();

        eagleRb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        animator.SetBool("IsAttacking", isAttacking);

        if(!isAttacking && !resetPosition) // only when the eagle is flying will try to find the player again.
        {
            enemyController.SetDirection(EagleDirection()); 

            if(FoundPlayer())
                isAttacking = true;
        }
        
        IsCloseToTheGround();
    }

    private void FixedUpdate()
    {
        if(isAttacking && !resetPosition)
            Attack();
        else if(resetPosition)
            StartCoroutine(StaringAtPlayer());
    }

    /// <summary>
    /// It's called every frame and will detect if the player is on the eagle's field of vision.
    /// On true returned will trigger the eagle's attack.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool FoundPlayer()
    {
        bool playerFound = false;

        if(Physics2D.Raycast(transform.position, attackDirection, rangeOfVision, playerLayer))
            playerFound = true;

        return playerFound;
    }

    /// <summary>
    /// Detects if there is ground nearby to stop eagle's attack by triggering the StaringAtPlayer coroutine.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void IsCloseToTheGround()
    {
        if(!resetPosition)
            resetPosition = enemyController.GroundDetected(transform, enemyController.GetGroundLayer(), groundDetectorDistance);
    }

    /// <summary>
    /// Moves the eagle towards his attack direction.
    /// </summary>
    private void Attack()
    {
        eagleRb.velocity = attackDirection * diveSpeed;
    }


    /// <summary>
    /// Routine which reset the eagle behaviour: puts the eagle back to the start position and make it stop attacking.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator StaringAtPlayer()
    {
        if(isAttacking)
        {
            eagleRb.velocity = Vector2.zero;

            isAttacking = false;

            yield return new WaitForSeconds(.35f);

            eagleRb.velocity = attackDirection * -1 * recoverySpeed;

            yield return new WaitForSeconds(recoverySpeed * .5f);

            eagleRb.velocity = Vector2.zero;

            resetPosition = false;
        }
    }
     /// <summary>
     /// This method will set the eagle's direction and attack direction based on where the player it's located.
     /// </summary>
     /// <returns></returns>
    private float EagleDirection()
    {
        float eagleDirection;

        if(GameObject.FindGameObjectWithTag("Player").transform.position.x - transform.position.x < 0)
            eagleDirection = -1;
        else
            eagleDirection = 1;

        // eagleDirection (x) = -1 -> attack left
        // eagleDirection (x) = 1 -> attack right

        attackDirection = new Vector2(eagleDirection, -1); 

        return eagleDirection;
    }
}
