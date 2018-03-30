using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ptrStateFct();
public class PlayerState {

    protected PlayerCharacterHub playerCharacterHub;
    protected PlayerControllerHub playerControllerHub;
    public bool stateAvailable = true;

    protected ptrStateFct curUpdateFct;
    protected ptrStateFct curFixedUpdateFct;
    public float maxCoolDown = 0;

    float airControlFactor = 0.42f;

    #region getterSetters
    public ptrStateFct CurActionFct
    {
        get{return curUpdateFct;}
        protected set{curUpdateFct = value;}
    }
    public ptrStateFct CurFixedUpdateFct
    {
        get{return curFixedUpdateFct;}
        set{curFixedUpdateFct = value;}
    }
    #endregion


    public PlayerState(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub )
    {
        playerCharacterHub = _playerCharacterHub;
        playerControllerHub = _playerControllerHub;
    }
    public virtual void OnBegin()
    {
    }
    public virtual void OnEnd()
    {
        stateAvailable = false;
        playerCharacterHub.StartCoroutine(StateCooldown(maxCoolDown));
    }
    public IEnumerator StateCooldown(float maxCoolDown)
    {
        yield return new WaitForSeconds(maxCoolDown);
        stateAvailable = true;
        yield return null;
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
    /// ACTIONS /////////////////////////////////////////////////////////

    public virtual Vector3 HandleSpeedWithController()
    {
        Vector3 initialVelocity = new Vector3(playerControllerHub.State.ThumbSticks.Left.X, 0.0f, playerControllerHub.State.ThumbSticks.Left.Y);
        initialVelocity.Normalize();

        initialVelocity *= playerCharacterHub.stats.Get(Stats.StatType.GROUND_SPEED);
        if (Utils.Abs(playerControllerHub.State.ThumbSticks.Left.X) + Utils.Abs(playerControllerHub.State.ThumbSticks.Left.Y) < 0.95f)
        {
            initialVelocity /= 2;
        }

        return initialVelocity;
    }

    public Vector3 GetVelocity3DThirdPerson(Vector3 initialVelocity)
    {
        Vector3 camVectorForward = new Vector3(playerControllerHub.Player.cameraReference.transform.GetChild(0).forward.x, 0.0f, playerControllerHub.Player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward;
        if (!playerCharacterHub.IsGrounded)
            velocityVec += initialVelocity.x * playerControllerHub.Player.cameraReference.transform.GetChild(0).right * airControlFactor;
        else
            velocityVec += initialVelocity.x * playerControllerHub.Player.cameraReference.transform.GetChild(0).right;

        return velocityVec;
    }
    public Vector3 GetVelocity3DSideView(Vector3 initialVelocity)
    {
        Debug.LogError("GetVelocity3DSideView Not Implemented");
        return Vector3.zero;
    }
    /// <summary>
    ///  I assume a side2d scene is still in the same orientation ( z = profondeur.)
    /// </summary>
    /// <param name="initialVelocity"></param>
    /// <returns></returns>
    public Vector3 GetVelocity2DSideView(Vector3 initialVelocity)
    {
        Vector3 velocityVec =  Vector3.up * playerCharacterHub.Rb.velocity.y;
        // MENU peter a cause de cette condition tu sais pourquoi c'est la antho ? sinon je peux faire une exception pour le menu
        if (!playerCharacterHub.IsGrounded && playerCharacterHub.jumpState.NbJumpMade == 2)
            velocityVec += initialVelocity.x * Vector3.right * airControlFactor;
        else
            velocityVec += initialVelocity.x * Vector3.right;
        return velocityVec;
    }

    public virtual void Move(Vector3 initialVelocity)
    {

        Vector3 velocityVec = Vector3.zero;
        switch (GameManager.Instance.CurrentGameMode.ViewMode)
        {
            case ViewMode.thirdPerson3d:
                velocityVec = GetVelocity3DThirdPerson(initialVelocity);
                break;
            case ViewMode.sideView3d:
                velocityVec = GetVelocity3DSideView(initialVelocity);
                break;
            case ViewMode.sideView2d:
                velocityVec = GetVelocity2DSideView(initialVelocity);
                break;
            default:
                Debug.LogError("Invalid view in GameMode");
                break;
        }

        //velocityVec.Normalize();
        if (initialVelocity.magnitude > 1f)
        {
            playerCharacterHub.Rb.drag = 0.0f;
            if (!playerControllerHub.forceCameraRecenter)
            {
                float maxForceMagnitude = playerCharacterHub.stats.Get(Stats.StatType.GROUND_SPEED);
                float maxSpeed = maxForceMagnitude;
                if (!playerCharacterHub.IsGrounded/* && playerController.jumpState.nbJumpMade == 2*/)
                {
                    maxForceMagnitude *= airControlFactor;
                }

                if (Utils.Abs(playerControllerHub.State.ThumbSticks.Left.X) + Utils.Abs(playerControllerHub.State.ThumbSticks.Left.Y) < 0.95f)
                {
                    maxForceMagnitude /= 2;
                    maxSpeed /= 2;
                }

                playerCharacterHub.Rb.AddForce(velocityVec * maxForceMagnitude*Time.deltaTime*50);
                Vector3 xzVelocity = new Vector3(playerCharacterHub.Rb.velocity.x, 0, playerCharacterHub.Rb.velocity.z);
                xzVelocity = Vector3.ClampMagnitude(xzVelocity, maxSpeed);
                playerCharacterHub.Rb.velocity = (/*(playerController.IsGrounded) ? 0 : */playerCharacterHub.Rb.velocity.y) * Vector3.up + xzVelocity;
                //playerController.transform.LookAt(playerController.transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z) + initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right);
                playerCharacterHub.transform.LookAt(playerCharacterHub.transform.position + xzVelocity);
            }
        }
        else
        {
            if (playerCharacterHub.IsGrounded)
                playerCharacterHub.Rb.drag = 15.0f;
            else
                playerCharacterHub.Rb.drag = 0.0f;
        }
    }
    public virtual void HandleGravity()
    {
        if (playerCharacterHub.IsGravityEnabled)
        {
            playerCharacterHub.Rb.AddForce(Gravity.defaultGravity * Vector3.down);
        }
    }
    public virtual void OnJumpPressed()
    {
        playerCharacterHub.Rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (playerCharacterHub.jumpState.NbJumpMade < playerCharacterHub.stats.Get(Stats.StatType.JUMP_NB))
        {
            playerCharacterHub.PlayerState = playerCharacterHub.jumpState;
        }
    }
    public virtual void OnDashPressed()
    {
        if (playerCharacterHub.dashState.nbDashMade < 1)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.dashState;
            if (playerCharacterHub.jumpState.NbJumpMade > 0)
                playerCharacterHub.dashState.nbDashMade++;
        }
    }
    public virtual void OnDownDashPressed()
    {
        if (playerCharacterHub.downDashState.nbDashDownMade < 1 && !playerCharacterHub.IsGrounded)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.downDashState;
            if (playerCharacterHub.jumpState.NbJumpMade > 0)
                playerCharacterHub.downDashState.nbDashDownMade++;
        }
    }
    public virtual void PushPlayer(Vector3 force)
    {
        playerCharacterHub.PlayerState = playerCharacterHub.expulsedState;
        playerCharacterHub.Rb.velocity = new Vector3(force.x, playerCharacterHub.Rb.velocity.y, force.z);
    }
    public virtual void HandleControllerAnim()
    {
        playerCharacterHub.Anim.SetFloat("MouvementSpeed", Utils.Abs(playerControllerHub.State.ThumbSticks.Left.X) > Utils.Abs(playerControllerHub.State.ThumbSticks.Left.Y) ? Utils.Abs(playerControllerHub.State.ThumbSticks.Left.X) + 0.2f : Utils.Abs(playerControllerHub.State.ThumbSticks.Left.Y) + 0.2f);
        playerCharacterHub.Anim.SetBool("isWalking", ((Utils.Abs(playerControllerHub.State.ThumbSticks.Left.X) > 0.02f) || Utils.Abs(playerControllerHub.State.ThumbSticks.Left.Y) > 0.02f) && playerCharacterHub.IsGrounded);
        float lastValue = playerCharacterHub.Anim.GetFloat("YVelocity");
        float newValue = Mathf.Lerp(lastValue, (Mathf.Clamp(playerCharacterHub.Rb.velocity.y / 30.0f, -1.0f, 1.0f) + 1) / 2.0f, 0.25f);
        playerCharacterHub.Anim.SetFloat("YVelocity", newValue);
    }


    public virtual void CollisionEnter(Collision collision)
    { }
    public virtual void CollisionStay(Collision collision)
    { }
    public virtual void CollisionExit(Collision collision)
    { }
    public virtual void DrawGizmo()
    {}
}

