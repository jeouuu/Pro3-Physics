using UnityEngine;

public class GunshipController : MonoBehaviour
{
    [SerializeField] private Transform leftCannon;
    [SerializeField] private Transform rightCannon;

    public GameObject cannonballPrefab;
    public float cannonballForce = 150f;

    private bool fireLeftCannon = false; 
    private bool fireRightCannon = false;

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 leftDirection = AimCannon(mousePosition, leftCannon);
        Vector3 rightDirection = AimCannon(mousePosition, rightCannon);

        if (Input.GetMouseButtonDown(0))
        {
           fireLeftCannon = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
           fireRightCannon = true;
        }

    }

    void FixedUpdate()
    {
        if (fireLeftCannon)
        {
            FireCannonball(AimCannon(Camera.main.ScreenToWorldPoint(Input.mousePosition), leftCannon), leftCannon);
            fireLeftCannon = false;
        }
        if (fireRightCannon)
        {
            FireCannonball(AimCannon(Camera.main.ScreenToWorldPoint(Input.mousePosition), rightCannon), rightCannon);
            fireRightCannon = false;
        }
    }

    private Vector3 AimCannon(Vector3 target, Transform cannon)
    {
        Vector3 direction = target - cannon.position;
        direction.z = 0;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cannon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        return direction;
    }
    private void FireCannonball(Vector3 direction, Transform cannon)
    {
        GameObject leftCannonBall = Instantiate(cannonballPrefab, cannon.position, Quaternion.identity);
        Rigidbody2D rbLeft = leftCannonBall.GetComponent<Rigidbody2D>();
        rbLeft.AddForce(direction.normalized * cannonballForce, ForceMode2D.Force);
    }
}
