using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;    

    private Rigidbody2D myBody;
    private SpriteRenderer mySprite;
    private float h;

    void Start()
    {
        myBody = GetComponent<Rigidbody2D>();
        mySprite = GetComponent<SpriteRenderer>();  
    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");

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
    }

    private void PlayerMovement(float hori)
    {
        Vector2 dir = new Vector2(hori, 0);
        myBody.linearVelocity = dir * moveSpeed;
    }


}
