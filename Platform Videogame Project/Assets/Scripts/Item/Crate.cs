using System.Collections;
using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private Vector2 playerdetectorSize = new Vector2(.5f, .5f);
    [SerializeField] private Vector2 enemydetectorSize = new Vector2(.5f, .5f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask enemyLayer;
    private bool playerNearby = false;

    private void Update()
    {
        if(PlayerNearby())
            HitEnemy();
    }

    private bool PlayerNearby()
    {
        if(Physics2D.OverlapBox(transform.position, playerdetectorSize, 0, playerLayer))
            playerNearby = true;
        else
            StartCoroutine(NotPlayerNearby());
        
        return playerNearby;
    }

    /// <summary>
    /// Delay a bit the playerNearby boolean in case we push away crate and we are not touching it but we want to kill an enemy.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator NotPlayerNearby()
    {
        yield return new WaitForSeconds(1.5f); // without this delay we can not kill an enemy by pushing the crate forward, we should be pushing it everytime if we wanted to kill an enemy.

        playerNearby = false;
    }

    private void HitEnemy()
    {
        Collider2D collision = Physics2D.OverlapBox(transform.position, enemydetectorSize, 0, enemyLayer);
        
        if(collision != null)
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();

            enemyController?.Die();

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(transform.position, playerdetectorSize);

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, enemydetectorSize);
    }
}
