using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTeleporter : MonoBehaviour {
    public CollectableType evolutionType;

    private void OnCollisionEnter(Collision collision)
    {
        // TODO; dev here the day we want multiple evolution component behaviour
        if (Utils.CheckEvolutionAndCollectableTypeCompatibility(evolutionType, collision.transform.GetComponent<EvolutionComponent>()))
            GetComponent<PlatformGameplay>().isATeleporter = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        GetComponent<PlatformGameplay>().isATeleporter = false;
    }
}
