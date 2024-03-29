using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class OpossumBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] checkers;
    [SerializeField] private Vector2 checkSize;
    private EnemyController enemyController;
    private Rigidbody2D opossumRb;
    private Transform checkInUse;
    void Start()
    {
        enemyController = GetComponent<EnemyController>();

        opossumRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        checkInUse = enemyController.GetUsedChecker(checkers);

        if(WallDetected() || !GroundDetected())
            ChangeDirection();
    }

    public void Move()
    {
        enemyController.ApplyVelocity(opossumRb);
    }

    public void ChangeDirection()
    {
        enemyController.ChangeEnemyDirection();
    }

    /// <summary>
    /// Uses the Enemy Controller's wall detector to swap enemy's direction.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool WallDetected()
    {
        return enemyController.WallDetected(checkInUse, checkSize, enemyController.GetGroundLayer());
    }

    /// <summary>
    /// Uses the Enemy Controller's ground detector to swap enemy's direction.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool GroundDetected()
    {
        return enemyController.GroundDetected(checkInUse, enemyController.GetGroundLayer(), 2f);
    }
}
