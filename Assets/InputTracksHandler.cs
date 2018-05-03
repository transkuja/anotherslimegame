using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTracksHandler : MonoBehaviour {

    [SerializeField]
    List<GameObject> tracks;

    [SerializeField]
    GameObject inputPrefab;

	void Start () {
        for (int i = 0; i < GameManager.Instance.CurrentGameMode.curNbPlayers; i++)
        {
            Instantiate(inputPrefab, tracks[i].transform.GetChild(0));
        }
	}
	
	void Update () {
		
	}


    public void SendNextInputDude()
    {

    }
}
