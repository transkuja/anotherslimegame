using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpType
{
    Simple,Continue,Double
}


public class Jumper : MonoBehaviour {

    private JumpManager jumpManager;
    private Rigidbody rb;
    public float maxGroundSpeed;
    public JumpType jumpType;
    bool onAir = false;
    bool isJumping = false;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        jumpManager = GetComponent<JumpManager>();
	}
	
	void Update () {
    }
    private void FixedUpdate()
    {
        Vector3 direction = Vector3.zero;
        direction += Input.GetAxis("Horizontal") * Vector3.right;
        direction += Input.GetAxis("Vertical") * Vector3.forward;
        direction = direction.normalized * maxGroundSpeed;

        // si sur sol
        if (!onAir || onAir && direction!=Vector3.zero)
            rb.velocity = new Vector3(direction.x, rb.velocity.y, direction.z);
       

                SimpleJump();
        //switch (jumpType)
        //{
        //    case JumpType.Simple:
        //        break;
        //    case JumpType.Continue:
        //        ContinueJump();
        //        break;
        //    case JumpType.Double:
        //        DoubleJump();
        //        break;
        //    default:
        //        break;
        //}
    }
    bool doubleJump = false;
    public void SimpleJump()
    {
        //if (Input.GetButton("Jump") && !onAir)
        //{
        //    jumpManager.Jump(maxGroundSpeed, JumpEnum.Basic);
        //    onAir = true;
        //    Debug.Log("Jpos: " + transform.position);
        //}
        //else if (Input.GetButton("Jump") && onAir && doubleJump == false)
        //{
        //    jumpManager.Jump(maxGroundSpeed, JumpEnum.Double);
        //    onAir = true;
        //    Debug.Log("Jpos: " + transform.position);
        //    doubleJump = true;
        //}
    }
public void ContinueJump()
{
    if (Input.GetButton("Jump") && !onAir)
    {
    //    jumpManager.Jump(maxGroundSpeed, JumpEnum.Small);
    //    onAir = true;
    //    Debug.Log("Jpos: " + transform.position);
    //    isJumping = true;
    }
    if (isJumping && !Input.GetButton("Jump"))
    {
        jumpManager.EndPushInputJump();
        isJumping = false;
    }
}
public void DoubleJump()
{

}


private void OnCollisionEnter(Collision collision)
    {
        if (onAir == true && Vector3.Angle(collision.contacts[0].normal,Vector3.up)<30)
        {
            Debug.Log("Endpos: " + transform.position);
            jumpManager.Stop();
            onAir = false;
        }
    }

}



    /*
     Systeme avec uniquement 2 sauts : 
     --> Le joueur demande à sauter en précisant quelle parabole il utilise. 
     Sinon le système reste telle quel. Il faut juste paramétrer toutes les
     paraboles dans le meme saut. 
     */