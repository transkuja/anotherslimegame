using UnityEngine;



public class StartingTrigger : MonoBehaviour {

    public GameModeProperties gmProperties;

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>() != null)
        {
            LevelSelection.Instance.ListOfPotentialGame.Add(gmProperties);
            LevelSelection.Instance.ProcessCountdown();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            LevelSelection.Instance.ListOfPotentialGame.Remove(gmProperties);
            LevelSelection.Instance.ProcessCountdown();
        }
    }
}
