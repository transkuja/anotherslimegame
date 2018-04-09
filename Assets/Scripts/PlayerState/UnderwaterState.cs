using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterState : PlayerState
{
    public bool hasReachedTheSurface = false;
    public bool hasStartedGoingUp = false;

    public float waterLevel;
    float waterTolerance;
    public UnderwaterState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }

    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        waterTolerance = playerCharacterHub.GetComponent<SphereCollider>().radius;
        if (hasReachedTheSurface)
        {
            playerCharacterHub.Anim.SetBool("isBoobbing", true);
        }
        else
        {
            if (AudioManager.Instance != null && AudioManager.Instance.splashWaterFx != null)
            {
                if (playerCharacterHub.GetComponent<PNJController>() && playerCharacterHub.GetComponent<PNJController>().myAudioSource != null)
                {
                    playerCharacterHub.GetComponent<PNJController>().myAudioSource.PlayOneShot(AudioManager.Instance.splashWaterFx, 5);
                }
                else
                {
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.splashWaterFx);
                }
            }
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        playerCharacterHub.Anim.SetBool("isBoobbing", false);
        if (AudioManager.Instance != null)
            AudioManager.Instance.ClearFX(AudioManager.Instance.swimmingFx);

        hasReachedTheSurface = false;
        hasStartedGoingUp = false;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
    }

    // No dash underwater
    public override void OnDashPressed()
    {

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!hasReachedTheSurface && hasStartedGoingUp && playerCharacterHub.transform.position.y > waterLevel - waterTolerance)
        {
            hasReachedTheSurface = true;
            playerCharacterHub.Anim.SetBool("isBoobbing", true);
        }

        // Critical case
        if (playerCharacterHub.transform.position.y > waterLevel)
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
    }

    public override void OnJumpPressed()
    {
        if (hasReachedTheSurface)
        {
            // TODO: cancel anim
            hasReachedTheSurface = false;
            base.OnJumpPressed();
        }
    }

    // Disable gravity when underwater
    public override void HandleGravity()
    {
        if (playerCharacterHub.transform.position.y < waterLevel - waterTolerance - 1f)
        {
            hasStartedGoingUp = true;
        }

        if (!hasReachedTheSurface && hasStartedGoingUp)
        {
            playerCharacterHub.Rb.AddForce(Gravity.underwaterGravity * Vector3.down);
        }

        if (!hasStartedGoingUp && playerCharacterHub.transform.position.y < waterLevel - waterTolerance && playerCharacterHub.transform.position.y > waterLevel - waterTolerance - 1f)
        {
            playerCharacterHub.Rb.AddForce(Gravity.defaultGravity * Vector3.down);
        }

        if (hasReachedTheSurface)
            playerCharacterHub.Rb.velocity = new Vector3(playerCharacterHub.Rb.velocity.x, 0, playerCharacterHub.Rb.velocity.z);
    }
}
