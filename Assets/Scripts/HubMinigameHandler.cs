using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public enum MessageTypeMinigame {AreYouReady, Lose, AreyoureadyOtherPlayer}

public class HubMinigameHandler : MonoBehaviour {

    public GameObject refCanvasParent;
    public GameObject TriggerEnd;
    public String[] message;
    [Tooltip("Temps donné au joueur")]
    public float timerForMinigame = 21.0f;
    [Tooltip("Delay à vol de camera")]
    public float delay = 5; // t = d/v
    private bool startTimerShow = false;

    private float currentCameraForDelay = 0;

    private GameObject[] refCanvas = new GameObject[2];
    private GameObject[] BbuttonShown = new GameObject[2];

    private GameObject retryMessageGo;
    private bool[] hasBeenInitialized = new bool[2];

    // tab de message ? 
    private GameObject[][] Message = new GameObject[2][];
    private bool hasBeenStarted = false;
    private int currentMessage = 0;
    private Vector3[] initialpos = new Vector3[2];
    private Quaternion[] initialrot = new Quaternion[2];
    private bool hasWin;

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
        // Camera
        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        retryMessageGo = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        if( GameManager.Instance.ActivePlayersAtStart == 2)
        {
            retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetRetryMessage(MessageTypeMinigame.AreyoureadyOtherPlayer);
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 1;
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().needShow = true;
        }
        else
        {
            retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetRetryMessage(MessageTypeMinigame.AreYouReady);
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 0;
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().needShow = true;
        }

        retryMessageGo.GetComponent<ReplayScreenControlsHub>().refMinigameHandler = this;

    }

    public void PrepareForStart(bool needShow = false)
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position = initialpos[i];
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.rotation = initialrot[i];
        }

        Destroy(retryMessageGo);
   
        // after delay
        if (needShow)
        {
            startTimerShow = true;
        }
        else
        {
            StartMinigame();
        }

    }
    public void StartMinigame()
    {
        hasBeenStarted = true;
        GameObject readySetGo = Instantiate(ResourceUtils.Instance.spriteUtils.spawnableSpriteUI, GameManager.UiReference.transform);
        readySetGo.AddComponent<ReadySetGo>();


        GameManager.Instance.GameFinalTimer = timerForMinigame;
        GameManager.Instance.LaunchFinalTimer();
    }


    public void CleanMinigameHub()
    {
        hasBeenStarted = false;
        startTimerShow = false;

        //currentMessage = 0; in destroy
        GameManager.Instance.CleanEndFinalCountdown();

        if (retryMessageGo)
            Destroy(retryMessageGo);

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
            if (GameManager.Instance.isTimeOver)
            {
                if(!hasWin)
                {
                    AskRetry();
                }
            }
        } else if (startTimerShow)
        {
            currentCameraForDelay += Time.deltaTime;

            // Once ????????
            if (currentCameraForDelay > delay / 2)
            {
                if (currentCameraForDelay > delay)
                {
                    startTimerShow = false;
                    currentCameraForDelay = 0;
                    StartMinigame();
                }
                else
                {
                    TriggerEnd.transform.GetChild(0).gameObject.SetActive(false);
                    for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
                    {
                        GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            
       
            } else
            {
                // Fade.... ?
                TriggerEnd.transform.GetChild(0).gameObject.SetActive(true);
                for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
                {
                    GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().transform.GetChild(1).gameObject.SetActive(false);
                }

            }


        }
    }

    public void AskRetry()
    {
        // sert aussi de bool pour le retry
        GameManager.Instance.isTimeOver = false;

        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        retryMessageGo = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);

        retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetRetryMessage(MessageTypeMinigame.Lose);
        retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 0;

        retryMessageGo.GetComponent<ReplayScreenControlsHub>().refMinigameHandler = this;
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

    public String GetRetryMessage(MessageTypeMinigame messageType)
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

    public void GiveReward()
    {
        // TP back
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position = initialpos[i];
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.rotation = initialrot[i];

            //Instantiate(ResourceUtils.Instance.feedbacksManager.prefabMessage, refCanvas[i].transform);
            //Message[i][0].transform.GetChild(2).GetComponent<Text>().text = GetComponent<Player>().playerName;
            //Message[i][0].transform.GetChild(3).GetComponent<Text>().text = "Bravooooo !!!!";
            //Message[i][0].SetActive(true);
        }

        //if (GetComponent<CreateEnumFromDatabase>() == null)
        //{
        //    Debug.LogError("Attract fct : It's a rune, it need a createEnumFromDatabase component link to the associated rune");
        //    return;
        //}
        //string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
        //DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(s, true);
    }

    public void WinMinigame()
    {
        hasWin = true;
        GiveReward();

        // after delay
        CleanMinigameHub();
    }
}
