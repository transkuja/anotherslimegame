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
    private Jump curJump;
    public Jump[] jumpTab;
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (curJump == null && jumpTab!=null && jumpTab.Length>1)
        {
            curJump = jumpTab[0];
        }
    }
    void SetGravity()
    {
        rb.useGravity = false;
    }

    public void Jump(float playerMaxGroundSpeed, JumpEnum type)
    {
        SetGravity();
        if (jumpTab != null && jumpTab.Length>=(int)type)
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
        curJump = null;
        rb.useGravity = true;
    }

    // dans le cas d'un jump à pression continue

    public void EndPushInputJump()
    {
        if (curJump != null && rb.velocity.y >0)
            curJump.SwitchMinimalJump();
    }
   
   
}