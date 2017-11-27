using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTeleporter : MonoBehaviour {
    public CollectableType evolutionType;
    bool isTeleporterActive = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Arrived at destination
        if (collision.transform.GetComponent<Player>() != null && collision.transform.GetComponent<Player>().hasBeenTeleported)
        {
            collision.transform.GetComponent<Player>().hasBeenTeleported = false;
            return;
        }

        // TODO; dev here the day we want multiple evolution component behaviour
        if (Utils.CheckEvolutionAndCollectableTypeCompatibility(evolutionType, collision.transform.GetComponent<EvolutionComponent>()))
        {
            GetComponent<PlatformGameplay>().isATeleporter = true;
            isTeleporterActive = true;
        }
    }

    private void Update()
    {
        if (isTeleporterActive)
        {
            PlatformGameplay gameplayComponent = GetComponent<PlatformGameplay>();
            float lerpValue = (gameplayComponent.delayBeforeMovement - gameplayComponent.DelayTimer) / gameplayComponent.delayBeforeMovement;
            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.red, lerpValue));
            if (lerpValue == 1.0f)
            {
                isTeleporterActive = false;
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isTeleporterActive && Utils.CheckEvolutionAndCollectableTypeCompatibility(evolutionType, collision.transform.GetComponent<EvolutionComponent>()))
        {
            if (GetComponent<MeshRenderer>())
            {
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
            }
            isTeleporterActive = false;
            GetComponent<PlatformGameplay>().isATeleporter = false;
        }
    }


}
