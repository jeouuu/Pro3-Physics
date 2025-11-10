using UnityEngine;

public class J5OwlScript : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // rotate with arrow keys
        float rotate = Input.GetAxis("Horizontal");
        transform.Rotate(0, 0, -rotate * 100f * Time.deltaTime);

        // move forward with relative force
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(Vector2.up * 1f);
        }
    }
}

