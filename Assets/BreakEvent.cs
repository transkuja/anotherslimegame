using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakEvent : MonoBehaviour {

    public GameObject go;

    public CollectableType type;

    public void OnBreakEvent()
    {
        go.GetComponentInChildren<Collectable>().enabled = true;

        Invoke("SpawnMyself", 10);
    }

    public void SpawnMyself()
    {
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        GameObject p = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(transform.position, transform.rotation, transform, type);
        p.GetComponent<Collectable>().enabled = false;

        GetComponent<BreakEvent>().go = p;
    }
}
