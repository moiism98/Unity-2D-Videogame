using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Physics and gravity")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AnimationCurve gravityCurve;
    private float maxFallSpeed = 20;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    private float direction = 1;
    private bool isRunning = false;
    [SerializeField] private float crouchSpeed = 2f;
    private float movement;    

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;
    private int health;
    private bool isHurt = false;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float doubleJumpForce = 2.5f;
    private int totalJumps = 2;
    private int jumps;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;

    [Header("Crouch")]
    [SerializeField] private Collider2D crouchCol;
    private bool isCrouching = false;
    private bool buttonPressed = false;
    [SerializeField] private Transform ceilingCheck;
    [SerializeField] private LayerMask ceilingLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float ceilingCheckRadius = .10f;
    private bool isjumpingUnderPlatform = false;

    [Header("Climb")]
    private bool isClimbing = false;
    [SerializeField] [Range(0f, 1f)] private float climbWallCheckRadius;

    [Header("Shoot")]
    [SerializeField] private GameObject arrowPrefab;

    [Header("Slopes materials")]
    [SerializeField] private PhysicsMaterial2D noFriction;
    [SerializeField] private PhysicsMaterial2D antiSliding;

    [Header("Particles effect")]
    [SerializeField] private ParticleSystem runParticles;
    [SerializeField] private ParticleSystem jumpParticles;
    private bool activateLandingParticles = false;
    private Transform spawnPoint;
    private GameController gameController;
    private AudioManager audioManager;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();

        audioManager = FindObjectOfType<AudioManager>();

        health = maxHealth;
    }

    private void Update()
    {
        animator.SetFloat("Horizontal", direction);

        animator.SetFloat("Speed", Mathf.Abs(movement));

        animator.SetFloat("Fall", rb.velocity.y);

        animator.SetBool("Climbing", isClimbing);

        animator.SetBool("IsHurt", isHurt);

        ResetCrouch(); // this method reset the crouch animation and action when the crouch button is not pressed!

        CanJump(); // check every frame if we can jump or not.

        CapPlayerHealth();
    }

    private void FixedUpdate()
    {
        if(isRunning && !isCrouching && !isClimbing)
            rb.velocity = new Vector2(movement * runSpeed, rb.velocity.y);
        else if(isCrouching && !isClimbing)
            rb.velocity = new Vector2(movement * crouchSpeed, rb.velocity.y);
        else if(isClimbing)
            rb.velocity = new Vector2(rb.velocity.x, movement * moveSpeed);
        else
            rb.velocity = new Vector2(movement * moveSpeed, rb.velocity.y);

        NoSlidingOnSlopes(); // method which applies a friction if we are stoped slopes

        if(!isClimbing)
        {
            CalculateGravity(); // method which calculates the gravity when player falls
        
            EnablePlayerCols();
        }

        if(isHurt)
            rb.AddForce(new Vector2(-direction * jumpForce, Mathf.Abs(direction) * 1.5f), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Makes the player move. (Input Action)
    /// </summary>
    /// <param name="walk"></param>
    public void Walk(InputAction.CallbackContext walk)
    {
        if(!isClimbing)
        {
            SetMovement(walk.ReadValue<Vector2>().x);

            if(walk.performed)
                direction = walk.ReadValue<Vector2>().x;
        }
    }

    /// <summary>
    /// Makes the player run. (Input Action)
    /// </summary>
    /// <param name="run"></param>
    public void Run(InputAction.CallbackContext run)
    {
        if(gameController.gameMode.Equals(GameMode.bonus))
        {
            if(run.performed) // if we running we flip the run boolean which by default it's false.
            {
                if(Mathf.Abs(movement) > 0 && !isCrouching)
                    PlayRunParticles();

                isRunning = !isRunning;
            }
            else if(run.canceled) // when we stop running we flip again the boolean now from true to false.
            {
                isRunning = !isRunning;
            
                StopRunParticles();
            }
        }
    }

    /// <summary>
    /// Makes the player jump and double jump. (Input Action)
    /// </summary>
    /// <param name="jump"></param>
    public void Jump(InputAction.CallbackContext jump)
    {
        if(gameController.gameMode.Equals(GameMode.bonus))
        {
            if(jumps < totalJumps && !isCrouching) // we only can jump if we have jumps remaining and we are not crouching !
            {
                if(jump.performed && !isHurt) // normal jump
                    ApplyJumpForce(jumpForce);
                else if(jump.canceled)
                    ApplyJumpForce(doubleJumpForce);

                // if we are in the air, we are prepare to play the landing particles

                activateLandingParticles = !activateLandingParticles;     
            }
        }
    }

    /// <summary>
    /// Makes the player crouch. (Input Action)
    /// </summary>
    /// <param name="crouch"></param>
    public void Crouch(InputAction.CallbackContext crouch)
    {
        if(!isClimbing && !isHurt) // if we are not climbing or hurts we can crouch
        {
            if(crouch.performed) // if we're pressing the crouch button.
            {
                isCrouching = true; // we are now crouching.

                buttonPressed = true; // save the button state.

                crouchCol.enabled = false; // disable the top collider.
            }
            else  // if we don't
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

                    buttonPressed = false; // but change this value because we've released the button.
                }
            }
        }
    }

    /// <summary>
    /// Allows the player jump under platforms. (Input Action)
    /// </summary>
    /// <param name="underPlatform"></param>
    public void UnderPlatform(InputAction.CallbackContext underPlatform)
    {
        /*
            we have to add the "tap" interaction to the input action because we are using the same buttons we use for crouching!

            and also we have to use "performed" instead of "canceled" because if we choose this option we can not crouch:
            if we release the button the player will pass through the platform whenever we stop crouching, we do not want that, only when we tap the button!
        */
        
        if(underPlatform.performed) 
        {
            if(AbovePlatform()) // we check if we are above a platform so we can pass through it.
                StartCoroutine(JumpUnderPlatform());
        }
    }

    /// <summary>
    /// Move the player vertically to climb. (Input action)
    /// After we had pressed the game action.
    /// </summary>
    /// <param name="climb"></param>
    public void Climb(InputAction.CallbackContext climb)
    {
        
        if(isClimbing)
            SetMovement(climb.ReadValue<Vector2>().y);
    }
    private void EnablePlayerCols()
    {
        CapsuleCollider2D[] playerColliders = gameObject.GetComponents<CapsuleCollider2D>();

        if(!isCrouching && !isjumpingUnderPlatform)
        {
            foreach(CapsuleCollider2D playerCollider in playerColliders)
                playerCollider.enabled = true;
        }
    }

    public void DisablePlayerCols()
    {
        CapsuleCollider2D[] playerColliders = gameObject.GetComponents<CapsuleCollider2D>();

        foreach(CapsuleCollider2D playerCollider in playerColliders)
            playerCollider.enabled = false;
    }

    private void ApplyJumpForce(float jumpForce)
    {
        rb.velocity = new Vector2(transform.position.x, jumpForce);

        animator.SetTrigger("Jump");

        StopRunParticles();

        jumps++;

        if(isClimbing) // jumping we disable the climb action!
            isClimbing = !isClimbing;
    }

    private void CanJump() // check every frame if we can jump or not.
    {
        bool ground = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);

        if(ground) // if we touch the ground reset the jumps number again. 
        {
            jumps = 0;
        
            if(activateLandingParticles) // when we touch the ground activates the particles and reset the state so it's not playing every frame
            {
                PlayJumpParticles();

                activateLandingParticles = !activateLandingParticles;
            }
        }
    }

    private void NoSlidingOnSlopes()
    {
        if(Mathf.Abs(movement) > 0) // if we are moving we apply no friction on our player
            rb.sharedMaterial = noFriction;
        else
            rb.sharedMaterial = antiSliding; // if we don't, we apply high friction on our player to stop the sliding effect (he 'sticks' on the ground)
    }

    private void CalculateGravity()
    {
        if(rb.velocity.y < maxFallSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed)); // this cap our fall speed at our maxFallSpeed

            // we use the animation curve to set player's gravity based on our fall speed so our gravity it's not static on jumps

            rb.gravityScale = gravityCurve.Evaluate(rb.velocity.y);
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

        if(!isClimbing)
        {
            animator.SetBool("Crouch", isCrouching);

            _ = isjumpingUnderPlatform == true ? isCrouching = false : isCrouching = true;

            if(!buttonPressed) // if the crouch button it's not pressed, whe are not under ceiling and not trying to jump under a platform.
            {
                if(!isjumpingUnderPlatform)
                {
                    if(!CeilingAbove())
                    {
                        crouchCol.enabled = true; // we activates the top collider.

                        isCrouching = false; // we are no longer crouching.
                    }
                }
            }
        }
    }


    /// <summary>
    /// Disables the player's colliders to pass through the platforms.
    /// Enables them again after a bit amount of time.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpUnderPlatform() 
    {

        // disable the player's capsule collider (his body), we do not want enemies pass under the platforms!

        CapsuleCollider2D [] playerColliders = gameObject.GetComponents<CapsuleCollider2D>();

        foreach(CapsuleCollider2D col in playerColliders)
            col.enabled = false;

        isjumpingUnderPlatform = true;

        yield return new WaitForSeconds(.25f); // wait an amount of secs

        // reactivate the player colliders

        foreach(CapsuleCollider2D col in playerColliders)
            col.enabled = true;

        isjumpingUnderPlatform = false;

    }

    /// <summary>
    /// Makes the player shoot an arrow. (Input Action)
    /// </summary>
    /// <param name="shoot"></param>
    public void Shoot(InputAction.CallbackContext shoot)
    {
        if(shoot.performed)
        {
            if(GameController.isArrowReady)
                ShootArrow();
        }
    }

    /// <summary>
    /// It will cap the player's health to its max health.
    /// </summary>
    private void CapPlayerHealth()
    {
        if(this.GetHealth() >= this.GetMaxHealth())
            this.SetHealth(this.GetMaxHealth());
    }

    /// <summary>
    /// Hurts the player and decrease its health based on the damage taken.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public IEnumerator TakeDamage(int damage)
    {
        if(!isHurt)
        {
            isHurt = true;

            health -= damage;

            gameController.UpdatePlayerHealth(this.GetHealth()); // change the player's health UI based on his current health

            if(health <= 0) Die();

            yield return new WaitForSeconds(.25f);

            isHurt = false;
        }
    }

    /// <summary>
    /// Displays the game over menu when the player dies.
    /// </summary>
    private void Die()
    {
        gameController.ShowGameOverMenu();
    }

    /// <summary>
    /// Spawns an arrow and throw it where the player is facing at.
    /// </summary>
    private void ShootArrow()
    {
        GameController.isArrowReady = false;

        Vector2 shootPoint = new Vector2(transform.position.x, transform.position.y - .5f);
        
        Instantiate(arrowPrefab, shootPoint, arrowPrefab.transform.rotation);

        audioManager.PlaySound("Arrow");
    }

    /// <summary>
    /// Displays the run particles when the player is running.
    /// </summary>
    private void PlayRunParticles()
    {
        runParticles.Play();
    }

    /// <summary>
    /// Hides the run particles when the player stops running.
    /// </summary>
    private void StopRunParticles()
    {
        runParticles.Stop();
    }

    /// <summary>
    /// Displays the run particles when the player hits on the ground after jumping.
    /// </summary>
    private void PlayJumpParticles()
    {
        jumpParticles.Play();
    }

    public void SetIsClimbing(bool isClimbing)
    {
        this.isClimbing = isClimbing;
    }

    public void SetMovement(float movement)
    {
        this.movement = movement;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public bool GetIsClimbing()
    {
        return this.isClimbing;
    }

    public Rigidbody2D GetRigidbody2D()
    {
        return this.rb;
    }

    public float GetDirection()
    {
        return this.direction;
    }

    public int GetMaxHealth()
    {
        return this.maxHealth;
    }

    public int GetHealth()
    {
        return this.health;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);

        Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, climbWallCheckRadius);
    }
}
