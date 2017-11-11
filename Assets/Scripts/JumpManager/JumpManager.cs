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
    private PlayerController pc;

    private Jump curJump;
    public Jump[] jumpTab;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
        if (curJump == null && jumpTab!=null && jumpTab.Length>1)
        {
            curJump = jumpTab[0];
        }
      
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
    }

    public void Jump(float playerMaxGroundSpeed, JumpEnum type)
    {
        SetGravity();
        if (jumpTab != null && jumpTab.Length >= (int)type) 
            curJump = jumpTab[(int) type];

        if (curJump != null)
            curJump.InitJump(rb, playerMaxGroundSpeed);
    }

    public void FixedUpdate()
    {
        if (curJump != null)
            curJump.JumpFixedUpdate();
    }
    public void Stop()
    {
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
   public float GetGravity(float playerMaxGroundSpeed)
    {
        if (jumpTab!=null)
        {
            Jump basicJump = jumpTab[(int)JumpEnum.Basic];
            if (basicJump != null)
            {
                basicJump.InitValues(playerMaxGroundSpeed);
                if (basicJump.HasFallingParabola)
                    return -basicJump.FallingParabola.CurGravity;
                else
                    return -basicJump.upParabola.CurGravity;
            }
        }
        Debug.Log("Error Gravity : Player has no basicJump : gravity set to 90");
        return -90;
    }
   
}