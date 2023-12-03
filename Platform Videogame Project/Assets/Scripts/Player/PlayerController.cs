using Unity.VisualScripting;
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

    [SerializeField]
    private LayerMask groundLayer;
    
    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private Vector2 groundCheckSize;

    private void Update()
    {
        animator.SetFloat("Horizontal", movement);

        animator.SetFloat("Speed", Mathf.Abs(movement));

        animator.SetFloat("Fall", rb.velocity.y);
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
        if(IsGrounded()) // when we add this piece of code the little jump stop working, find out what's happening and fix it up!
        {
            if(jump.performed)
            {
                // normal jump

                rb.velocity = new Vector2(transform.position.x, jumpForce);
            }
            else
            {   
                if(jump.canceled)
                {
                    // little jump

                    rb.velocity = new Vector2(transform.position.x, rb.velocity.y * .5f);
                }
            }

            animator.SetTrigger("Jump");
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer); 
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
