using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectEvent : MonoBehaviour {

    public GameObject go;

    public CollectableType type;

    public float evolutionDuration = 1.0f;

    public virtual void OnCollectEvent(Player playerTarget)
    {

        GameManager.EvolutionManager.AddEvolutionComponent(
           playerTarget.gameObject,
           GameManager.EvolutionManager.GetEvolutionByCollectableType(type),
           true,
           evolutionDuration
       );
    }
}
