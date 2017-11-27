using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalljumpState : PlayerState
{
    float pushForce;
    float pushTime;
    public Vector3 pushDirection;
    float timer;
    public WalljumpState(PlayerController _playerController) : base(_playerController)
    {
    }
    public override void OnBegin()
    {
        base.OnBegin();
        curFixedUpdateFct = PushedFromWall;
        pushTime = 0.5f;
        pushForce = 700;
        timer = 0;
        JumpManager jm;
        if (jm = playerController.GetComponent<JumpManager>())
            jm.Jump(JumpManager.JumpEnum.Basic);
        playerController.Rb.AddForce(pushDirection.normalized * pushForce);
        //playerController.Rb.velocity = ;
    }
    public override void OnEnd()
    {
        base.OnEnd();
    }
    public void PushedFromWall()
    {
        timer += Time.deltaTime;
        if (timer > pushTime)
        {
            playerController.PlayerState = playerController.freeState;
        }
    }
   

    public bool WallJumpTest()
    {
        if (playerController.GetComponent<EvolutionAgile>() != null)
            if (!playerController.IsGrounded)
            {
                Collider[] collTab;

                LayerMask layer = LayerMask.GetMask(new string[] { "Default" });
                collTab = Physics.OverlapSphere(playerController.transform.position, 2.5f, layer);
                if (collTab != null)
                {
                    for (int i = 0; i < collTab.Length; i++)
                    {
                        // il faudra vérifier quel est le mur le plus proche. 
                        Vector3 contact = collTab[i].ClosestPointOnBounds(playerController.transform.position);
                        Vector3 normal = (playerController.transform.position - contact).normalized;
                        if (Vector3.Angle(Vector3.up, normal) > 65 || normal == Vector3.zero) // a partir de quel moment le mur est trop incliné, (marche dessus plutot que sauter)
                        {
                            playerController.wallJumpState.pushDirection = normal;
                            return true; // arrêt de la boucle
                        }
                    }
                }
            }
        return false;
    }

    public override void Move(Vector3 initialVelocity)
    {
    }
    public override void OnDashPressed()
    {
    }
    public override void OnDownDashPressed()
    {
    }
    public override void OnJumpPressed()
    {
    }
}
