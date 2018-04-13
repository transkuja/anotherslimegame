using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJDefaultBehavior : MonoBehaviour {

    [SerializeField]
    protected PNJMessages messages;

    public PNJName pnjName;

    // Reward list
    protected RewardType[] rewards;

    protected virtual void Start()
    {
        messages = new PNJMessages(PNJDialogUtils.GetDefaultMessages(pnjName),
            PNJDialogUtils.GetQuestMessages(pnjName),
            PNJDialogUtils.GetDefaultEmotions(pnjName),
            PNJDialogUtils.GetQuestEmotions(pnjName));

        InitRewards();

        // TODO: Load current step from database    
        NextStepCommonProcess();
    }

    public virtual void InitNextStep()
    {

    }

    protected virtual void NextStepCommonProcess()
    {
        GetComponent<PNJController>().UpdateOriginalPosition();
    }

    public virtual string GetNextMessage(int _messageIndex)
    {
        return "";
    }

    public virtual int GetNextMessagesLength()
    {
        return 0;
    }

    public virtual bool IsEventOver()
    {
        return false;
    }

    protected virtual void InitRewards()
    {
    }
}
