using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private float direction;
    private Rigidbody2D rb;
    private Animator animator;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        direction = FindObjectOfType<PlayerController>().GetDirection();

        Destroy(gameObject, 15f);
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", direction);
    }
    
    void FixedUpdate()
    {
        rb.velocity = Vector2.right * direction * speed; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();

        if(enemy != null)
        {
            Destroy(gameObject);

            enemy.Die();
        }
    }
}
