using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VendorBehavior : PNJDefaultBehavior {

    private float currentCameraForDelay = 0;
    private GameObject retryMessageGo;

    private Vector3[] initialpos = new Vector3[2];
    private Quaternion initialrot;

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
    }


    public override void InitNextStep(int playerIndex)
    {
        if (IsEventOver())
            return;

        NextStepCommonProcess(playerIndex);
    }

    protected override void NextStepCommonProcess(int playerIndex)
    {
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

    public void AskForReadiness(int playerIndex)
    {
        EndOtherPlayerDialog(playerIndex);

        // Stop all players from moving
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>().Rb.drag = 25.0f;
            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<PlayerCharacterHub>().Rb.velocity = Vector3.zero;
        }

        retryMessageGo = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        if (GameManager.Instance.ActivePlayersAtStart == 2)
        {
            // other player
            if (playerIndex == 1)
            {
                retryMessageGo.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = Utils.GetRetryMessage(MessageTypeMinigame.AreyoureadyPlayer1);
                retryMessageGo.GetComponent<ReplayScreenControlsHub>().index = playerIndex;
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

        //retryMessageGo.GetComponent<ReplayScreenControlsHub>().validationFct += PrepareForStart;
        //retryMessageGo.GetComponent<ReplayScreenControlsHub>().refusalFct += CleanMinigameHub;


        GameManager.ChangeState(GameState.ForcedPauseMGRules);
    }

    void GoToShop()
    {
        SlimeDataContainer.instance.isInTheShop = true;
        LevelLoader.LoadLevelWithFadeOut("Menu");
    }

    public void EndOtherPlayerDialog(int playerIndex)
    {
        for (int i = 0; i < GameManager.Instance.ActivePlayersAtStart; i++)
        {
            if (i != playerIndex)
            {
                if (GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().RefMessage)
                    PNJDialogUtils.EndDialog(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().RefMessage.myCharacter, i);
            }
        }
    }
}
