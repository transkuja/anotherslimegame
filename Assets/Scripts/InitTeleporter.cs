using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTeleporter : MonoBehaviour {
    public CollectableType evolutionType;
    bool isTeleporterActive = false;
    Color startColor;

    private void Start()
    {
        startColor = GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
    }

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
            float lerpValue = (gameplayComponent.delayBetweenMovements - gameplayComponent.DelayTimer) / gameplayComponent.delayBetweenMovements;
            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(startColor, Color.red, lerpValue));
            if (lerpValue >= 1.0f)
            {
                isTeleporterActive = false;
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", startColor);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isTeleporterActive && Utils.CheckEvolutionAndCollectableTypeCompatibility(evolutionType, collision.transform.GetComponent<EvolutionComponent>()))
        {
            if (GetComponent<MeshRenderer>())
            {
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", startColor);
            }
            isTeleporterActive = false;
            GetComponent<PlatformGameplay>().isATeleporter = false;
        }
    }


}
