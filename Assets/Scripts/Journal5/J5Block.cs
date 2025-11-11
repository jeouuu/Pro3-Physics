using Unity.VisualScripting;
using UnityEngine;

public class J5Block : MonoBehaviour
{
    private GameObject player;
    private Player playerScript;
    private Rigidbody2D blockBody;

    private bool qPressed;
    private bool ePressed;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        blockBody = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            qPressed = true;
        } else { qPressed = false; }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ePressed = true;
        } else { ePressed = false; }
    }

    private void FixedUpdate()
    {
        if (Player.hitBlock)
        {
            Player.hitBlock = false;
            Vector2 force = new Vector2(Mathf.Sign(playerScript.h), 0) * playerScript.playerPushForce;
            Vector2 position = (Vector2)transform.position + Vector2.up * 1f;
            //blockBody.AddForce(force, ForceMode2D.Impulse);
            blockBody.AddForceAtPosition(force, position, ForceMode2D.Impulse);
        }

        #region Example 2
        //// Press Q = centered push  
        //if (qPressed)
        //{
        //    blockBody.AddForce(Vector2.left * 5f, ForceMode2D.Impulse);
        //}

        //// Press E = off-center push
        //if (ePressed)
        //{
        //    Vector2 position = (Vector2)transform.position + Vector2.up * 1f; // top edge
        //    blockBody.AddForceAtPosition(Vector2.right * 1f, position, ForceMode2D.Impulse);
        //}
        #endregion
    }
}

