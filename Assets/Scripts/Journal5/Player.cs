using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;    
    public float jumpForce= 5f; 

    private Rigidbody2D myBody;
    private SpriteRenderer mySprite;
    private float h;
    private bool jump;
    private bool onGround;

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }


        if (h < 0)
        {
            mySprite.flipX = true;
        }
        else if (h > 0)
        {
            mySprite.flipX = false;
        }
    }
    private void FixedUpdate()
    {
        PlayerMovement(h);
        if (jump && onGround)
        {
            PlayerJump();
            jump = false;
        }

    }

    private void PlayerMovement(float hori)
    {
        Vector2 dir = Vector2.right * hori;
        myBody.linearVelocityX = dir.x * moveSpeed;
    }

    private void PlayerJump()
    {
        myBody.AddForce(Vector2.up * jumpForce , ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }


}
