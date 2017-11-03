using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcedJump {
    bool isForcedJumpActive = false;
    Vector3 repulseForce = Vector3.zero;

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
    }

    public void Stop()
    {
        isForcedJumpActive = false;
        repulseForce = Vector3.zero;
    }

    public void AddForcedJumpForce(Rigidbody _rb)
    {
        _rb.velocity += repulseForce;
    }

}
