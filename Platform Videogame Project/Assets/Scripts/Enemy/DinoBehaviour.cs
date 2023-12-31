using System;
using System.Collections;
using UnityEngine;

public class DinoBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float fieldOfVision = 3f;

    private EnemyController enemyController;
    [SerializeField] private Transform[] checkers;
    [SerializeField] private Vector2 checkerSize;
    private Transform checkerInUse;
    private LayerMask checkerLayer;
    private bool playerDetected = false;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();

        GetCheckers();
    }

    private void Update()
    {
        PlayerCheck();
    }

    private void GetCheckers()
    {
        checkerLayer = enemyController.GetGroundLayer();
    }
    /// <summary>
    /// Try to detect the player every frame. If the player is detected, the dinosaur starts moving.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void PlayerCheck()
    {
        checkerInUse = GetUsedChecker();
        
        if(Physics2D.Raycast(checkerInUse.position, new Vector2(enemyController.GetDirection(), 0f), fieldOfVision, playerLayer))
            SetPlayerDetected(true);
    }

    /// <summary>
    /// Moves the dinosaur.
    /// </summary>
    public void Move()
    {
        if(playerDetected)
        {
            Rigidbody2D dinoRb = enemyController.GetRigidbody2D();

            dinoRb.velocity = new Vector2(enemyController.GetDirection() * enemyController.GetMoveSpeed(), dinoRb.velocity.y);

            // check if there is a fall or wall forward

            if(WallDetected() || !GroundDetected()) // if we do:
            {
                // stop the dino

                dinoRb.velocity = Vector2.zero;

                // change dino's direction

                enemyController.ChangeEnemyDirection();

                // reset the player check

                SetPlayerDetected(false);
            }
        }
    }

    /// <summary>
    /// Detects walls to reset dinosaur's behaviour.
    /// </summary>
    /// <returns></returns>
    private bool WallDetected()
    {
        if(Physics2D.OverlapBox(checkerInUse.position, checkerSize, 0f, checkerLayer))
            return true;
        else 
            return false;
    }

    /// <summary>
    /// Detects ground to reset dinosaur's behaviour. This functions only activates when the dinosaur it's on platforms.
    /// </summary>
    /// <returns></returns>
    private bool GroundDetected()
    {
        if(Physics2D.Raycast(checkerInUse.position, Vector2.down, 1.5f, checkerLayer))
            return true;
        else 
            return false;
    }

    /// <summary>
    /// Set walls and falls player's detector point using the dinosaur's corresponding direction. 
    /// </summary>
    /// <returns></returns>
    private Transform GetUsedChecker()
    {
        Transform checker;

        checker = Array.Find(checkers, chk => Mathf.Round(transform.position.x - chk.transform.position.x) == enemyController.GetDirection() * -1);
        
        return checker;
    }

    private void SetPlayerDetected(bool playerDetected)
    {
        this.playerDetected = playerDetected;
    }

    public bool GetPlayerDetected()
    {
        return this.playerDetected;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        foreach(Transform checker in checkers)
            Gizmos.DrawWireCube(checker.position, checkerSize);
    }
}
