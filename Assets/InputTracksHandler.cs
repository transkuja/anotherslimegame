using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTracksHandler : MonoBehaviour {

    [SerializeField]
    List<GameObject> tracks;

    [SerializeField]
    GameObject inputPrefab;

	public void StartGame() {
        for (int i = 0; i < GameManager.Instance.CurrentGameMode.curNbPlayers; i++)
        {
            GameObject newInput = Instantiate(inputPrefab, tracks[i].transform.GetChild(1)); // child 1 is start position
            newInput.transform.localPosition = Vector3.zero;
        }
	}
	
	void Update () {
		
	}


    public void SendNextInputDude()
    {

    }
}
