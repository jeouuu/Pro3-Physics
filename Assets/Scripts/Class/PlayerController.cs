using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Properties")]
    public float moveSpeed;
    public float accTime;
    public float decTime;

    private Vector2 moveInput;
    private Rigidbody2D playerRB;


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
    }

    private void FixedUpdate()
    {
        MovementUpdate(moveInput);
    }

    private void MovementUpdate(Vector2 _moveInput)
    {
        float targetSpeed = _moveInput.x * moveSpeed;
        float acc = moveSpeed / accTime;
        float dec = moveSpeed / decTime;

        float accRate = 0; ;
        if(_moveInput.x != 0)
        {
            accRate = acc;
        }else if(_moveInput.x == 0)
        {
            accRate = dec;
        }

        float speedDiff = targetSpeed - playerRB.linearVelocityX;
        Vector2 force = new Vector2( speedDiff * accRate * playerRB.mass , 0f);
        playerRB.AddForce(force);
    }

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
