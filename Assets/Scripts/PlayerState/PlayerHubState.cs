using UWPAndXInput;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Cinemachine;


public delegate void ptrStateFct();
public class PlayerHubState : PlayerState3DDefault {

    float dragForce = 0.025f;
    float dragForceDash = 0.01f; // Dash value

    float airControlFactor = 0.75f;
    float timerApplyDrag = 0.02f;


    public PlayerHubState(PlayerControllerHub _playerController) : base(_playerController)
    {
        playerController = _playerController;
    }
   
    public virtual Vector3 HandleSpeedWithController()
    {
        Vector3 initialVelocity = new Vector3(playerController.State.ThumbSticks.Left.X, 0.0f, playerController.State.ThumbSticks.Left.Y);
        initialVelocity.Normalize();
       
        initialVelocity *= playerController.stats.Get(Stats.StatType.GROUND_SPEED);
        if (Utils.Abs(playerController.State.ThumbSticks.Left.X) + Utils.Abs(playerController.State.ThumbSticks.Left.Y) < 0.95f)
        {
            initialVelocity /= 2;
        }

        return initialVelocity;
    }

    public override void Move(Vector3 initialVelocity)
    {
        Vector3 camVectorForward = new Vector3(playerController.Player.cameraReference.transform.GetChild(0).forward.x, 0.0f, playerController.Player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        if (initialVelocity.magnitude > 1f)
        {
            if (!playerController.forceCameraRecenter)
            {
                Vector3 velocityVec = initialVelocity.z * camVectorForward + Vector3.up * playerController.Player.Rb.velocity.y;
                // MENU peter a cause de cette condition tu sais pourquoi c'est la antho ? sinon je peux faire une exception pour le menu
                if (!playerController.IsGrounded && playerController.jumpState.nbJumpMade == 2)
                    velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right * airControlFactor;
                else
                    velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right;

                playerController.Player.Rb.velocity = velocityVec;
                playerController.transform.LookAt(playerController.transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z) + initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right);
            }
        }
        else
        {
            Vector3 tmp = new Vector3(playerController.Player.Rb.velocity.x, 0.0f, playerController.Player.Rb.velocity.z);
            Vector3 fwd = playerController.transform.forward;

            float dragForceUsed = dragForce;//(playerController.PreviousPlayerState == playerController.dashState) ? dragForceDash : dragForce;

            if (tmp.sqrMagnitude > 7.0f)// && Vector3.Dot(playerController.transform.forward, tmp) > 0)
            {
                if ((tmp.x > 0 && playerController.Player.Rb.velocity.x - tmp.x * fwd.x * dragForceUsed < 0)
                || (tmp.x < 0 && playerController.Player.Rb.velocity.x - tmp.x * fwd.x * dragForceUsed > 0)
                || (tmp.z > 0 && playerController.Player.Rb.velocity.z - tmp.z * fwd.z * dragForceUsed < 0)
                || (tmp.z < 0 && playerController.Player.Rb.velocity.z - tmp.z * fwd.z * dragForceUsed > 0))
                    playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
                else
                    playerController.Player.Rb.velocity -= (tmp.normalized * dragForceUsed * tmp.sqrMagnitude) * ((Time.deltaTime/ timerApplyDrag));

                tmp = new Vector3(playerController.Player.Rb.velocity.x, 0.0f, playerController.Player.Rb.velocity.z);

                if (Vector3.Dot(fwd, tmp) < 0)
                    playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;

            }
            else
            {
                playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
            }
        }

    }


    public virtual void OnJumpPressed()
    {
        if(playerController.wallJumpState.WallJumpTest())
        {
            playerController.PlayerState = playerController.wallJumpState;
        }
        else if (playerController.jumpState.nbJumpMade < playerController.stats.Get(Stats.StatType.JUMP_NB))
        {
            playerController.PlayerState = playerController.jumpState;
        }
    }

    public virtual void PushPlayer(Vector3 force)
    {
        playerController.PlayerState = playerController.expulsedState;
        playerController.Rb.velocity = new Vector3(force.x, playerController.Rb.velocity.y, force.z);
    }
    public virtual void HandleControllerAnim()
    {
        playerController.Player.Anim.SetFloat("MouvementSpeed", Utils.Abs(playerController.State.ThumbSticks.Left.X) > Utils.Abs(playerController.State.ThumbSticks.Left.Y) ? Utils.Abs(playerController.State.ThumbSticks.Left.X) +0.2f : Utils.Abs(playerController.State.ThumbSticks.Left.Y)+0.2f);
        playerController.Player.Anim.SetBool("isWalking", ((Utils.Abs(playerController.State.ThumbSticks.Left.X) > 0.02f) || Utils.Abs(playerController.State.ThumbSticks.Left.Y) > 0.02f) && playerController.IsGrounded);
    }
}
