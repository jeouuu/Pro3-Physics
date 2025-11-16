using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Properties")]
    public float moveSpeed;
    public float accTime;
    public float decTime;

    public bool use1 ;
    public bool useUpdate ;

    private Vector2 moveInput;
    private Rigidbody2D playerRB;
    private Vector2 vel = Vector2.zero;


    public enum FacingDirection
    {
        left, right
    }

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
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
    }

    #region Player Movement
    private void MoveWithTransform()
    {
        float acc = moveSpeed / accTime;
        float dec = moveSpeed / decTime;
        if (moveInput.x != 0)
        {
            vel += acc * Time.deltaTime * new Vector2(moveInput.x, 0);
        } else if (moveInput.x == 0)
        {
            vel -= dec * Time.deltaTime * vel.normalized;
        }

        vel = Vector3.ClampMagnitude(vel, moveSpeed);
        transform.position += (Vector3)vel * Time.deltaTime;
    }

    private void MovementUpdate(Vector2 _moveInput)
    {
        // method 1
        if (use1)
        {
            float targetSpeed = _moveInput.x * moveSpeed;
            float acc = moveSpeed / accTime;
            float dec = moveSpeed / decTime;

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
            float acc = moveSpeed / accTime;
            float dec = moveSpeed / decTime;

            if (_moveInput.x != 0)
            {
                vel += acc * Time.fixedDeltaTime * new Vector2(_moveInput.x, 0);
            } else if (_moveInput.x == 0)
            {
                vel -= dec * Time.fixedDeltaTime * vel.normalized;
            }

            vel = Vector2.ClampMagnitude(vel, moveSpeed);
            playerRB.linearVelocity = vel;
        }
    }
    #endregion

    private void GetPlayerInput()
    {
        moveInput = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public bool IsWalking()
    {
        return false;
    }
    public bool IsGrounded()
    {
        return false;
    }

    public FacingDirection GetFacingDirection()
    {
        return FacingDirection.left;
    }
}
