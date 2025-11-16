using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Properties")]
    public float maxSpeed;
    public float accTime;
    public float decTime;
    public float jumpForce;
    public bool use1;
    public bool useUpdate ;

    private Vector2 vel = Vector2.zero;
    private FacingDirection currentDir = FacingDirection.right;

    [Header("Detection Properties")]
    public float groundBoxCastLength = 1;
    public LayerMask groundLayer;

    // Input Var
    private Vector2 moveInput;
    private bool jumpWasPressed;

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
    }

    void Update()
    {
        GetPlayerInput();
        
        if (useUpdate)
        {
            MoveWithTransform();
        }
    }

    private void FixedUpdate()
    {
        if (!useUpdate)
        {
            MovementUpdate(moveInput);
        }

        if (jumpWasPressed && IsGrounded())
        {
            PlayerJump();
            jumpWasPressed = false;
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
        } else if (moveInput.x == 0)
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
        playerRB.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }
    #endregion

    #region Player Input
    private void GetPlayerInput()
    {
        moveInput = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Jump"))
        {
            jumpWasPressed = true;
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
    #endregion

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
}
