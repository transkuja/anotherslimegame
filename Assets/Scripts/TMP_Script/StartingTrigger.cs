using UnityEngine;



public class StartingTrigger : MonoBehaviour {

    public GameModeProperties gmProperties;

    public Material mat;
    public Color color;

    public void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        color = mat.color;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>() != null)
        {
            LevelSelection.Instance.ListOfPotentialGame.Add(gmProperties);
            GetComponent<MeshRenderer>().material.color = Color.green;
            LevelSelection.Instance.ProcessCountdown();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            GetComponent<MeshRenderer>().material.color = color;
            LevelSelection.Instance.ListOfPotentialGame.Remove(gmProperties);
            LevelSelection.Instance.ProcessCountdown();
        }
    }
}
