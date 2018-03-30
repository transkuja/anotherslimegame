﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformistChargedState : PlayerState
{
    public PlatformistChargedState(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub) : base(_playerCharacterHub, _playerControllerHub)
    {
    }

    public override void Move(Vector3 initialVelocity)
    {
        base.Move(initialVelocity);
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
