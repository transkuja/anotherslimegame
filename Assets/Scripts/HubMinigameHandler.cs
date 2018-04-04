using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public class HubMinigameHandler : MonoBehaviour {

    public GameObject refCanvasParent;
    private GameObject[] refCanvas = new GameObject[2];
    private GameObject[] BbuttonShown = new GameObject[2];

    // tab de message ? 
    private GameObject[] Message = new GameObject[2];
    public String message;

    public float timerForMinigame = 21.0f;

    public GameObject TriggerEnd;
   
    public void DisplayMessage(int playerIndex)
    {
        if(Message[playerIndex].gameObject.activeSelf)
        {
            Message[playerIndex].SetActive(false);
            LunchMinigameHub();
        }
        else
        {
            Message[playerIndex].SetActive(true);
            BbuttonShown[playerIndex].SetActive(false);
        }

    }

    public void LunchMinigameHub()
    {
        // Fade....
        // Camera

        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        GameObject readySetGo = Instantiate(ResourceUtils.Instance.spriteUtils.spawnableSpriteUI, GameManager.UiReference.transform);
        readySetGo.AddComponent<ReadySetGo>();

        GameManager.Instance.GameFinalTimer = timerForMinigame;
        GameManager.Instance.LaunchFinalTimer();
    }

    public void StopMinigameHub()
    {
        Debug.Log("fin minijeu");
    }

    public void Update()
    {
        if(GameManager.Instance.isTimeOver)
        {
            // Lose
            StopMinigameHub();
        }
    }

    internal void CreateUIHubMinigame(int playerIndex)
    {
        refCanvas[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabCanvasWithUiCameraAdapter, refCanvasParent.transform);
        refCanvas[playerIndex].GetComponent<UICameraApdater>().PlayerIndex = playerIndex;

        Message[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabMessage, refCanvas[playerIndex].transform);
        Message[playerIndex].transform.GetChild(2).GetComponent<Text>().text = GetComponent<Player>().playerName;
        Message[playerIndex].transform.GetChild(3).GetComponent<Text>().text = message;
        Message[playerIndex].SetActive(false);

        BbuttonShown[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabBButton, refCanvas[playerIndex].transform);
        BbuttonShown[playerIndex].SetActive(true);

        if (playerIndex == 0)
        {
            refCanvas[playerIndex].layer = LayerMask.NameToLayer("CameraP1");
        }

        if (playerIndex == 1)
        {
            refCanvas[playerIndex].layer = LayerMask.NameToLayer("CameraP2");
        }
    }

    internal void DestroyUIMinigame(int playerIndex)
    {
        Destroy(Message[playerIndex]);
        Destroy(BbuttonShown[playerIndex]);
        Destroy(refCanvas[playerIndex]);
    }
}
