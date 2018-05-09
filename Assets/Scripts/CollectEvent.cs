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
        //playerTarget.UpdateCollectableValue(type, 1);

        Invoke("ReactivateMySelf", 2);
    }

    public void ReactivateMySelf()
    {
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;
        foreach (Renderer mr in GetComponentsInChildren<Renderer>())
            mr.enabled = true;
        //GetComponent<MeshRenderer>().enabled = true;
        //GetComponent<BoxCollider>().enabled = true;
        //GameObject p = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(transform.position, transform.rotation, transform, type);
        //p.transform.localScale = new Vector3(0.33f, 0.33f, 0.33f);
        //p.GetComponent<Collectable>().enabled = false;

        //GetComponent<BreakEvent>().go = p;
    }
}
