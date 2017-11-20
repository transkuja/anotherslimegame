using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedPlatform : MonoBehaviour {
    enum TrapType { MoveHorizontal, MoveBackward, MoveDown, Flip, RotateAroundY, Size }
    public Player owner;

    // The chance of the platform being trapped is 1 out of inverseTrapChance
    int inverseTrapChance = 2;

    PlatformGameplay gameplay;

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
        gameplay = GetComponent<PlatformGameplay>();
        if (gameplay == null)
        {
            Debug.LogWarning("Platform gameplay component is null and shouldn't");
            return;
        }

        TrapType trapRand = (TrapType)Random.Range(0, (int)TrapType.Size);      
        switch(trapRand)
        {
            case TrapType.MoveHorizontal:
                break;
            case TrapType.MoveBackward:
                break;
            case TrapType.MoveDown:
                break;
            case TrapType.RotateAroundY:
                break;
            case TrapType.Flip:
                break;
        }
    }

    void MoveHorizontal()
    {
        gameplay.movingAxis = Vector3.right * (Random.Range(0,2) == 0 ? -1 : 1);
        gameplay.IsMoving = true;
    }

    void MoveBackward()
    {
        gameplay.movingAxis = Vector3.forward * -1;
        gameplay.IsMoving = true;
    }

    void MoveDown()
    {
        gameplay.movingAxis = Vector3.up * -1;
        gameplay.IsMoving = true;
    }

    void RotateAroundY()
    {

    }

    void Flip()
    {

    }
}
