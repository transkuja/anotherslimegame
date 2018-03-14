using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Inspiré de "Math for Game Programmers: Building a Better Jump"
 *  https://www.youtube.com/watch?v=hG9SzQxaCm8                  */

[RequireComponent(typeof(Rigidbody))]
public class JumpManager : MonoBehaviour
{
    public enum JumpEnum
    {
        Small,Basic, Double
    }


    private Rigidbody rb;
    private PlayerControllerHub pc;

    private Jump curJump;
    public Jump[] jumpTab;

    float timerForForcedStop = 1.0f;
    Coroutine forceStopCoroutine;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerControllerHub>();
    }
    public void Start()
    {
        for (int i = 0; i < jumpTab.Length; i++)
        {
            jumpTab[i].InitValues(pc.stats.Get(Stats.StatType.GROUND_SPEED));
        }
    }
    void SetGravity()
    {
        pc.isGravityEnabled = false;
        forceStopCoroutine = StartCoroutine("ForceStop");
    }

    IEnumerator ForceStop()
    {
        yield return new WaitForSeconds(timerForForcedStop);
        Stop();
    }

    public void Jump(JumpEnum type)
    {
        if (jumpTab != null && jumpTab.Length >= (int)type) 
            curJump = jumpTab[(int) type];

        if (curJump != null)
        {
            curJump.InitJump(rb);
            SetGravity();
        }
    }

    public void FixedUpdate()
    {
        if (curJump != null)
            curJump.JumpFixedUpdate();
    }
    public void Stop()
    {
        if (forceStopCoroutine != null)
        {
            StopCoroutine("ForceStop");
            forceStopCoroutine = null;
        }
        if (curJump != null)
        {
            curJump = null;
            pc.isGravityEnabled = true;
        }
    }


    public void EndPushInputJump()
    {
        if (curJump != null && rb.velocity.y >0)
            curJump.SwitchMinimalJump();
    }
   public float GetGravity()
    {
        if (jumpTab!=null)
        {
            Jump basicJump = jumpTab[(int)JumpEnum.Basic];
            if (basicJump != null)
            {
                if (basicJump.HasFallingParabola)
                    return -basicJump.FallingParabola.CurGravity;
                else
                    return -basicJump.upParabola.CurGravity;
            }
        }
        Debug.Log("Error Gravity : Player has no basicJump : gravity set to 90");
        return -Gravity.defaultGravity;
    }
    public float GetJumpHeight()
    {
        if (jumpTab != null)
            return jumpTab[(int)JumpEnum.Basic].upParabola.Height;
        else return 0;
    }
    public void SetJumpHeight(float height,float maxGroundSpeed)
    {
        if (jumpTab != null)
        {
            jumpTab[(int)JumpEnum.Basic].upParabola.Height = height;
            jumpTab[(int)JumpEnum.Basic].upParabola.ComputeValues(maxGroundSpeed);
        }
    }
}