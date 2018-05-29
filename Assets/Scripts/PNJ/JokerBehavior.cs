using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerBehavior : PNJDefaultBehavior {

    protected override void Start()
    {
        base.Start();
    }

    public override void InitNextStep(int playerIndex = 0)
    {
        rewards[DatabaseManager.Db.JokerProgress].GetReward();
        step = ++DatabaseManager.Db.JokerProgress;

        if (IsEventOver())
            return;

        NextStepCommonProcess();

    }

    protected override void NextStepCommonProcess(int playerIndex = 0)
    {
        if (IsEventOver())
            return;

        GetComponent<PNJController>().UpdateOriginalPosition();
    }

    public override string GetNextMessage(int index)
    {
        string nextMsg = messages.GetQuestMessages(DatabaseManager.Db.JokerProgress).messages[index];

        if (nextMsg.StartsWith("1"))
        {
            if (AudioManager.Instance != null && AudioManager.Instance.drumrollFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.drumrollFx);

            nextMsg = nextMsg.Remove(0, 1);
            GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Attack;
            isLocked = true;
            Invoke("UnlockDialog", AudioManager.Instance.drumrollFx.length);
        }
        if (nextMsg.StartsWith("2"))
        {
            if (AudioManager.Instance != null && AudioManager.Instance.badumtssFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.badumtssFx, 4.0f, 1.0f);

            nextMsg = nextMsg.Remove(0, 1);
            GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Winner;
            isLocked = true;
            Invoke("UnlockDialog", AudioManager.Instance.badumtssFx.length);
        }
        else
            GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Neutral;

        return nextMsg;
    }

    void UnlockDialog()
    {
        isLocked = false;
    }

    public override int GetNextMessagesLength()
    {
        return messages.GetQuestMessages(DatabaseManager.Db.JokerProgress).messages.Length;
    }

    public override bool IsEventOver()
    {
        return DatabaseManager.Db.JokerProgress == messages.QuestMessagesLength();
    }

    protected override void InitRewards()
    {
        rewards = new RewardType[messages.QuestMessagesLength()];
        for (int i = 0; i < 7; i++)
            rewards[i] = new MoneyReward(2, transform);
        rewards[7] = new CustomizableReward(transform, CustomizableType.Hat, "Glitter");
        for (int i = 8; i < 17; i++)
            rewards[i] = new MoneyReward(4, transform);
        rewards[17] = new RuneReward("RuneJoker1", transform);
    }
}
