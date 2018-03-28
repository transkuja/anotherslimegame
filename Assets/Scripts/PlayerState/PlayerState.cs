using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ptrStateFct();
public class PlayerState {

    protected PlayerControllerHub playerController;
    public bool stateAvailable = true;

    protected ptrStateFct curUpdateFct;
    protected ptrStateFct curFixedUpdateFct;
    public float maxCoolDown = 0;

    float dragForce = 0.025f;
    float dragForceDash = 0.01f; // Dash value

    float airControlFactor = 0.42f;
    float timerApplyDrag = 0.02f;

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


    public PlayerState(PlayerControllerHub _playerController)
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
        Vector3 initialVelocity = new Vector3(playerController.State.ThumbSticks.Left.X, 0.0f, playerController.State.ThumbSticks.Left.Y);
        initialVelocity.Normalize();

        initialVelocity *= playerController.stats.Get(Stats.StatType.GROUND_SPEED);
        if (Utils.Abs(playerController.State.ThumbSticks.Left.X) + Utils.Abs(playerController.State.ThumbSticks.Left.Y) < 0.95f)
        {
            initialVelocity /= 2;
        }

        return initialVelocity;
    }

    public Vector3 GetVelocity3DThirdPerson(Vector3 initialVelocity)
    {
        Vector3 camVectorForward = new Vector3(playerController.Player.cameraReference.transform.GetChild(0).forward.x, 0.0f, playerController.Player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward;
        if (!playerController.IsGrounded)
            velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right * airControlFactor;
        else
            velocityVec += initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right;

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
        Vector3 velocityVec =  Vector3.up * playerController.Player.Rb.velocity.y;
        // MENU peter a cause de cette condition tu sais pourquoi c'est la antho ? sinon je peux faire une exception pour le menu
        if (!playerController.IsGrounded && playerController.jumpState.NbJumpMade == 2)
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
            playerController.Rb.drag = 0.0f;
            if (!playerController.forceCameraRecenter)
            {
                float maxForceMagnitude = playerController.stats.Get(Stats.StatType.GROUND_SPEED);
                float maxSpeed = maxForceMagnitude;
                if (!playerController.IsGrounded/* && playerController.jumpState.nbJumpMade == 2*/)
                {
                    maxForceMagnitude *= airControlFactor;
                }

                if (Utils.Abs(playerController.State.ThumbSticks.Left.X) + Utils.Abs(playerController.State.ThumbSticks.Left.Y) < 0.95f)
                {
                    maxForceMagnitude /= 2;
                    maxSpeed /= 2;
                }

                playerController.Rb.AddForce(velocityVec * maxForceMagnitude*Time.deltaTime*50);
                Vector3 xzVelocity = new Vector3(playerController.Rb.velocity.x, 0, playerController.Player.Rb.velocity.z);
                xzVelocity = Vector3.ClampMagnitude(xzVelocity, maxSpeed);
                playerController.Rb.velocity = (/*(playerController.IsGrounded) ? 0 : */playerController.Rb.velocity.y) * Vector3.up + xzVelocity;
                //playerController.transform.LookAt(playerController.transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z) + initialVelocity.x * playerController.Player.cameraReference.transform.GetChild(0).right);
                playerController.transform.LookAt(playerController.transform.position + xzVelocity);
            }
        }
        else
        {
            if (playerController.IsGrounded)
                playerController.Rb.drag = 15.0f;
            else
                playerController.Rb.drag = 0.0f;
        }
    }
    public virtual void HandleGravity()
    {
        if (playerController.IsGravityEnabled)
        {
            playerController.Player.Rb.AddForce(Gravity.defaultGravity * Vector3.down);
        }

       
    }
    public virtual void OnJumpPressed()
    {
        playerController.Rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (playerController.jumpState.NbJumpMade < playerController.stats.Get(Stats.StatType.JUMP_NB))
        {
            playerController.PlayerState = playerController.jumpState;
        }
    }
    public virtual void OnDashPressed()
    {
        if (playerController.dashState.nbDashMade < 1)
        {
            playerController.PlayerState = playerController.dashState;
            if (playerController.jumpState.NbJumpMade > 0)
                playerController.dashState.nbDashMade++;
        }
    }
    public virtual void OnDownDashPressed()
    {
        if (playerController.downDashState.nbDashDownMade < 1 && !playerController.IsGrounded)
        {
            playerController.PlayerState = playerController.downDashState;
            if (playerController.jumpState.NbJumpMade > 0)
                playerController.downDashState.nbDashDownMade++;
        }
    }
    public virtual void PushPlayer(Vector3 force)
    {
        playerController.PlayerState = playerController.expulsedState;
        playerController.Rb.velocity = new Vector3(force.x, playerController.Rb.velocity.y, force.z);
    }
    public virtual void HandleControllerAnim()
    {
        playerController.Player.Anim.SetFloat("MouvementSpeed", Utils.Abs(playerController.State.ThumbSticks.Left.X) > Utils.Abs(playerController.State.ThumbSticks.Left.Y) ? Utils.Abs(playerController.State.ThumbSticks.Left.X) + 0.2f : Utils.Abs(playerController.State.ThumbSticks.Left.Y) + 0.2f);
        playerController.Player.Anim.SetBool("isWalking", ((Utils.Abs(playerController.State.ThumbSticks.Left.X) > 0.02f) || Utils.Abs(playerController.State.ThumbSticks.Left.Y) > 0.02f) && playerController.IsGrounded);
        float lastValue = playerController.Player.Anim.GetFloat("YVelocity");
        float newValue = Mathf.Lerp(lastValue, (Mathf.Clamp(playerController.Rb.velocity.y / 30.0f, -1.0f, 1.0f) + 1) / 2.0f, 0.25f);
        playerController.Player.Anim.SetFloat("YVelocity", newValue);
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
