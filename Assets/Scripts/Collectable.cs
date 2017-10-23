﻿using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    CollectableType type;
    [SerializeField]
    int value;
    bool isAttracted = false;
    uint movementSpeed = 40;
    Player playerTarget;

    private void OnTriggerEnter(Collider other)
    {
        PickUp(other.GetComponent<Player>());
    }

    private void PickUp(Player player)
    {
        if (player && !isAttracted)
        {
            // Grab everything not linked to evolution (points)
            if (!Utils.IsAnEvolutionCollectable(type))
            {
                if (player.Collectables[(int)type] < Utils.GetMaxValueForCollectable(type))
                {
                    isAttracted = true;
                    playerTarget = player;
                    return;
                }
            }
            else
            {
                if (GameManager.CurrentGameMode.evolutionMode != EvolutionMode.GrabEvolution)
                {
                    if (player.Collectables[(int)type] < Utils.GetMaxValueForCollectable(type))
                    {
                        isAttracted = true;
                        playerTarget = player;
                    }
                }
                else
                {
                    if (player.activeEvolutions == 0)
                    {
                        isAttracted = true;
                        playerTarget = player;
                    }
                }
            }

        }    
    }

    private void FixedUpdate()
    {
        if (isAttracted)
            Attract();
    }

    void Attract()
    {
        Vector3 direction = (playerTarget.transform.position - transform.position).normalized;
   
        GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < GetComponent<MeshFilter>().mesh.bounds.extents.magnitude)
        {
            playerTarget.UpdateCollectableValue(type, value);
            Destroy(this.gameObject);
        }
    }
}
