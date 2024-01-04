using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float incrementSpeed = 0.01f;
    [SerializeField] private CompositeCollider2D tilemapCol;
    [SerializeField] private LayerMask playerLayer;
    private bool playerDead = false;
    private bool isIncresingSpeed = false;

    private void Update()
    {
        KillPlayer();

        StartCoroutine(IncrementSpikesSpeed());
    }
    
    private void FixedUpdate()
    {
        if(!playerDead)
            rb.velocity = Vector2.up * speed;  
    }

    private void KillPlayer()
    {
        if(tilemapCol.IsTouchingLayers(playerLayer))
        {
            PlayerController player = FindObjectOfType<PlayerController>();

            StartCoroutine(player.TakeDamage(player.GetMaxHealth()));

            // delete this when death screen it's done

            playerDead = true;

            rb.velocity = Vector2.zero;
        }
    }

    private IEnumerator IncrementSpikesSpeed()
    {   
        if(!isIncresingSpeed)
        {
            isIncresingSpeed = true;

            speed += incrementSpeed;

            yield return new WaitForSeconds(7);

            isIncresingSpeed = false;
        }
    }
}
