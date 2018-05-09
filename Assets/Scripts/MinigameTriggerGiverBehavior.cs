using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MinigameTriggerGiverBehavior : PNJDefaultBehavior
{
    public float timer = 31.0f;
    public float delay = 6.0f;

    private float currentCameraForDelay = 0;
    private GameObject retryMessageGo;

    public GameObject triggerEnd;
    private GameObject FadeInAndOut;

    private Vector3[] initialpos = new Vector3[2];
    private Quaternion initialrot;

    private bool hasWin = false;
    private bool isStarted = false;
    private bool startTimerShow = false;

    public CustomizableType rewardType;
    public string currentHatToUnlock;

    protected override void Start()
    {
        messages = new PNJMessages(PNJDialogUtils.GetDefaultMessages(pnjName),
            PNJDialogUtils.GetQuestMessages(pnjName),
            PNJDialogUtils.GetDefaultEmotions(pnjName),
            PNJDialogUtils.GetQuestEmotions(pnjName));

        InitRewards();

        initialpos[0] = transform.position + transform.forward * 4;
        initialpos[1] = transform.position + transform.forward * 4 + transform.right * 2;

        // Change that
        initialrot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);

        if (IsEventOver())
        {
            InactiveCustomzable(rewardType);

            step = messages.QuestMessagesLength();
        } else
        {
            step = 0;
        }
    }

    private void InactiveCustomzable(CustomizableType type)
    {
        switch (type)
        {
            case CustomizableType.Hat:
                GetComponentInChildren<PlayerCosmetics>().Hat = "None";
                break;
        }
    }

    public override void InitNextStep(int playerIndex)
    {
        if (IsEventOver())
            return;

        NextStepCommonProcess(playerIndex);
    }

    protected override void NextStepCommonProcess(int playerIndex)
    {
        //GetComponent<PNJController>().UpdateOriginalPosition();
        if (IsEventOver())
            return;

        AskForReadiness(playerIndex);
    }

    public override string GetNextMessage(int index)
    {
        return messages.GetQuestMessages(step).messages[index];
    }

    public override int GetNextMessagesLength()
    {
        return messages.GetQuestMessages(step).messages.Length;
    }

    public override bool IsEventOver()
    {
        return UnlockInDb(rewardType , currentHatToUnlock);
    }

    public bool UnlockInDb(CustomizableType type, string idToUnlock)
    {
        switch (type)
        {
            case CustomizableType.Color:
                return (DatabaseManager.Db.IsUnlock<DatabaseClass.ColorData>(idToUnlock));
            case CustomizableType.Face:
                return (DatabaseManager.Db.IsUnlock<DatabaseClass.FaceData>(idToUnlock));
            case CustomizableType.Ears:
                return (DatabaseManager.Db.IsUnlock<DatabaseClass.EarsData>(idToUnlock));
            case CustomizableType.Mustache:
                return (DatabaseManager.Db.IsUnlock<DatabaseClass.MustacheData>(idToUnlock));
            case CustomizableType.Hat:
                return (DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>(idToUnlock));
            default:
                Debug.LogError("@Remi : cas non géré");
                break;
        }
        return false;
    }

    protected override void InitRewards()
    {
        rewards = new RewardType[1];
        rewards[0] = new CustomizableReward(transform, rewardType, currentHatToUnlock);
    }

    public bool IsMinigameStarted()
    {
        return startTimerShow || isStarted;
    }

    public void AskForReadiness(int playerIndex)
    {
        EndOtherPlayerDialog(playerIndex);

        retryMessageGo = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        if (GameManager.Instance.ActivePlayersAtStart == 2)
        {
            // other player
            if(playerIndex == 1)
            {
                retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.AreyoureadyPlayer1);
                retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 0;
            }
            else
            {
                retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.AreyoureadyPlayer2);
                retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 1;
            }
        }
        else
        {
            retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.AreYouReady);
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 0;
        }

        retryMessageGo.GetComponent<ReplayScreenControlsHub>().validationFct += PrepareForStart;
        retryMessageGo.GetComponent<ReplayScreenControlsHub>().refusalFct += CleanMinigameHub;


        GameManager.ChangeState(GameState.ForcedPauseMGRules);
    }

    public void PrepareForStart()
    {

        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position = initialpos[i];
            GameManager.Instance.PlayerStart.PlayersReference[i].transform.rotation = initialrot;
            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>().m_RecenterToTargetHeading.m_enabled = true;
        }

        triggerEnd.SetActive(true);

        // Reset Cam at starting pos
        if(triggerEnd.GetComponentInChildren<Cinemachine.CinemachineDollyCart>())
            triggerEnd.GetComponentInChildren<Cinemachine.CinemachineDollyCart>().m_Position = 0;

        Destroy(retryMessageGo);

        step++;


        startTimerShow = true;

    }
    public void StartMinigame()
    {
        GameObject readySetGo = Instantiate(ResourceUtils.Instance.spriteUtils.spawnableSpriteUI, GameManager.UiReference.transform);
        readySetGo.AddComponent<ReadySetGo>();

        isStarted = true;

        GameManager.Instance.GameFinalTimer = timer;
        GameManager.Instance.LaunchFinalTimer();
    }

    public void CleanVariable()
    {
        isStarted = false;
        startTimerShow = false;

        triggerEnd.SetActive(false);

        //currentMessage = 0; in destroy
        GameManager.Instance.CleanEndFinalCountdown();

    }

    public void CleanMinigameHub()
    {
        CleanVariable();

        if (retryMessageGo)
            Destroy(retryMessageGo);

        GameManager.ChangeState(GameState.Normal);
    }

    public void Update()
    {
        if (isStarted)
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
            if (currentCameraForDelay > 3 * delay / 4)
            {
                if (currentCameraForDelay > delay)
                {
                    startTimerShow = false;
                    currentCameraForDelay = 0;
                    StartMinigame();
                }
                else
                {
                    triggerEnd.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            else
            {
                // Fade.... ?
                triggerEnd.transform.GetChild(0).gameObject.SetActive(true);
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
        retryMessageGo.GetComponent<ReplayScreenControlsHub>().validationFct += PrepareForStart;
        retryMessageGo.GetComponent<ReplayScreenControlsHub>().refusalFct += CleanMinigameHub;
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

        GameManager.ChangeState(GameState.Normal);

        // TMP
        GetComponent<PNJMessage>().Message[0].SetActive(true);
        GetComponent<PNJMessage>().currentMessage = 0;
        GetComponent<PNJMessage>().NextMessage(0);

        yield return new WaitForSeconds(2.0f);
        hasWin = true;
        InactiveCustomzable(rewardType);
        rewards[0].GetReward();

        CleanMinigameHub();
        yield return null;
    }

    public void EndOtherPlayerDialog(int playerIndex)
    {
        for(int i =0; i <GameManager.Instance.ActivePlayersAtStart; i++)
        {
            if( i != playerIndex)
            {
                if(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().RefMessage)
                PNJDialogUtils.EndDialog(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().RefMessage.myCharacter, i);
            }
        }
    }
}
