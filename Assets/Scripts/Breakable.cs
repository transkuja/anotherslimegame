using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour {

    public int minFragments;
    public int maxFragments;

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
        }
    }
}
