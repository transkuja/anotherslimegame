using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBombPickup : TheBall {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BoardFloor>())
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }


    public void Explode()
    {
        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles)
            .GetItem(null, transform.position, Quaternion.identity, true, false, (int)HitParticles.BigHitStar);

        int separationMask = LayerMask.GetMask(new string[] { "Player", "GhostPlayer", "Rabite" });
        Collider[] playersCollided = Physics.OverlapSphere(transform.position, 7.0f, separationMask);

        if (playersCollided != null && playersCollided.Length > 0)
        {
            for (int i = 0; i < playersCollided.Length; i++)
            {
                if (playersCollided[i].transform.GetComponent<PlayerCollisionCenter>() != null)
                    playersCollided[i].transform.GetComponent<PlayerCollisionCenter>()
                        .ExpulsePlayer(playersCollided[i].ClosestPoint(transform.position), playersCollided[i].GetComponent<PlayerCharacter>().Rb, 35);
            }
        }

        Destroy(gameObject);
    }
}
