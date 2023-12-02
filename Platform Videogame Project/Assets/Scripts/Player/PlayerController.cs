using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Animator animator;

    [Header("Movement")]
    [SerializeField]
    private float speed = 3f;
    private float movement;    

    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 5f;
    private void Update()
    {
        animator.SetFloat("Horizontal", movement);

        animator.SetFloat("Speed", Mathf.Abs(movement));
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movement * speed, rb.velocity.y);
    }

    public void Walk(InputAction.CallbackContext walk)
    {
        movement = walk.ReadValue<Vector2>().x;
    }

    public void Run(InputAction.CallbackContext run)
    {
        if(run.performed)
            SetSpeed(speed * 2);
        else
        {
            if(run.canceled)
                SetSpeed(speed * .5f);
        }
    }

    public void Jump(InputAction.CallbackContext jump)
    {
        if(jump.performed)
        {
            // normal jump
            
            rb.velocity = new Vector2(transform.position.x,  jumpForce);
        }
        else
        {   
            if(jump.canceled)
            {
                // little jump

                rb.velocity = new Vector2(transform.position.x, rb.velocity.y * .5f);
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
