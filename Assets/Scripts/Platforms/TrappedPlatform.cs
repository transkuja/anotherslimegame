using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedPlatform : MonoBehaviour {
    enum TrapType { MoveHorizontal, MoveBackward, MoveDown, Flip, Size }
    public Player owner;

    // The chance of the platform being trapped is 1 out of inverseTrapChance
    int inverseTrapChance = 2;

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with player from above and not the owner
        Player collisionPlayer = collision.gameObject.GetComponent<Player>();
        if (collisionPlayer != null
            && Vector3.Dot(collision.contacts[0].normal, Vector3.up) < -0.95f
            && owner != collisionPlayer)
        {
            // Trap launch
            if (Random.Range(0, inverseTrapChance) == 0)
                ActivateTrap();
        }
    }

    void ActivateTrap()
    {
        TrapType trapRand = (TrapType)Random.Range(0, (int)TrapType.Size);      
        switch(trapRand)
        {
            case TrapType.MoveHorizontal:
                break;
            case TrapType.MoveBackward:
                break;
            case TrapType.MoveDown:
                break;
            case TrapType.Flip:
                break;
        }
    }
}
