using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedPlatform : MonoBehaviour {
    enum TrapType { MoveHorizontal, MoveBackward, MoveDown, Flip, RotateAroundY, Size }
    public Player owner;

    [SerializeField] private bool isLevelDesignPlatform;
    [SerializeField] private TrapType trapType;
    // The chance of the platform being trapped is 1 out of inverseTrapChance
    int inverseTrapChance = 1;

    PlatformGameplay gameplay;

    bool isTrapEnabled = false;
    bool canMoveRight = true;
    bool canMoveLeft = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (isTrapEnabled)
            return;

        // Check if collision is with player from above and not the owner
        Player collisionPlayer = collision.gameObject.GetComponent<Player>();
        if (collisionPlayer != null
            && Vector3.Dot(collision.contacts[0].normal, Vector3.up) < -0.95f
            && owner != collisionPlayer)
        {
            // Trap launch
            if (inverseTrapChance == 1 || Random.Range(0, inverseTrapChance) == 0)
                ActivateTrap();
        }
    }

    void ActivateTrap()
    {
        gameplay = GetComponent<PlatformGameplay>();
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        mat.SetColor("_EmissionColor", Color.red);

        if (gameplay == null)
        {
            Debug.LogWarning("Platform gameplay component is null and shouldn't");
            return;
        }
        TrapType trapRand;
            // si la platforme est générée par le platformiste
        if (!isLevelDesignPlatform)
        {
            trapRand = (TrapType)Random.Range(0, (int)TrapType.Size);
            List<TrapType> usedIndex = new List<TrapType>();
            int nbAttempts = 0;

            while (!TrapViabilityCheck(trapRand) || nbAttempts == 10)
            {
                usedIndex.Add(trapRand);
                bool isInList = true;
                while (isInList)
                {
                    trapRand = (TrapType)Random.Range(0, (int)TrapType.Size);
                    foreach (TrapType checkedType in usedIndex)
                    {
                        if (checkedType != trapRand)
                            isInList = false;
                    }
                }
                nbAttempts++;
            }

            if (nbAttempts == 10) trapRand = TrapType.Flip;
        }
        else // si la platforme a été posée en LevelDesign
        {
            trapRand = trapType;
            StartCoroutine(ResetTrap());
        }

        switch (trapRand)
        {
            case TrapType.MoveHorizontal:
                MoveHorizontal();
                break;
            case TrapType.MoveBackward:
                MoveBackward();
                break;
            case TrapType.MoveDown:
                MoveDown();
                break;
            case TrapType.RotateAroundY:
                RotateAroundY();
                break;
            case TrapType.Flip:
                Flip();
                break;
        }

        isTrapEnabled = true;
    }

    bool TrapViabilityCheck(TrapType _trap)
    {
        switch (_trap)
        {
            case TrapType.MoveHorizontal:
                if (Physics.Raycast(new Ray(transform.position, transform.rotation * Vector3.right), gameplay.movingDistance))
                    canMoveRight = false;
                if (Physics.Raycast(new Ray(transform.position, transform.rotation * Vector3.left), gameplay.movingDistance))
                    canMoveLeft = false;
                return canMoveRight || canMoveLeft;
            case TrapType.MoveBackward:
                if (Physics.Raycast(new Ray(transform.position, transform.rotation * Vector3.back), gameplay.movingDistance))
                    return false;
                break;
            case TrapType.MoveDown:
                if (Physics.Raycast(new Ray(transform.position, transform.rotation * Vector3.down), gameplay.movingDistance))
                    return false;
                break;
        }

        return true;
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
        gameplay.baseRotation.rotateAxis = Vector3.up;
        gameplay.isRotating = true;
    }

    void Flip()
    {
        gameplay.hasADelayedRotation = true;
        gameplay.rotateAxisLocal = Random.Range(0, 2) == 0 ? Vector3.right : Vector3.forward;
        gameplay.isRotating = true;
    }
    IEnumerator ResetTrap()
    {
        yield return new WaitForSeconds(gameplay.delayBeforeTransition );
        Debug.Log("Once");
        while ( gameplay.rotateLerpValue > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(gameplay.delayBeforeTransitionReturn);
        Debug.Log("Two");
        while (gameplay.rotateLerpValue < 1)
        {
            yield return null;
        }
        gameplay.IsMoving = false;
        gameplay.hasADelayedRotation = false;
        gameplay.isRotating = false;
        gameplay.ResetPlatformToOrigin();

        isTrapEnabled = false;
        Material mat = GetComponentInChildren<MeshRenderer>().material;
        mat.SetColor("_EmissionColor", Color.white);
        yield return null;
    }
}
