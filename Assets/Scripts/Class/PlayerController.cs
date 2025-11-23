using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Properties")]
    public float maxSpeed;
    public float accTime;
    public float decTime;
    public float jumpForce;
    public bool use1 = true;

    private Vector2 vel = Vector2.zero;
    private FacingDirection currentDir = FacingDirection.right;

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

    // Input Var
    private Vector2 moveInput;
    private bool jumpWasPressed;

    // Get Components Vars
    private Rigidbody2D playerRB;
    private Collider2D playerColl;

    public enum FacingDirection
    {
        left, right
    }

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerColl = GetComponent<Collider2D>();

        playerRB.gravityScale = 0;
        gravity = (-2 * apexHeight) / (apexTime * apexTime);
        initialJumpVelocity = (2 * apexHeight) / apexTime;
    }
    void Update()
    {     
        GetPlayerInput();
        TimerUpdate();
        JumpCheck();
    }
    private void FixedUpdate()
    {
        SetGravity();

        // move action
        MovementUpdate(moveInput);

        // jump action
        if (CanJump())  
        {
            PlayerJump();
        }
    }


    #region Player Movement
    private void MoveWithTransform()
    {
        float acc = maxSpeed / accTime;
        float dec = maxSpeed / decTime;
        if (moveInput.x != 0)
        {
            vel += acc * Time.deltaTime * new Vector2(moveInput.x, 0);
        } else if (moveInput.x == 0 && playerRB.linearVelocity != Vector2.zero)
        {
            vel -= dec * Time.deltaTime * vel.normalized;
        }

        vel = Vector3.ClampMagnitude(vel, maxSpeed);
        transform.position += (Vector3)vel * Time.deltaTime;
    }
    private void MovementUpdate(Vector2 _moveInput)
    {
        // method 1
        if (use1)
        {
            float targetSpeed = _moveInput.x * maxSpeed;
            float acc = maxSpeed / accTime;
            float dec = maxSpeed / decTime;

            float accRate = 0; ;
            if (_moveInput.x != 0)
            {
                accRate = acc;
            } else if (_moveInput.x == 0)
            {
                accRate = dec;
            }

            float speedDiff = targetSpeed - playerRB.linearVelocityX;

            Vector2 force = new Vector2(speedDiff * accRate * playerRB.mass, 0f);
            playerRB.AddForce(force);
        }


        //method 2
        else if (!use1)
        {
            Vector2 vel = playerRB.linearVelocity;
            float acc = maxSpeed / accTime;
            float dec = maxSpeed / decTime;

            if (_moveInput.x != 0)
            {
                vel += acc * Time.fixedDeltaTime * new Vector2(_moveInput.x, 0);
            } else if (_moveInput.x == 0)
            {
                vel -= dec * Time.fixedDeltaTime * vel.normalized;
            }

            vel = Vector2.ClampMagnitude(vel, maxSpeed);
            playerRB.linearVelocity = vel;
        }
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
    private void GetPlayerInput()
    {
        moveInput = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Jump"))
        {
            jumpWasPressed = true;
        } else
        {
            jumpWasPressed = false;
        }
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
    #endregion


}
