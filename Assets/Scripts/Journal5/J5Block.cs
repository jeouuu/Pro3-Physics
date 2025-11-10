using Unity.VisualScripting;
using UnityEngine;

public class J5Block : MonoBehaviour
{
    private GameObject player;
    private Player playerScript;
    private Rigidbody2D blockBody;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<Player>();
        blockBody = gameObject.GetComponent<Rigidbody2D>();
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
        //// Press A = centered push
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    blockBody.AddForce(Vector2.left * 1f, ForceMode2D.Impulse);
        //}

        //// Press S = off-center push
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    Vector2 position = (Vector2)transform.position + Vector2.up * 1f; // top edge
        //    blockBody.AddForceAtPosition(Vector2.right * 1f, position, ForceMode2D.Impulse);
        //}
        #endregion
    }
}

