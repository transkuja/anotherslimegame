using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerBehavior : PNJDefaultBehavior {

    protected override void Start()
    {
        base.Start();
    }

    public override void InitNextStep()
    {
        rewards[DatabaseManager.Db.JokerProgress].GetReward();
        DatabaseManager.Db.JokerProgress++;

        if (IsEventOver())
            return;

        NextStepCommonProcess();

    }

    protected override void NextStepCommonProcess()
    {
        GetComponent<PNJController>().UpdateOriginalPosition();

        if (IsEventOver())
            return;
    }

    public override string GetNextMessage(int _messageIndex)
    {
        return messages.GetQuestMessages(DatabaseManager.Db.JokerProgress).messages[_messageIndex];
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
    }
}
