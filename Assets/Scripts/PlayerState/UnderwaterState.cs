﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterState : PlayerState
{
    public bool hasReachedTheSurface = false;
    public bool hasStartedGoingUp = false;

    public float waterLevel;
    float waterTolerance;
    public UnderwaterState(PlayerControllerHub _playerController) : base(_playerController)
    {
    }

    public override void DrawGizmo()
    {
        base.DrawGizmo();
    }

    public override void OnBegin()
    {
        base.OnBegin();
        waterTolerance = playerController.GetComponent<SphereCollider>().radius;
        if (hasReachedTheSurface)
        {
            playerController.Player.Anim.SetBool("isBoobbing", true);
        }
        else
        {
            if (AudioManager.Instance != null && AudioManager.Instance.splashWaterFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.splashWaterFx);
        }
    }

    public override void OnEnd()
    {
        base.OnEnd();
        playerController.Player.Anim.SetBool("isBoobbing", false);
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
        if (!hasReachedTheSurface && hasStartedGoingUp && playerController.transform.position.y > waterLevel - waterTolerance)
        {
            hasReachedTheSurface = true;
            playerController.Player.Anim.SetBool("isBoobbing", true);
        }

        // Critical case
        if (playerController.transform.position.y > waterLevel)
            playerController.PlayerState = playerController.freeState;
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
        if (playerController.transform.position.y < waterLevel - waterTolerance - 1f)
        {
            hasStartedGoingUp = true;
        }

        if (!hasReachedTheSurface && hasStartedGoingUp)
        {
            playerController.Player.Rb.AddForce(Gravity.underwaterGravity * Vector3.down);
        }

        if (!hasStartedGoingUp && playerController.transform.position.y < waterLevel - waterTolerance && playerController.transform.position.y > waterLevel - waterTolerance - 1f)
        {
            playerController.Player.Rb.AddForce(Gravity.defaultGravity * Vector3.down);
        }

        if (hasReachedTheSurface)
            playerController.Player.Rb.velocity = new Vector3(playerController.Player.Rb.velocity.x, 0, playerController.Player.Rb.velocity.z);
    }
}
