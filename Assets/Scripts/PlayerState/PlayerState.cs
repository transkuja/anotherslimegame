using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ptrStateFct();
public class PlayerState {

    protected PlayerCharacterHub playerCharacterHub;
    public bool stateAvailable = true;

    protected ptrStateFct curUpdateFct;
    protected ptrStateFct curFixedUpdateFct;
    public float maxCoolDown = 0;

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


    public PlayerState(PlayerCharacterHub _playerCharacterHub)
    {
        playerCharacterHub = _playerCharacterHub;
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

    public virtual Vector3 HandleSpeed(float x, float y)
    {
        Vector3 initialVelocity = new Vector3(x, 0.0f, y);
        initialVelocity.Normalize();

        initialVelocity *= playerCharacterHub.stats.Get(Stats.StatType.GROUND_SPEED);
        if (Utils.Abs(x) + Utils.Abs(y) < 0.95f)
        {
            initialVelocity /= 2;
        }

        return initialVelocity;
    }

 

    public virtual void Move(Vector3 initialVelocity, float airControlFactor, float x, float y, bool forceCameraRecenter = false)
    {
        //velocityVec.Normalize();
        if (initialVelocity.magnitude > 1f)
        {
            playerCharacterHub.Rb.drag = 0.0f;
            if (!forceCameraRecenter)
            {
                float maxForceMagnitude = playerCharacterHub.stats.Get(Stats.StatType.GROUND_SPEED);
                float maxSpeed = maxForceMagnitude;
                if (!playerCharacterHub.IsGrounded/* && playerController.jumpState.nbJumpMade == 2*/)
                {
                    maxForceMagnitude *= airControlFactor;
                }

                if (Utils.Abs(x) + Utils.Abs(y) < 0.95f)
                {
                    maxForceMagnitude /= 2;
                    maxSpeed /= 2;
                }

                playerCharacterHub.Rb.AddForce(initialVelocity * maxForceMagnitude*Time.deltaTime*50);
                Vector3 xzVelocity = new Vector3(playerCharacterHub.Rb.velocity.x, 0, playerCharacterHub.Rb.velocity.z);
                xzVelocity = Vector3.ClampMagnitude(xzVelocity, maxSpeed);
                playerCharacterHub.Rb.velocity = (playerCharacterHub.Rb.velocity.y) * Vector3.up + xzVelocity;
              
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
    public virtual void HandleControllerAnim(float x, float y)
    {
        playerCharacterHub.Anim.SetFloat("MouvementSpeed", Utils.Abs(x) > Utils.Abs(y) ? Utils.Abs(x) + 0.2f : Utils.Abs(y) + 0.2f);
        playerCharacterHub.Anim.SetBool("isWalking", ((Utils.Abs(x) > 0.02f) || Utils.Abs(y) > 0.02f) && playerCharacterHub.IsGrounded);
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

