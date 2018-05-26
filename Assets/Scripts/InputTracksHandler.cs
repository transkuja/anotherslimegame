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


        for (int i = 0; i < nbPlayers; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);

            GetComponentsInChildren<FoodInputSettings>()[i].Init();
            GetComponentsInChildren<PlayerControllerFood>()[i].playerIndex = (UWPAndXInput.PlayerIndex)i;

            board.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
