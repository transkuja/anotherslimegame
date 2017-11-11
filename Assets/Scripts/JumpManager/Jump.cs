using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* Inspiré de "Math for Game Programmers: Building a Better Jump"
 *  https://www.youtube.com/watch?v=hG9SzQxaCm8                  */

/*
    Le saut permet de récupérer l'impusion
    et applique une gravité personnalisé.
*/


[System.Serializable]
public class Jump  {

    [SerializeField] private string name;

    public Parabola upParabola= null; // la parabole de base du saut
    [SerializeField] bool hasFallingParabola = false;
    [SerializeField] Parabola fallingParabola = null; // la parabole de descente du saut.

    [SerializeField] bool isContinue;
    [SerializeField] Parabola minJumpParabola = null; // la parabole minimale de saut.

    private Parabola curParabola = null; // parabole en cours -> set la gravité à appliquer.
    private float playerMaxGroundSpeed;
    Rigidbody controllerRb;

    #region Getters
    public bool IsContinue
    {
        get{return isContinue;}
        set {isContinue = value;}
    }
    public bool HasFallingParabola
    {
        get{return hasFallingParabola;}
        set { hasFallingParabola = value; }
    }
    public Parabola FallingParabola
    {
        get{return fallingParabola;}
    }

    public Parabola MinJumpParabola
    {
        get{return minJumpParabola;}
    }

    public string Name
    {
        get
        {
            return name;
        }

    }
    #endregion


    public void InitValues(float _playerMaxGroundSpeed)
    {
        playerMaxGroundSpeed = _playerMaxGroundSpeed;
        if (upParabola != null)
        {
            upParabola.ComputeValues(playerMaxGroundSpeed);
        }
        if (fallingParabola != null && hasFallingParabola)
        {
            fallingParabola.ComputeValues(playerMaxGroundSpeed);
        }
        if (minJumpParabola!=null && isContinue)
        {
            minJumpParabola.ComputeValues(playerMaxGroundSpeed);
        }
    }
    public void InitJump(Rigidbody rb,float _playerMaxGroundSpeed)
    {
        controllerRb = rb;
        playerMaxGroundSpeed = _playerMaxGroundSpeed;
        if (upParabola != null)
        {
            upParabola.ComputeValues(playerMaxGroundSpeed);
            //Debug.Log("upForce :" + upParabola.V0);

            rb.velocity = new Vector3(rb.velocity.x, upParabola.V0, rb.velocity.z);
            rb.useGravity = false;
        }
        curParabola = upParabola;
    }
    public void SwitchMinimalJump()
    {
        if (isContinue)
        {
            //Debug.Log("SwitchMinimalJump");
            curParabola = minJumpParabola;
            minJumpParabola.ComputeValues(playerMaxGroundSpeed);
        }
    }
    public void AtPeakOfJump()
    {
        Debug.Log("heightAtPeak : " + controllerRb.position.y);
        if (isContinue)
        {
            curParabola = upParabola;
            //Debug.Log("Back to firstParabola");
        }
        if (hasFallingParabola == true)
        {
            //Debug.Log("Parabola Falling");
            curParabola = fallingParabola;
            fallingParabola.ComputeValues(playerMaxGroundSpeed);
        }
    }
    Vector3 lastVelocity;



    public void JumpFixedUpdate()
    {
            // on applique la gravité personnalisé pour saut.
        if (curParabola != null)
        {
            controllerRb.AddForce(curParabola.CurGravity * Vector3.up, ForceMode.Acceleration);
            //Debug.Log("curGravity :" + curParabola.CurGravity);
            if (controllerRb.velocity.y <= 0 && lastVelocity.y > 0 )
            {
                AtPeakOfJump();
            }
            lastVelocity = controllerRb.velocity;
        }
    }

}
