using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public enum FacingDirection
    {
        left, right
    }
    public enum PlayerState
    {
        idle, walking, jumping, doubleJumping, falling, sliding, dead
    }

    public static PlayerState currentState = PlayerState.idle;

    [Header("Player Movement Properties")]
    public float maxSpeed;
    private Vector2 velocity;
    private FacingDirection currentDir = FacingDirection.right;

    [Header("Acc/Dec")]
    public float accTime;
    public float decTime;
    public float turnAccMultiplier;
    private float acc;
    private float dec;

    [Header("Player Jump Properties")]
    public float apexHeight;
    public float apexTime;
    public float maxFallingSpeed;
    public float jumpCutGravityMult;
    public float fastFallGravityMult;
    public int maxJumps;
    private float gravity;
    private float initialJumpVel;
    private int usedJumps;
    private bool isJumping;
    private bool isJumpCutting;
    private bool isDoubleJumping;

    [Header("Wall Slide/Jump Properties")]
    public float wallRaycastLength;
    public float slideSpeed;
    public float slideAccMult;

    private float lastOnWallTime;
    private float onRightWallTime;
    private float onLeftWallTime;
    private bool isWallJumping = false;
    private bool isSliding;

    [Header("Collision Properties")]
    public float groundBoxCastLength = 1;
    public LayerMask groundLayer;

    [Header("Timer Properties")]
    public float coyoteTime;
    public float jumpBufferTime;
    private float coyoteCounter;
    private float jumpBufferCounter;

    [Header("Other Reference")]
    public Transform spawnPoint;

    // Input Var // 
    private Vector2 moveInput;
    private bool jumpWasPressed;
    private bool jumpWasRelease;

    // Get Components Vars //
    private Rigidbody2D playerRB;
    private Collider2D playerColl;

    // Condition Vars //
    private bool touchHazard = false;
    #endregion

    void Start()
    {
        // GetComponent
        playerRB = GetComponent<Rigidbody2D>();
        playerColl = GetComponent<Collider2D>();

        // Calculate acc/dec
        acc = maxSpeed / accTime;
        dec = maxSpeed / decTime;

        // Calculate gravity and initial jump velocity
        gravity = -(2 * apexHeight) / (Mathf.Pow(apexTime, 2));
        initialJumpVel = 2 * apexHeight / apexTime;
        playerRB.gravityScale = 0;
    }
    void Update()
    {     
        ReadInput();
        JumpCheck();
        TimerUpdate();
        UpdatePlayerState();
    }
    private void FixedUpdate()
    {
        WallCheckCollision();

        // Jump Action
        if (CanJump())
        {
            PlayerJump(); 
            jumpWasPressed = false;
        }     

        // Wall Slide
        if(isSliding)
        {
            Debug.Log("Wall Sliding");
            PlayerWallSlide();
        }

        // Apply the modified gravity last (after jump logics)
        SetGravity();

        // Walk Action
        PlayerMovement();
    }


    #region Player Horizontal Movement
    private void PlayerMovement()
    {
        if(touchHazard)
        {
            playerRB.linearVelocityX = 0;
            return;
        }

        // If there's a move input, do the move logics
        if (moveInput.x != 0)
        {
            float accToUse = acc;

            // If we are turning, tune the acc with the turnAccMult
            if (Mathf.Sign(moveInput.x) != Mathf.Sign(velocity.x)) accToUse *= turnAccMultiplier;   

            velocity.x += accToUse * moveInput.x * Time.fixedDeltaTime;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }

        // Else if there's no move input, and the player's velocity is still larger than the threshold: do the decelerate logic
        // the threshold means in floating-point math, player almost never land on exact zero
        else if ( Mathf.Abs(velocity.x) > 0.005f)
        {
            velocity.x += dec * -Mathf.Sign(velocity.x) * Time.fixedDeltaTime;
        } 
        
        // when it is close enough to zero, snap it to zero
        else
        {
            velocity.x = 0;
        }

        playerRB.linearVelocityX = velocity.x;
    }
    #endregion

    #region Player Vertical Movement
    private void PlayerJump()
    {
        // Set Jump Condition
        isJumping = true;

        // Jump Input/Buffer Reset
        jumpBufferCounter = 0;

        // Add the usedJump
        usedJumps++;

        playerRB.linearVelocityY = initialJumpVel;
    }

    private void PlayerWallSlide()
    {
        Debug.Log("Sliding");   

        //We remove the remaining upwards velocity to prevent upwards sliding (when we still jumping, but already touch the wall, get rid of the upward force)
        if (playerRB.linearVelocityY > 0)
        {
            playerRB.AddForce(-playerRB.linearVelocityY * Vector2.up, ForceMode2D.Impulse);
        }

        // Calculate the speedDiff and force
        float speedDiff = slideSpeed - playerRB.linearVelocityY;
        float force = speedDiff * acc * slideAccMult ;

        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        force = Mathf.Clamp(force, -Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime));

        // Apply to the body
        playerRB.AddForce(force * Vector2.up);
    }
    private void SetGravity()
    {
        float gravityToUse = gravity;

        // Apply JumpCut gravity
        if (isJumpCutting)
        {
            gravityToUse = gravity * jumpCutGravityMult;
        }

        // Apply DoubleJump Gravity
        if(isDoubleJumping)
        {
            gravityToUse = gravity * fastFallGravityMult;
        }

        playerRB.linearVelocityY += gravityToUse * Time.fixedDeltaTime;

        // Clamp the fallingSpeed
        if (playerRB.linearVelocityY < -maxFallingSpeed)
        {
            playerRB.linearVelocityY = -maxFallingSpeed;
        }
    }
    #endregion

    #region Player State Updates
    private void UpdatePlayerState()
    {
        if (touchHazard)
        {
            currentState = PlayerState.dead;
            return;
        }

        // first check if grounded
        if (IsGrounded())
        {
            if (IsWalking())
            {
                currentState = PlayerState.walking;
            }
            else
            {
                currentState = PlayerState.idle;
            }
        }

        // if we are not grounded, then we are in air   
        else
        {
            if (playerRB.linearVelocityY > 0)
            {
                currentState = PlayerState.jumping;
            }
            else if (isDoubleJumping)
            {
                currentState = PlayerState.doubleJumping;
            }
            else if (playerRB.linearVelocityY < 0 && !touchHazard)
            {

                currentState = PlayerState.falling;
            }
            else if (isSliding)
            {
                currentState = PlayerState.sliding;
            }
        }
    }
    private void JumpCheck()
    {
        // JUMP BUFFER
        if (jumpWasPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // JUMP CUT
        if (jumpWasRelease)
        {
            if (playerRB.linearVelocityY > 0)
            {
                // if the player vel.y is bigger than 0 means still rising.So these 2 condition check: if player release the jump button when still rising...
                isJumpCutting = true;
            }
        }
        else if (IsGrounded())
        {
            // only set it back to false when player grounded, else the jump cut is only true for 1 frame
            isJumpCutting = false;
        }

        // Double Jumps
        if (IsGrounded())
        {
            usedJumps = 0;
        }
        if (usedJumps == maxJumps)
        {
            isDoubleJumping = true;
        }
        else isDoubleJumping = false;

        // Wall Slide
        if (CanSlide() && ((onLeftWallTime > 0 && moveInput.x < 0) || (onRightWallTime > 0 && moveInput.x > 0)))
        {
            Debug.Log("Start Sliding cuz pressing the direction as wall");
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        // Basic Jump Check
        if (isJumping && playerRB.linearVelocityY < 0)  // when the player is falling down
        {
            isJumping = false;
        }
    }
    private bool CanJump()
    {
        // First Jump
        if(usedJumps == 0)
        {
            return coyoteCounter > 0 && jumpBufferCounter > 0;
        } 
        
        // Double Jump
        else
        {
            return usedJumps < maxJumps && jumpWasPressed;
        }
    }
    private bool CanSlide()
    {
        // return true when: on a wall & not jumping & not wall jumping & not grounded
        if (lastOnWallTime > 0 && !isJumping && !isWallJumping && !IsGrounded())
        {
            Debug.Log("On Wall ");
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Timer Updates
    private void TimerUpdate()
    {
        // coyote count
        if (IsGrounded())
        {
            coyoteCounter = coyoteTime;
        } else 
        {
            coyoteCounter -= Time.deltaTime;
        }

        // buffer count
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    #endregion

    #region Player Input
    private void ReadInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if (Input.GetButtonDown("Jump"))
        {
            jumpWasPressed = true;
        }
        jumpWasRelease = Input.GetButtonUp("Jump");
    }   
    #endregion

    #region Condition Check
    public bool IsWalking()
    {
        if(moveInput.x != 0)
        {
            return true;
        } else {
            return false;
        }  
    }
    public bool IsGrounded()
    {
        Vector2 boxOrigin = playerColl.bounds.center;
        Vector2 boxSize = new Vector2(playerColl.bounds.size.x * 0.5f, playerColl.bounds.size.y); // Slightly smaller than the player's 

        RaycastHit2D groundHit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, groundBoxCastLength, groundLayer);
        
        if(groundHit.collider != null)
        {
            return true;
        } else
        {
            return false;
        }
    }
    private void WallCheckCollision()
    {
        Vector2 raycastOrigin = playerColl.bounds.center;

        RaycastHit2D wallHitRight = Physics2D.Raycast(raycastOrigin, Vector2.right, wallRaycastLength, groundLayer);
        RaycastHit2D wallHitLeft = Physics2D.Raycast(raycastOrigin, Vector2.left, wallRaycastLength, groundLayer);

        // Right Wall Check
        // The condition check: Am I touching a wall to my right? && not wall jumping
        if (wallHitRight.collider != null && !isWallJumping)
        {
            onRightWallTime = coyoteTime;
        }

        // Left Wall Check
        // Same as above
        if (wallHitLeft.collider != null && !isWallJumping)
        {
            onLeftWallTime = coyoteTime;
        }
         
        lastOnWallTime = Mathf.Max(onLeftWallTime, onRightWallTime);
    }
    public FacingDirection GetFacingDirection()
    {
        if (moveInput.x > 0)
        {
            currentDir = FacingDirection.right;
        } else if (moveInput.x < 0)
        {
            currentDir = FacingDirection.left;
        }
        return currentDir;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Hazard")
        {
            //Debug.Log("Touch Hazard");
            touchHazard = true;
        }
    }
    #endregion

    #region Animation Events
    public void RespawnPlayer()
    {
        touchHazard = false;
        transform.position = spawnPoint.position;
    }
    #endregion

}
