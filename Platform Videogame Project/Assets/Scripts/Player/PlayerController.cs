using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;
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
    private float doubleJumpForce = 2.5f;
    private int totalJumps = 2;
    private int jumps;

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
    private bool isjumpingUnderPlatform = false;

    private void Update()
    {
        animator.SetFloat("Horizontal", movement);

        animator.SetFloat("Speed", Mathf.Abs(movement));

        animator.SetFloat("Fall", rb.velocity.y);

        ResetCrouch(); // this method reset the crouch animation and action when the crouch button is not pressed!

        CanJump();

        Debug.Log("Jumps: " + jumps);
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
        if(jumps < totalJumps)
        {
            if(!isCrouching) // or not crouching
            {
                if(jump.performed) // normal jump
                    ApplyJumpForce(jumpForce);
                else if(jump.canceled) // little jump
                    ApplyJumpForce(doubleJumpForce);

                animator.SetTrigger("Jump");
            }
            else if(AbovePlatform()) // if we are crouching, standing on a platform and pressing the jump button we pass through it falling into the ground.
                StartCoroutine(JumpUnderPlatform());
        }
    }

    private void ApplyJumpForce(float jumpForce)
    {
        rb.velocity = new Vector2(transform.position.x, jumpForce);

        jumps++;
    }

    private void CanJump() // check every frame if we can jump or not.
    {
        bool ground = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);

        if(ground) // if we touch the ground reset the jumps number again.
            jumps = 0;
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
                if(!isjumpingUnderPlatform) // check if we are not jumping under platform
                {
                    if(!CeilingAbove()) // if we are not above a platform, check if detect ceiling above, if not, we stay crouching.
                    {
                        isCrouching = false; // we are no longer crouching.

                        crouchCol.enabled = true; // enable the top collider.
                    }
                }

                buttonPressed = false; // but change this value because we've released the button. NOT PRESSING IT
            }
        }
    }

    private bool AbovePlatform() // detects if we are touching a platform
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, platformLayer);
    }

    private bool CeilingAbove() // detects if we are under ceilings
    {
        return Physics2D.OverlapCircle(transform.position, ceilingCheckRadius, ceilingLayer);
    }

    private void ResetCrouch()
    {
        // now our crouching animation depends also in if we are jumping over a platform or not

        // if we are not jumping over a platform our animation is still working as always, depending on buttons and ceilings. 

        // but if we are jumping over a platform we now have to reset the crouch animation because we are falling over it.

        animator.SetBool("Crouch", isCrouching);

        _ = isjumpingUnderPlatform == true ? isCrouching = false : isCrouching = true;

        if(!buttonPressed) // if the crouch button it's not pressed and whe are not under ceiling.
        {
            if(!isjumpingUnderPlatform)
            {
                if(!CeilingAbove())
                {
                    crouchCol.enabled = true; // activates the top collider.

                    isCrouching = false; // we are no longer crouching.
                }
            }
        }
    }

    private IEnumerator JumpUnderPlatform() 
    {

        // disable the player's capsule collider (his body)

        CapsuleCollider2D [] playerColliders = gameObject.GetComponents<CapsuleCollider2D>();

        foreach(CapsuleCollider2D col in playerColliders)
            col.enabled = false;

        isjumpingUnderPlatform = true;

        yield return new WaitForSeconds(.35f); // wait an amount of secs

        // reactivate the player colliders

        foreach(CapsuleCollider2D col in playerColliders)
            col.enabled = true;

        isjumpingUnderPlatform = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
    }
}
