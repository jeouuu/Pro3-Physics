using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody2D myBody;
    private SpriteRenderer mySprite;
    public float h;
    private bool jump;
    private bool onGround;
   
    private bool onPurpleGround;
    private float t = 0f;

    public static bool hitBlock = false;
    public float playerPushForce = 2;

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        #region Get Input
        h = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (h < 0)
        {
            mySprite.flipX = true;
        } else if (h > 0)
        {
            mySprite.flipX = false;
        }
        #endregion

        #region Purple Ground Logic
        if (onPurpleGround)
        {
            t += Time.deltaTime;
            myBody.constraints = RigidbodyConstraints2D.FreezePosition;

            if (t >= 1)
            {
                myBody.constraints = RigidbodyConstraints2D.None;
                myBody.freezeRotation = true;
            }
        } else if (!onPurpleGround)
        {
            t = 0f;
        }
        #endregion

        
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

        if(collision.gameObject.name == "Block")
        {
            hitBlock = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Purple")
        {
            onPurpleGround = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }

        if (collision.gameObject.name == "Purple")
        {
            onPurpleGround = false;
        }
    }


}
