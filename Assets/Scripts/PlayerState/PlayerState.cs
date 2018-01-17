using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState<TPlayerController>where TPlayerController : PlayerController {

    protected TPlayerController playerController;
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


    public PlayerState(TPlayerController _playerController)
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






    public virtual void CollisionEnter(Collision collision)
    { }
    public virtual void CollisionStay(Collision collision)
    { }
    public virtual void CollisionExit(Collision collision)
    { }



    public virtual void DrawGizmo()
    {}
}
