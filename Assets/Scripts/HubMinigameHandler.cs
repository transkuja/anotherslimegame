using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public enum MessageTypeMinigame {AreYouReady, Lose, AreyoureadyOtherPlayer}

public class HubMinigameHandler : MonoBehaviour {

    public GameObject refCanvasParent;
    private GameObject[] refCanvas = new GameObject[2];
    private GameObject[] BbuttonShown = new GameObject[2];

    GameObject areYouReady;
    private bool[] hasBeenInitialized = new bool[2];

    // tab de message ? 
    private GameObject[][] Message = new GameObject[2][];
    public String[] message;

    public float timerForMinigame = 21.0f;

    public bool hasBeenStarted = false;

    public GameObject TriggerEnd;

    public int currentMessage = 0;

    public Vector3[] initialpos = new Vector3[2];
    public Quaternion[] initialrot = new Quaternion[2];

    public void DisplayMessage(int playerIndex)
    {

        if (hasBeenStarted)
            return;

        if (currentMessage < Message[playerIndex].Length)
        {
            if (Message[playerIndex][currentMessage].gameObject.activeSelf)
            {
                Message[playerIndex][currentMessage].SetActive(false);
                currentMessage++;

                if (currentMessage == Message[playerIndex].Length)
                {
                    AskForReadiness();
                } else
                {
                    Message[playerIndex][currentMessage].SetActive(true);
                }
            }
            else
            {
                Message[playerIndex][currentMessage].SetActive(true);
                BbuttonShown[playerIndex].SetActive(false);
            }
        }
    }

    public void Start()
    {
        if (message.Length > 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Message[i] = new GameObject[message.Length];
            }
        }
        hasBeenInitialized[0] = false;
        hasBeenInitialized[1] = false;
        initialpos[0] = transform.position + Vector3.right * 2;
        initialpos[1] = transform.position + Vector3.right * 4;

        // Change taht
        initialrot[0] = transform.rotation;
        initialrot[1] = transform.rotation;

    }

    public void AskForReadiness()
    {
        // Fade....
        TriggerEnd.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().transform.GetChild(1).gameObject.SetActive(false);
        }

        // Camera
        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        areYouReady = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        if( GameManager.Instance.ActivePlayersAtStart == 2)
        {
            areYouReady.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetMessage(MessageTypeMinigame.AreyoureadyOtherPlayer);
            areYouReady.GetComponent<ReplayScreenControlsHub>().index = 1;
        }
        else
        {
            areYouReady.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetMessage(MessageTypeMinigame.AreYouReady);
            areYouReady.GetComponent<ReplayScreenControlsHub>().index = 0;
        }

        areYouReady.GetComponent<ReplayScreenControlsHub>().refMinigameHandler = this;

    }

    public void LunchMinigameHub()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position = initialpos[i];
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.rotation = initialrot[i];
        }

        Destroy(areYouReady);
        GameObject readySetGo = Instantiate(ResourceUtils.Instance.spriteUtils.spawnableSpriteUI, GameManager.UiReference.transform);
        readySetGo.AddComponent<ReadySetGo>();
     

        hasBeenStarted = true;
        GameManager.Instance.GameFinalTimer = timerForMinigame;
        GameManager.Instance.LaunchFinalTimer();
    }

    public void CleanMinigameHub()
    {
        hasBeenStarted = false;

        //currentMessage = 0; in destroy
        GameManager.Instance.CleanEndFinalCountdown();

        if (areYouReady)
            Destroy(areYouReady);



        GameManager.ChangeState(GameState.Normal);

        for (int playerIndex = 0; playerIndex < GameManager.Instance.PlayerStart.PlayersReference.Count; playerIndex++)
        {
            if (!hasBeenInitialized[playerIndex])
                return;

            DestroyUIMinigame(playerIndex);
        }

    }

    public void Update()
    {
        if (hasBeenStarted)
        {
            //Lose
            //Tp here;
            //if win
            // else

            //StopMinigameHub();

            if (GameManager.Instance.isTimeOver)
            {
                AskRetry();

            }
        }
    }

    public void AskRetry()
    {
        GameManager.Instance.isTimeOver = false;
        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        areYouReady = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);

        areYouReady.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetMessage(MessageTypeMinigame.Lose);
        areYouReady.GetComponent<ReplayScreenControlsHub>().index = 0;

        areYouReady.GetComponent<ReplayScreenControlsHub>().refMinigameHandler = this;
    }

    public void CreateUIHubMinigame(int playerIndex)
    {
        refCanvas[playerIndex] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabCanvasWithUiCameraAdapter, refCanvasParent.transform);
        refCanvas[playerIndex].GetComponent<UICameraApdater>().PlayerIndex = playerIndex;

        for(int i= 0; i < Message[playerIndex].Length; i++)
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

    public void DestroyUIMinigame(int playerIndex)
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

    public String GetMessage(MessageTypeMinigame messageType)
    {
        switch (messageType)
        {
            case MessageTypeMinigame.AreYouReady:
                return "Es tu pret ?";
            case MessageTypeMinigame.AreyoureadyOtherPlayer:
                return "L'autre vient de lancer un minijeu, es tu pret ? ";
            case MessageTypeMinigame.Lose:
                return "Tu as perdu, voulez vous recommencez ? ";
            default:
                return "?????";
        }
    }

    public void WinMinigame()
    {
        CleanMinigameHub();
    }
}
