using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] [Range(5f, 20f)] private float rangeOfVision;
    [SerializeField] [Range(1f, 10f)] private float diveSpeed = 5f;
    [SerializeField] [Range(1f, 10f)] private float recoverySpeed = 3f;
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
        attackDirection = NewAttackDirection();

        animator.SetBool("IsAttacking", isAttacking);

        if(!isAttacking && !resetPosition) // only when the eagle is flying will try to find the player again.
        {
            enemyController.SetDirection(NewDirection()); 

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
    /// Detects the nearby ground and triggers the StaringAtPlayer coroutine.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void IsCloseToTheGround()
    {
        if(!resetPosition)
        {
            if(Physics2D.Raycast(transform.position, Vector2.down, 1.5f, enemyController.GetGroundLayer()))
                resetPosition = true;
        }
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
     /// This method will set the eagle's direction based on where the player it's located.
     /// </summary>
     /// <returns></returns>
    private float NewDirection()
    {
        float direction;

        if(GameObject.FindGameObjectWithTag("Player").transform.position.x - transform.position.x < 0)
            direction = -1;
        else
            direction = 1;

        return direction;
    }

    /// <summary>
    /// This method will set the eagle's attack direction based where is he facing at.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Vector2 NewAttackDirection()
    {
        Vector2 newAttackDirection;

        if(enemyController.GetDirection() < 0)
            newAttackDirection = new Vector2(enemyController.GetDirection(), enemyController.GetDirection());
        else
           newAttackDirection = new Vector2(enemyController.GetDirection(), -enemyController.GetDirection());

        return newAttackDirection;
    }
}
