using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private Vector2 playerdetectorSize = new Vector2(.5f, .5f);
    [SerializeField] private Vector2 enemydetectorSize = new Vector2(.5f, .5f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        if(PlayerNearby())
            HitEnemy();
    }

    private bool PlayerNearby()
    {
        bool playerNearby = false;

        if(Physics2D.OverlapBox(transform.position, playerdetectorSize, 0, playerLayer))
            playerNearby = true;
        
        return playerNearby;
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
