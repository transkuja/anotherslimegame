using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController3DDefault : PlayerController {


    public bool isGravityEnabled = true;
    [SerializeField]protected bool isGrounded = true;
    public JumpState jumpState;
    public DashState dashState;


    public bool IsGrounded
    {
        get
        {
            return isGrounded;
        }

        set
        {
            if (value == true)
            {
                jumpState.nbJumpMade = 20; // Very high value, reset when releasing button AND being grounded
                downDashState.nbDashDownMade = 0;
                dashState.nbDashMade = 0;
                if (GetComponent<JumpManager>() != null)
                    GetComponent<JumpManager>().Stop();
                GetComponent<Player>().Anim.SetBool("isExpulsed", false);
                if (dustTrailParticles && dustTrailParticles.GetComponent<ParticleSystem>() != null)
                {
                    dustTrailParticles.GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                if (dustTrailParticles && dustTrailParticles.GetComponent<ParticleSystem>() != null)
                {
                    dustTrailParticles.GetComponent<ParticleSystem>().Stop();
                }

            }

            isGrounded = value;
        }
    }


    protected override void Awake()
    {
        jumpState = new JumpState(this);
        dashState = new DashState(this);
        downDashState = new DashDownState(this);
    }


}
