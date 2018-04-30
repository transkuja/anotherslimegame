using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BobBehavior : PNJDefaultBehavior
{
    private int step = 0;

    public float timer = 31.0f;
    public float delay = 5.0f;

    private float currentCameraForDelay = 0;
    private GameObject retryMessageGo;

    public GameObject triggerEnd;
    public GameObject toDesactivate;
    private GameObject FadeInAndOut;

    private Vector3[] initialpos = new Vector3[2];
    private Quaternion initialrot;

    private bool hasWin = false;
    private bool isStarted = false;
    private bool startTimerShow = false;

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

        if (DatabaseManager.Db.IsUnlock<DatabaseClass.HatData>("Cowboy"))
        {
            toDesactivate.SetActive(false);
            step = messages.QuestMessagesLength();
        } else
        {
            step = 0;
        }
    }

    public override void InitNextStep()
    {
        if (IsEventOver())
            return;

        NextStepCommonProcess();
    }

    protected override void NextStepCommonProcess()
    {
        //GetComponent<PNJController>().UpdateOriginalPosition();
        if (IsEventOver())
            return;

        AskForReadiness();
    }

    public override string GetNextMessage(int _messageIndex)
    {
        return messages.GetQuestMessages(step).messages[_messageIndex];
    }

    public override int GetNextMessagesLength()
    {
        return messages.GetQuestMessages(step).messages.Length;
    }

    public override bool IsEventOver()
    {
        return step == messages.QuestMessagesLength();
    }

    protected override void InitRewards()
    {
        rewards = new RewardType[1];
        rewards[0] = new CustomizableReward(transform, CustomizableType.Hat, "Cowboy");
    }

    public bool IsMinigameStarted()
    {
        return startTimerShow || isStarted;
    }

    public void AskForReadiness()
    {
        retryMessageGo = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        if (GameManager.Instance.ActivePlayersAtStart == 2)
        {
            retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.AreyoureadyOtherPlayer);
            retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = 1;
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
        Destroy(retryMessageGo);

        step++;


        startTimerShow = true;

    }
    public void StartMinigame()
    {
        GameObject readySetGo = Instantiate(ResourceUtils.Instance.spriteUtils.spawnableSpriteUI, GameManager.UiReference.transform);

        //GameManager.ChangeState(GameState.Normal);
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

        for (int playerIndex = 0; playerIndex < GameManager.Instance.PlayerStart.PlayersReference.Count; playerIndex++)
        {
            if (!GetComponent<PNJDefaultMessage>().hasBeenInitialized[playerIndex])
                return;

            GetComponent<PNJDefaultMessage>().DestroyUIMessage(playerIndex);
        }


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
                    triggerEnd.transform.GetChild(0).gameObject.SetActive(false);
                    for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
                    {
                        GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            }
            else
            {
                // Fade.... ?
                triggerEnd.transform.GetChild(0).gameObject.SetActive(true);
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
        GetComponent<PNJDefaultMessage>().CreateUIMessage(0);
       GetComponent<PNJDefaultMessage>().DisplayMessage(0);

        hasWin = true;
        toDesactivate.SetActive(false);
        rewards[0].GetReward();

        CleanMinigameHub();
        yield return null;
    }
}
