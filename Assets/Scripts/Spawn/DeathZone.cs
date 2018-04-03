using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCharacterHub>() != null)
        {
            other.GetComponent<PlayerCharacterHub>().OnDeath();
        }
        else if(other.GetComponent<PlayerControllerKart>()) //Needs to be replaced with a character?
        {
            other.GetComponent<PlayerControllerKart>().RespawnToLastCheckpoint();
        }
    }
}
