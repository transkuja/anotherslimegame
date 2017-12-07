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
            // TODO: may externalize this behaviour to avoid duplication
            Vector3 playerToTarget = transform.position - collision.transform.position;
            Vector3 playerCenterToTargetCenter = (transform.position + Vector3.up * 0.5f) - (collision.transform.position + Vector3.up * 0.5f);
            GameObject go = Instantiate(ResourceUtils.Instance.particleSystemManager.impactFeedback, transform);

            go.transform.position = transform.position + Vector3.up * 0.5f + playerCenterToTargetCenter / 2.0f;
            go.transform.rotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
            Destroy(go, 10.0f); // TODO: use a pool instead of instantiate/destroy in chain
            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);


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
            if (AudioManager.Instance != null && AudioManager.Instance.breakFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.breakFx);
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
