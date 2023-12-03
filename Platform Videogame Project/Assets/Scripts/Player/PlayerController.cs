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
    private float moveSpeed = 3f;

    [SerializeField]
    private float runSpeed = 6f;
    private bool isRunning = false;

    [SerializeField]
    private float crouchSpeed = 2f;
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

    [Header("Crouch")]

    [SerializeField]
    private BoxCollider2D crouchCol;
    private bool isCrouching = false;

    private void Update()
    {
        animator.SetFloat("Horizontal", movement);

        animator.SetFloat("Speed", Mathf.Abs(movement));

        animator.SetFloat("Fall", rb.velocity.y);
    }

    private void FixedUpdate()
    {
        if(isRunning)
            rb.velocity = new Vector2(movement * runSpeed, rb.velocity.y);
        else if(isCrouching)
            rb.velocity = new Vector2(movement * crouchSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(movement * moveSpeed, rb.velocity.y);
    }

    public void Walk(InputAction.CallbackContext walk)
    {
        movement = walk.ReadValue<Vector2>().x;
    }

    public void Run(InputAction.CallbackContext run)
    {
        if(run.performed) // if we running we flip the run boolean which by default it's false.
            isRunning = !isRunning;
        else
        {
            if(run.canceled) // when we stop running we flip again the boolean now from true to false.
                isRunning = !isRunning;
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
    public void Crouch(InputAction.CallbackContext crouch)
    {
        if(crouch.performed) // if we're crouching
        {
            // we are crouching now

            isCrouching = !isCrouching;

            // disable the head collider

            crouchCol.enabled = false;

            // play crouch animation
        }
        else if(crouch.canceled) // if we do not
        {
            isCrouching = !isCrouching;
        }
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
