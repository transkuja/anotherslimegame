using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<EnnemyController>() || other.GetComponent<TheBombPickup>())
        {
            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles)
            .GetItem(null, transform.position, Quaternion.identity, true, false, (int)HitParticles.BigHit);
            Destroy(other.gameObject);
        }
        if (other.GetComponent<Player>() != null)
        {
            other.GetComponent<Player>().OnDeath();
        }
    }
}
