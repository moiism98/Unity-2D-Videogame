using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

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
    private Collider2D crouchCol;
    private bool isCrouching = false;
    private bool buttonPressed = false;

    [SerializeField]
    private Transform ceilingCheck;

    [SerializeField]
    private LayerMask ceilingLayer;

    [SerializeField]
    private LayerMask platformLayer;

    [SerializeField]
    private float ceilingCheckRadius = .10f;

    private GameObject platform;

    private void Update()
    {
        animator.SetFloat("Horizontal", movement);

        animator.SetFloat("Speed", Mathf.Abs(movement));

        animator.SetFloat("Fall", rb.velocity.y);

        animator.SetBool("Crouch", isCrouching);
        
        ResetCrouch(); // this method reset the crouching when the crouch button is not pressed and we are not under ceilings.
    }

    private void FixedUpdate()
    {
        if(isRunning && !isCrouching)
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
            if(!isCrouching)
            {
                if(jump.performed) // normal jump
                    rb.velocity = new Vector2(transform.position.x, jumpForce);
                /*else
                {   
                    if(jump.canceled) // little jump
                        rb.velocity = new Vector2(transform.position.x, rb.velocity.y * .5f); 
                }*/

                animator.SetTrigger("Jump");
            }
            else if(AbovePlatform())
                StartCoroutine(JumpUnderPlatform());
        }
    }
    public void Crouch(InputAction.CallbackContext crouch)
    {

        if(crouch.performed) // if we're pressing the crouch button.
        {
            isCrouching = true; // we are now crouching.

            buttonPressed = true; // save the button state. PRESSING IT

            crouchCol.enabled = false; // disable the top collider.
        }
        else  // if we do not
        {
            if(crouch.canceled) // when we release the crouch button.
            {
                if(!CeilingAbove()) // check if detect ceiling above, if not, we stay crouching.
                {
                    isCrouching = false; // we are no longer crouching.

                    crouchCol.enabled = true; // enable the top collider.
                }

                buttonPressed = false; // but change this value because we've released the button. NOT PRESSING IT
            }
        }
    }

    private bool IsGrounded() // detects if we are touching the ground
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer); 
    }

    private bool AbovePlatform() // detects if we are touching a platform
    {
        Collider2D onPlatform = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, platformLayer);

        if(onPlatform != null)
        {
            platform = onPlatform.gameObject;

            return true;
        }

        return false;
    }

    private bool CeilingAbove() // detects if we are under ceilings
    {
        return Physics2D.OverlapCircle(transform.position, ceilingCheckRadius, ceilingLayer);
    }

    private void ResetCrouch()
    {
        if(!buttonPressed && !CeilingAbove()) // if the crouch button it's not pressed and whe are not under ceiling.
        {
            crouchCol.enabled = true; // activates the top collider.

            isCrouching = false; // we are no longer crouching.
        }
    }

    private IEnumerator JumpUnderPlatform() 
    {
        platform.GetComponent<TilemapCollider2D>().enabled = false; // disable platorm collider.

        isCrouching = false; // also the crouch animator, we are falling now.

        yield return new WaitForSeconds(.35f); // wait an amount of secs

        platform.GetComponent<TilemapCollider2D>().enabled = true; // and enable the platform collider again.
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
    }
}
