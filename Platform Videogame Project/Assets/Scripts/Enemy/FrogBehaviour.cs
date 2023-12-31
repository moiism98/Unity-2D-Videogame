using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBehaviour : MonoBehaviour
{
    private EnemyController enemyController;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float jumpForce = 5f;
    private bool moveComplete;

    [Header("Timers")]
    [SerializeField] private float stopMove = 1.5f;
    void Start()
    {
        enemyController = GetComponent<EnemyController>();

        moveComplete = true;
    }

    public IEnumerator Move()
    {
        if(moveComplete && isGrounded())
        {
            moveComplete = false;

            Rigidbody2D frogRb = enemyController.GetRigidbody2D();

            frogRb.velocity = new Vector2(enemyController.GetDirection() * enemyController.GetMoveSpeed(), jumpForce);

            GetComponent<Animator>().SetTrigger("Jump");

            yield return new WaitForSeconds(stopMove);

            enemyController.ChangeEnemyDirection();

            frogRb.velocity = new Vector2(enemyController.GetDirection() * enemyController.GetMoveSpeed(), jumpForce);

            GetComponent<Animator>().SetTrigger("Jump");

            moveComplete = true;
        }
    }

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
