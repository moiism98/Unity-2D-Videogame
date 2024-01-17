using UnityEngine;

public class EnemyStomp : MonoBehaviour
{
    private PlayerController playerController;
    private Collider2D col;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        col = GetComponent<Collider2D>();
    }
    private void Update()
    {
        if(playerController.GetIsClimbing()) // if we are climbing we disable feet's collider, if not it will collide with ladder's one and stop players climb.
            col.enabled = false;
        else col.enabled = true;
    }
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
