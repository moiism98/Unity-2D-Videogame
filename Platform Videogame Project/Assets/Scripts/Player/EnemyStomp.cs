using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStomp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("HitPoint"))
        {
            EnemyController enemy = collision.GetComponentInParent<EnemyController>();

            Rigidbody2D playerRb = GetComponentInParent<Rigidbody2D>();

            if(enemy != null && playerRb != null)
                enemy.Die(playerRb);
        }
    }
}
