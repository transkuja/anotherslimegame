using XInputDotNetPure;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;


public delegate void ptrStateFct();
public class PlayerState  {

    protected ptrStateFct curUpdateFct;
    protected ptrStateFct curFixedUpdateFct;
    protected PlayerController playerController;
    #region getterSetters

    public ptrStateFct CurActionFct
    {
        get
        {
            return curUpdateFct;
        }

        protected set
        {
            curUpdateFct = value;
        }
    }

    public ptrStateFct CurFixedUpdateFct
    {
        get
        {
            return curFixedUpdateFct;
        }
        set
        {
            curFixedUpdateFct = value;
        }
    }

    #endregion
    public PlayerState(PlayerController _playerController)
    {
        playerController = _playerController;
    }
    public virtual void OnBegin()
    {
    }
    public virtual void OnEnd()
    {
    }
   
    public virtual void OnUpdate()
    {
        if (curUpdateFct != null)
            curUpdateFct();
    }
    public virtual void OnFixedUpdate()
    {
        if (CurFixedUpdateFct != null)
            CurFixedUpdateFct();
    }
  
    public virtual void DrawGizmo()
    {

    }
    // Fonction utiles pour tout état : 
    // peut être à deplacer dans un composant qui gère les evolutions.

   
    public virtual void HandleMovementWithController()
    {
        Vector3 initialVelocity = HandleSpeedWithController();
     
        Vector3 camVectorForward = new Vector3(playerController.Player.cameraReference.transform.GetChild(0).forward.x, 0.0f, playerController.Player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward + Vector3.up * playerController.Player.Rb.velocity.y;
        if (playerController.IsGrounded)
            velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right;

        playerController.Player.Rb.velocity = velocityVec;
        playerController.transform.LookAt(playerController.transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z) + initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right);

        // TMP Animation
        playerController.Player.Anim.SetFloat("MouvementSpeed", Mathf.Abs(playerController.State.ThumbSticks.Left.X) > Mathf.Abs(playerController.State.ThumbSticks.Left.Y) ? Mathf.Abs(playerController.State.ThumbSticks.Left.X) : Mathf.Abs(playerController.State.ThumbSticks.Left.Y));
        playerController.Player.Anim.SetBool("isWalking", ((Mathf.Abs(playerController.State.ThumbSticks.Left.X) > 0.02f) || Mathf.Abs(playerController.State.ThumbSticks.Left.Y) > 0.02f) && playerController.Player.GetComponent<PlayerController>().IsGrounded);
    }
    public virtual Vector3 HandleSpeedWithController()
    {
        Vector3 initialVelocity = new Vector3(playerController.State.ThumbSticks.Left.X, 0.0f, playerController.State.ThumbSticks.Left.Y);
        initialVelocity.Normalize();
       
        initialVelocity *= playerController.stats.Get(Stats.StatType.GROUND_SPEED);
        if (Mathf.Abs(playerController.State.ThumbSticks.Left.X) + Mathf.Abs(playerController.State.ThumbSticks.Left.Y) < 0.95f)
        {
            initialVelocity /= 2;
        }
        return initialVelocity;
    }
    
    public virtual void Dash()
    {

    }
   
    public virtual void HandleGravity()
    {
        if (playerController.isGravityEnabled)
        {
            float gravity = playerController.GetComponent<JumpManager>().GetGravity();
            playerController.Player.Rb.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
        }
    }
    public virtual void OnJumpPressed()
    {
        if (playerController.jumpState.nbJumpMade < playerController.stats.Get(Stats.StatType.JUMP_NB))
            playerController.PlayerState = playerController.jumpState;
    }
    public virtual void OnDashPressed()
    {
        playerController.PlayerState = playerController.dashState;
    }
}
