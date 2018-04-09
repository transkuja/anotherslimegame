using System;
using UnityEngine;
using UnityEngine.UI;

public class PNJDefaultMessage : MonoBehaviour {

    public GameObject refCanvasParent;

    public String[] message;
    private GameObject[] refCanvas = new GameObject[2];
    private GameObject[] BbuttonShown = new GameObject[2];

    private GameObject retryMessageGo;
    private bool[] hasBeenInitialized = new bool[2];

    // tab de message ? 
    private GameObject[][] Message = new GameObject[2][];

    private int currentMessage = 0;
    private Vector3[] initialpos = new Vector3[2];

    // Use this for initialization
    void Start () {
        if (message.Length > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Message[i] = new GameObject[message.Length];
            }
        }
        hasBeenInitialized[0] = false;
        hasBeenInitialized[1] = false;
        initialpos[0] = transform.position + transform.forward * 4;
        initialpos[1] = transform.position + transform.forward * 4 + transform.right * 2;
    }


    public void CreateUIMessage(int playerIndex)
    {
        refCanvas[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabCanvasWithUiCameraAdapter, refCanvasParent.transform);
        refCanvas[playerIndex].GetComponent<UICameraApdater>().PlayerIndex = playerIndex;

        for (int i = 0; i < Message[playerIndex].Length; i++)
        {
            Message[playerIndex][i] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabMessage, refCanvas[playerIndex].transform);
            Message[playerIndex][i].transform.GetChild(2).GetComponent<Text>().text = GetComponent<Player>().playerName;
            Message[playerIndex][i].transform.GetChild(3).GetComponent<Text>().text = message[i];
            Message[playerIndex][i].SetActive(false);
        }

        BbuttonShown[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabBButton, refCanvas[playerIndex].transform);
        BbuttonShown[playerIndex].SetActive(true);

        refCanvas[playerIndex].layer = LayerMask.NameToLayer((playerIndex == 0) ? "CameraP1" : "CameraP2");
        hasBeenInitialized[playerIndex] = true;
    }

    public void DestroyUIMessage(int playerIndex)
    {
        currentMessage = 0;
        for (int i = 0; i < Message[playerIndex].Length; i++)
        {
            Destroy(Message[playerIndex][i]);
        }
        Destroy(BbuttonShown[playerIndex]);
        Destroy(refCanvas[playerIndex]);
        hasBeenInitialized[playerIndex] = false;
    }

    public void DisplayMessage(int playerIndex)
    {
        if (!hasBeenInitialized[playerIndex])
            return;

        if (currentMessage < Message[playerIndex].Length)
        {
            transform.LookAt(new Vector3(GameManager.Instance.PlayerStart.PlayersReference[playerIndex].transform.position.x, transform.position.y, GameManager.Instance.PlayerStart.PlayersReference[playerIndex].transform.position.z));

            if (Message[playerIndex][currentMessage].gameObject.activeSelf)
            {
                Message[playerIndex][currentMessage].SetActive(false);
                currentMessage++;

                if (currentMessage < Message[playerIndex].Length)
                {
                    Message[playerIndex][currentMessage].SetActive(true);
                }
                if( currentMessage == Message[playerIndex].Length)
                    GameManager.ChangeState(GameState.Normal);
            }
            else
            {
                Message[playerIndex][currentMessage].SetActive(true);
                BbuttonShown[playerIndex].SetActive(false);
                GameManager.ChangeState(GameState.ForcedPauseMGRules);
            }
        }
    }
}
