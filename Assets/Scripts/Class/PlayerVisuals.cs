using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;


    void Update()
    {

        // Switch for Player's facing direction //
        switch (playerController.GetFacingDirection())
        {
            case PlayerController.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerController.FacingDirection.right:
                bodyRenderer.flipX = false;
                break;
        }

        // Switch for Player's state //
        switch(PlayerController.currentState)
        {
            case PlayerController.PlayerState.idle:
                animator.Play("Idle");
                break;
            case PlayerController.PlayerState.walking:
                animator.Play("Walk");
                break;
            case PlayerController.PlayerState.jumping:
                animator.Play("Jump");
                break;
            case PlayerController.PlayerState.doubleJumping:
                animator.Play("DoubleJump");
                break;
            case PlayerController.PlayerState.falling:
                animator.Play("Fall");
                break;
            case PlayerController.PlayerState.sliding:
                animator.Play("Slide");
                break;
            case PlayerController.PlayerState.dead:
                animator.Play("Death"); 
                break;
        }


    }
}
