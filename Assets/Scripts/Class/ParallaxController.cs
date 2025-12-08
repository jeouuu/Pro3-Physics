using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    public GameObject cam;
    public float parallaxSpeed;
    private float startPos;

    private void Start()
    {
        startPos = transform.position.x;
    }

    private void FixedUpdate()
    {
        float distant = cam.transform.position.x * parallaxSpeed;

        transform.position = new Vector3(startPos + distant, transform.position.y, transform.position.z);
    }

}
