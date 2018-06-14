using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformistChargedState : PlayerState
{
    public PlatformistChargedState(PlayerCharacterHub _playerCharacterHub) : base(_playerCharacterHub)
    {
    }

    public override void Move(Vector3 initialVelocity, float airControlFactor, float x, float y, bool forceCameraRecenter = false)
    {
        playerCharacterHub.transform.LookAt(playerCharacterHub.transform.position + initialVelocity);

        playerCharacterHub.Rb.velocity = Vector3.zero;
    }

    public override void OnJumpPressed()
    {
        // TODO: cancel charge
        base.OnJumpPressed();
    }

    public override void OnDashPressed()
    {
    }
}
