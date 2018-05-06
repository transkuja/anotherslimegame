using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTracksHandler : MonoBehaviour {

    [SerializeField]
    List<GameObject> tracks;

    [SerializeField]
    GameObject inputPrefab;

    public GameObject board; // TMP

    public Vector3[] ComputePlayerStartingPositions(int _nbPlayers)
    {
        Vector3[] positions = new Vector3[_nbPlayers];
        if (_nbPlayers == 1)
        {
            positions[0] = 
                board.transform.GetChild(0).position = (board.transform.GetChild(1).position + board.transform.GetChild(2).position) / 2.0f;
        }
        else if (_nbPlayers == 2)
        {
            positions[0] = 
                board.transform.GetChild(0).position = (board.transform.GetChild(0).position + board.transform.GetChild(1).position) / 2.0f;
            positions[1] =
                board.transform.GetChild(1).position = (board.transform.GetChild(2).position + board.transform.GetChild(3).position) / 2.0f;
        }
        else if (_nbPlayers == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                positions[i] =
                    board.transform.GetChild(i).position = (board.transform.GetChild(i).position + board.transform.GetChild(i + 1).position) / 2.0f;
            }
        }

        return positions;
    }

	public void StartGame() {
        int nbPlayers = GameManager.Instance.CurrentGameMode.curNbPlayers;

        if (nbPlayers == 1)
        {
            transform.GetChild(0).position = (transform.GetChild(1).position + transform.GetChild(2).position) / 2.0f;
        }
        else if (nbPlayers == 2)
        {
            transform.GetChild(0).position = (transform.GetChild(0).position + transform.GetChild(1).position) / 2.0f;
            transform.GetChild(1).position = (transform.GetChild(2).position + transform.GetChild(3).position) / 2.0f;
        }
        else if (nbPlayers == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                transform.GetChild(i).position = (transform.GetChild(i).position + transform.GetChild(i + 1).position) / 2.0f;
            }
        }

        for (int i = nbPlayers; i < 4; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        for (int i = 0; i < nbPlayers; i++)
        {
            GameObject newInput = Instantiate(inputPrefab, tracks[i].transform.GetChild(3)); // child 3 is start position
            newInput.transform.localPosition = Vector3.zero;
            newInput.GetComponent<FoodInputSettings>().StartGame();

            board.transform.GetChild(i).gameObject.SetActive(true);
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
