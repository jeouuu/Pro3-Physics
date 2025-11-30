using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Player Movement Properties")]
    public float maxSpeed;
    private Vector2 velocity;

    //private FacingDirection currentDir = FacingDirection.right;

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
    private float gravity;
    private float initialJumpVelocity;

    [Header("Detection Properties")]
    public float groundBoxCastLength = 1;
    public LayerMask groundLayer;

    [Header("Timer Properties")]
    public float coyoteTime;
    public float jumpBufferTime;
    private float coyoteCounter;
    private float jumpBufferCounter;

    // Input Var // 
    private Vector2 moveInput;
    private bool jumpWasPressed;

    // Get Components Vars
    private Rigidbody2D playerRB;
    private Collider2D playerColl;

    public enum FacingDirection
    {
        left, right
    }
    #endregion

    void Start()
    {
        // GetComponent
        playerRB = GetComponent<Rigidbody2D>();
        playerColl = GetComponent<Collider2D>();

        // Calculate acc/dec
        acc = maxSpeed / accTime;
        dec = maxSpeed / decTime;

    }
    void Update()
    {     
        ReadInput();
    }
    private void FixedUpdate()
    {
        PlayerMovement();
    }


    #region Player Movement
    private void PlayerMovement()
    {
        // If there's a move input, do the move logics
        if(moveInput.x != 0)
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

        playerRB.linearVelocity = velocity;
    }
    private void PlayerJump()
    {
        //reset bufferCount
        jumpBufferCounter = 0;

        playerRB.linearVelocityY = initialJumpVelocity;
    }
    private void SetGravity()
    {
        playerRB.linearVelocityY += gravity * Time.fixedDeltaTime;

        if (playerRB.linearVelocityY < -maxFallingSpeed)
        {
            playerRB.linearVelocityY = -maxFallingSpeed;
        }
    }
    #endregion

    #region Player State Updates
    private void JumpCheck()
    {
        if (jumpWasPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
    }
    private bool CanJump()
    {
        // return true if: the coyote timer is bigger than 0
        return coyoteCounter > 0 && jumpBufferCounter > 0;
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
        if(jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

    }
    #endregion

    #region Player Input
    private void ReadInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        jumpWasPressed = Input.GetButtonDown("Jump");
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
        Vector2 boxSize = playerColl.bounds.size;

        RaycastHit2D groundHit = Physics2D.BoxCast(boxOrigin, boxSize, 0f, Vector2.down, groundBoxCastLength, groundLayer);
        
        if(groundHit.collider != null)
        {
            return true;
        } else
        {
            return false;
        }
    }
    //public FacingDirection GetFacingDirection()
    //{
    //    //if (moveInput.x > 0)
    //    //{
    //    //    currentDir = FacingDirection.right;
    //    //} else if (moveInput.x < 0)
    //    //{
    //    //    currentDir = FacingDirection.left;
    //    //}
    //    //return currentDir;
    //}
    #endregion


}
