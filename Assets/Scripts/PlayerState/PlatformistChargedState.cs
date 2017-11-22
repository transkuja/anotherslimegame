using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformistChargedState : PlayerState
{
    public PlatformistChargedState(PlayerController _playerController) : base(_playerController)
    {
    }

    public override Vector3 HandleSpeedWithController()
    {
        return Vector3.zero;
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
