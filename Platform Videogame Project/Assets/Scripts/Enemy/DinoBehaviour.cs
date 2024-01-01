using System;
using System.Collections;
using UnityEngine;

public class DinoBehaviour : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float fieldOfVision = 3f;
    [SerializeField] private Transform[] checkers;
    [SerializeField] private Vector2 checkerSize;
    private EnemyController enemyController;
    private Rigidbody2D dinoRb;
    private Transform checkerInUse;
    private LayerMask checkerLayer;
    private bool playerDetected = false;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();

        dinoRb = GetComponent<Rigidbody2D>();

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
        checkerInUse = enemyController.GetUsedChecker(checkers);
        
        if(checkerInUse != null && Physics2D.Raycast(checkerInUse.position, new Vector2(enemyController.GetDirection(), 0f), fieldOfVision, playerLayer))
            SetPlayerDetected(true);
    }
    
    public void Move()
    {
        if(playerDetected)
        {
            dinoRb.velocity = new Vector2(enemyController.GetDirection() * enemyController.GetMoveSpeed(), dinoRb.velocity.y);

            // check if there is a fall or wall forward

            if(enemyController.WallDetected(checkerInUse, checkerSize, checkerLayer) || !enemyController.GroundDetected(checkerInUse, checkerLayer)) // if we do:
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
