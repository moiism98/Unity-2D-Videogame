using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private Vector2 enemydetectorSize = new Vector2(.5f, .5f);
    [SerializeField] private LayerMask enemyLayer;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HitEnemy();
    }

    /// <summary>
    /// Hits and kills the enemy when collides with it.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void HitEnemy()
    {
        if(Mathf.Abs(rb.velocity.x) > 0) // the crate only can damage the enemies when its moving.
        {
            Collider2D collision = Physics2D.OverlapBox(transform.position, enemydetectorSize, 0, enemyLayer);
            
            if(collision != null)
            {
                EnemyController enemyController = collision.GetComponent<EnemyController>();

                enemyController?.Die();

                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, enemydetectorSize);
    }
}
