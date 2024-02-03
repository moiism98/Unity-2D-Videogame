using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float incrementSpeed = 0.01f;
    [SerializeField] private float speedIncreaseInterval = 7.0f;
    [SerializeField] private CompositeCollider2D tilemapCol;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask spikesLimitLayer;
    private GameController gameController;
    private bool isIncresingSpeed = false;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }
    private void Update()
    {
        KillPlayer();

        IncrementSpeed(); // this only happens at bonus levels.
    }
    
    private void FixedUpdate()
    {
        if(speed > 0)
            MoveSpikes();
    }

    private void MoveSpikes()
    {
        if(tilemapCol.IsTouchingLayers(spikesLimitLayer))
            speed = 0;
        
        rb.velocity = Vector2.up * speed;   
    }

    private void IncrementSpeed()
    {
        if(gameController.gameMode.Equals(GameMode.bonus))
            StartCoroutine(IncrementSpikesSpeed());
    }

    /// <summary>
    /// Kills the player when collides with it.
    /// </summary>
    private void KillPlayer()
    {
        if(tilemapCol.IsTouchingLayers(playerLayer))
        {
            PlayerController player = FindObjectOfType<PlayerController>();

            StartCoroutine(player.TakeDamage(player.GetMaxHealth()));
        }
    }

    /// <summary>
    /// Increments the spike wall's speed over the time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator IncrementSpikesSpeed()
    {   
        if(!isIncresingSpeed)
        {
            isIncresingSpeed = true;

            speed += incrementSpeed;

            yield return new WaitForSeconds(speedIncreaseInterval);

            isIncresingSpeed = false;
        }
    }
}
