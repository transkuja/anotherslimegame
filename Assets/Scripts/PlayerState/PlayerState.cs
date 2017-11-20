using XInputDotNetPure;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Cinemachine;


public delegate void ptrStateFct();
public class PlayerState  {

    protected ptrStateFct curUpdateFct;
    protected ptrStateFct curFixedUpdateFct;
    protected PlayerController playerController;

    public float maxCoolDown = 0;
    public bool stateAvailable = true;

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
        stateAvailable = false;
        playerController.StartCoroutine(StateCooldown(maxCoolDown));
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
    public virtual void CollisionEnter(Collision collision)
    {}
    public virtual void CollisionStay(Collision collision)
    {}
    public virtual void CollisionExit(Collision collision)
    {}
  
    public virtual void DrawGizmo()
    {}
    public  IEnumerator StateCooldown(float maxCoolDown)
    {
        yield return new WaitForSeconds(maxCoolDown);
        stateAvailable = true;
        yield return null;
    }
    // Fonction utiles pour tout état : 
    // peut être à deplacer dans un composant qui gère les evolutions.
  
   
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
    public virtual void Move(Vector3 initialVelocity)
    {
        Vector3 camVectorForward = new Vector3(playerController.Player.cameraReference.transform.GetChild(0).forward.x, 0.0f, playerController.Player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward + Vector3.up * playerController.Player.Rb.velocity.y;
        if (playerController.IsGrounded)
            velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right;

        playerController.Player.Rb.velocity = velocityVec;
        playerController.transform.LookAt(playerController.transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z) + initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right);

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
        // obligé de faire un check ici( jump--> dash--> jump--> dash-->jump ...)
        if (playerController.jumpState.nbJumpMade < playerController.stats.Get(Stats.StatType.JUMP_NB))
            playerController.PlayerState = playerController.jumpState;
    }
    public virtual void OnDashPressed()
    {
        playerController.PlayerState = playerController.dashState;
    }
    public virtual void PushPlayer(Vector3 force)
    {
        playerController.PlayerState = playerController.expulsedState;
        playerController.Rb.velocity = new Vector3(force.x, playerController.Rb.velocity.y, force.z);
    }
    public virtual void HandleControllerAnim()
    {
        playerController.Player.Anim.SetFloat("MouvementSpeed", Mathf.Abs(playerController.State.ThumbSticks.Left.X) > Mathf.Abs(playerController.State.ThumbSticks.Left.Y) ? Mathf.Abs(playerController.State.ThumbSticks.Left.X) +0.2f : Mathf.Abs(playerController.State.ThumbSticks.Left.Y)+0.2f);
        playerController.Player.Anim.SetBool("isWalking", ((Mathf.Abs(playerController.State.ThumbSticks.Left.X) > 0.02f) || Mathf.Abs(playerController.State.ThumbSticks.Left.Y) > 0.02f) && playerController.IsGrounded);
    }
}
