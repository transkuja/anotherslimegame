using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControllerHub>() != null)
        {
            other.GetComponent<PlayerControllerHub>().OnDeath();
        }
    }
}
