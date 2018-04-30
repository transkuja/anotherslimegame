using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum MessageTypeMinigame {AreYouReady, Retry, AreyoureadyOtherPlayer}

public class HubMinigameHandler : MonoBehaviour {

    public GameObject refCanvasParent;
    public GameObject TriggerEnd;
    public String[] message;
    public String victoryMessage;
    [Tooltip("Temps donné au joueur")]
    public float timerForMinigame = 21.0f;
    [Tooltip("Delay à vol de camera")]
    public float delay = 5; // t = d/v
    private bool hasWin = false;
    public bool hasBeenStarted = false;

    public GameObject toDesactivate;

    public CustomizableType t;
    public String id;

    private bool startTimerShow = false;

    private float currentCameraForDelay = 0;

    private GameObject[] refCanvas = new GameObject[2];
    private GameObject[] BbuttonShown = new GameObject[2];

    private GameObject retryMessageGo;
    private bool[] hasBeenInitialized = new bool[2];

    // tab de message ? 
    private GameObject[][] Message = new GameObject[2][];

    private int currentMessage = 0;
    private Vector3[] initialpos = new Vector3[2];
    private Quaternion initialrot;
    private GameObject FadeInAndOut;

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
        initialpos[0] = transform.position + transform.forward * 4;
        initialpos[1] = transform.position + transform.forward * 4 + transform.right *2;

        // Change taht
        initialrot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);

        if (t == CustomizableType.Hat && id != "")
        {
            if (DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>(id))
                //toDesactivate.SetActive(false);
                GetComponentInChildren<NewPlayerCosmetics>().Hat = "None";
        }

    }

    public void DisplayMessage(int playerIndex)
    {
        if (hasBeenStarted)
            return;

        if (!hasBeenInitialized[playerIndex])
            return;

        if (currentMessage < Message[playerIndex].Length)
        {
            transform.LookAt(new Vector3(GameManager.Instance.PlayerStart.PlayersReference[playerIndex].transform.position.x, transform.position.y, GameManager.Instance.PlayerStart.PlayersReference[playerIndex].transform.position.z));

            if (Message[playerIndex][currentMessage].gameObject.activeSelf)
            {
                Message[playerIndex][currentMessage].SetActive(false);
                currentMessage++;

                if (currentMessage == Message[playerIndex].Length)
                {
                    AskForReadiness();
                }
                else
                {
                    Message[playerIndex][currentMessage].SetActive(true);
                }
            }
            else
            {
                Message[playerIndex][currentMessage].SetActive(true);
                BbuttonShown[playerIndex].SetActive(false);
                GameManager.ChangeState(GameState.ForcedPauseMGRules);

                PlayerCharacterHub pc = GameManager.Instance.PlayerStart.PlayersReference[playerIndex].GetComponent<PlayerCharacterHub>();
                pc.Rb.drag = 25.0f;
                pc.Rb.velocity = Vector3.zero;
            }
        }
    }

    public void AskForReadiness()
    {
        retryMessageGo = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        if( GameManager.Instance.ActivePlayersAtStart == 2)
        {
            retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.AreyoureadyOtherPlayer);
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 1;
        }
        else
        {
            retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.AreYouReady);
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 0;
        }

        retryMessageGo.GetComponent<ReplayScreenControlsHub>().refMinigameHandler = this;

    }

    public void PrepareForStart()
    {

        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position = initialpos[i];
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.rotation = initialrot;
            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>().m_RecenterToTargetHeading.m_enabled = true;
        }
        TriggerEnd.SetActive(true);
        Destroy(retryMessageGo);
   
       startTimerShow = true;

    }
    public void StartMinigame()
    {
        hasBeenStarted = true;

        GameObject readySetGo = Instantiate(ResourceUtils.Instance.spriteUtils.spawnableSpriteUI, GameManager.UiReference.transform);
        readySetGo.AddComponent<ReadySetGo>();


        GameManager.Instance.GameFinalTimer = timerForMinigame;
        GameManager.Instance.LaunchFinalTimer();
    }

    public void CleanVariable()
    {
        hasBeenStarted = false;
        startTimerShow = false;

        TriggerEnd.SetActive(false);

        //currentMessage = 0; in destroy
        GameManager.Instance.CleanEndFinalCountdown();

    }

    public void CleanMinigameHub()
    {
        CleanVariable();

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
                if (!hasWin)
                {
                    AskRetry();
                }
            }
        }
        else if (startTimerShow)
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
            }
            else
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
        CleanVariable();

        GameManager.ChangeState(GameState.ForcedPauseMGRules);

        retryMessageGo = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.Retry);
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

    public void WinMinigame()
    {
        FadeInAndOut = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabFadeInAndOut, GameManager.UiReference.transform);
        FadeInAndOut.SetActive(false);

        CleanVariable();

        StartCoroutine(GiveReward());
    }

    public IEnumerator GiveReward()
    {
        FadeInAndOut.SetActive(true);
        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        yield return new WaitUntil(
            () => FadeInAndOut.GetComponent<UiFadeInAndOut>().halfwaythere == true
        );

        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position = initialpos[i];
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.rotation = initialrot;
            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>().m_RecenterToTargetHeading.m_enabled = true;
        }
        yield return new WaitUntil(
          () => FadeInAndOut == null
         );
        GameObject[] tmpMessage = new GameObject[GameManager.Instance.PlayerStart.PlayersReference.Count];
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            if (refCanvas[i] == null)
            {
                refCanvas[i] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabCanvasWithUiCameraAdapter, refCanvasParent.transform);
                refCanvas[i].GetComponent<UICameraApdater>().PlayerIndex = i;
            }
            tmpMessage[i] = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabMessage, refCanvas[i].transform);

            tmpMessage[i].transform.GetChild(2).GetComponent<Text>().text = GetComponent<Player>().playerName;
            tmpMessage[i].transform.GetChild(3).GetComponent<Text>().text = victoryMessage;
            tmpMessage[i].SetActive(true);
        }

        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            if (tmpMessage[i])
            {
                Destroy(tmpMessage[i]);
            }
            if (refCanvas[i])
                Destroy(refCanvas[i]);
        }

        GameManager.ChangeState(GameState.Normal);

        // Local equivalent
        hasWin = true;
        if (t == CustomizableType.Hat && id != "")
        {
            DatabaseManager.Db.SetUnlock<DatabaseClass.HatData>(id, true);
            //toDesactivate.SetActive(false);
            GetComponentInChildren<NewPlayerCosmetics>().Hat = "None";
        }

        CleanMinigameHub();
        yield return null;
    }
}
