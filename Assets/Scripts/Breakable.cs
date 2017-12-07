using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

    public int minFragments = 2;
    public int maxFragments = 5;
    public int minCollectableDropOnBreak = 1;
    public int maxCollectableDropOnBreak = 5;

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.transform.GetComponent<PlayerController>();
        if (player != null && (
                player.PlayerState == player.dashState
                || player.PlayerState == player.downDashState
                )
           )
        {
            // impact
            if (GetComponent<MeshRenderer>()) GetComponent<MeshRenderer>().enabled = false;
            else if (GetComponentInChildren<MeshRenderer>()) GetComponentInChildren<MeshRenderer>().enabled = false;
            if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
            else if (GetComponentInChildren<Collider>()) GetComponentInChildren<Collider>().enabled = false;

            // pool de morceaux cassés
            int nbFragments = Random.Range(minFragments, maxFragments);
            for (int i = 0; i < nbFragments; i++)
            {
                GameObject fragment = ResourceUtils.Instance.poolManager.breakablePiecesPool.GetItem();
                fragment.transform.SetParent(transform);
                fragment.transform.localPosition = Vector3.up * 0.5f;
                fragment.SetActive(true);
            }

            DropCollectableOnGround();
        }
    }

    void DropCollectableOnGround()
    {
        int numberOfCollectablesToDrop = Random.Range(minCollectableDropOnBreak, maxCollectableDropOnBreak);
        Vector3[] positions = SpawnManager.GetVector3ArrayOnADividedCircle(transform.position, 1.0f, numberOfCollectablesToDrop, SpawnManager.Axis.XZ);
        for (int i = 0; i < numberOfCollectablesToDrop; i++)
        {
            GameObject go = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
            positions[i] + Vector3.up * 0.5f,
            Quaternion.identity,
            null,
            CollectableType.Points,
            true);

            go.GetComponent<Collectable>().Disperse(i, (positions[i] - transform.position + Vector3.up * 1.5f).normalized);
        }
    }
}
