using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedJump {
    bool isForcedJumpActive = false;
    public Vector3 repulseForce = Vector3.zero;

    public bool IsForcedJumpActive { get { return isForcedJumpActive; } }

    public ForcedJump()
    {
        isForcedJumpActive = false;
        repulseForce = Vector3.zero;
    }

    public ForcedJump(Vector3 _repulseForce)
    {
        repulseForce = _repulseForce;
        isForcedJumpActive = true;
    }

    public void StartJump()
    {
        isForcedJumpActive = true;
        if (AudioManager.Instance != null && AudioManager.Instance.hahahaFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.hahahaFX);

    }

    public void Stop()
    {
        isForcedJumpActive = false;
        repulseForce = Vector3.zero;
    }

    public void AddForcedJumpForce(PlayerCharacterHub _playerCharacterHub)
    {
        _playerCharacterHub.Rb.velocity += repulseForce;
        _playerCharacterHub.Anim.SetFloat("MouvementSpeed", 3);
        _playerCharacterHub.Anim.SetBool("isExpulsed", _playerCharacterHub.IsGrounded);
    }

}
