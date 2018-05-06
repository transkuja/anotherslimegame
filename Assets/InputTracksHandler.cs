using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTracksHandler : MonoBehaviour {

    [SerializeField]
    List<GameObject> tracks;

    [SerializeField]
    GameObject inputPrefab;

	public void StartGame() {
        int nbPlayers = GameManager.Instance.CurrentGameMode.curNbPlayers;

        if (nbPlayers == 1)
        {
            transform.GetChild(0).position = (transform.GetChild(1).position + transform.GetChild(2).position) / 2.0f;

        }
        else if (nbPlayers == 2)
        {
            Vector3 tmpPosition = (transform.GetChild(0).position + transform.GetChild(1).position) / 2.0f;
            transform.GetChild(1).position = (transform.GetChild(2).position + transform.GetChild(3).position) / 2.0f;
            transform.GetChild(0).position = tmpPosition;
        }
        else if (nbPlayers == 3)
        {
            transform.GetChild(0).position = (transform.GetChild(0).position + transform.GetChild(1).position) / 2.0f;
            transform.GetChild(1).position = (transform.GetChild(1).position + transform.GetChild(2).position) / 2.0f;
            transform.GetChild(2).position = (transform.GetChild(2).position + transform.GetChild(3).position) / 2.0f;
        }

        for (int i = nbPlayers; i < 4; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < nbPlayers; i++)
        {
            GameObject newInput = Instantiate(inputPrefab, tracks[i].transform.GetChild(3)); // child 3 is start position
            newInput.transform.localPosition = Vector3.zero;
            newInput.GetComponent<FoodInputSettings>().StartGame();
        }
	}
	
	void Update () {
		
	}


    public void SendNextInput(int _trackNumber)
    {
        GameObject newInput = Instantiate(inputPrefab, tracks[_trackNumber].transform.GetChild(3)); // child 3 is start position
        newInput.transform.localPosition = Vector3.zero;
        newInput.GetComponent<FoodInputSettings>().Init();
    }
}
