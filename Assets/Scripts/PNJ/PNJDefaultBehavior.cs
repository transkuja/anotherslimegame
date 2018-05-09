using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PNJDefaultBehavior : MonoBehaviour {

    [SerializeField]
    protected PNJMessages messages;

    protected int step;

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

    public virtual void InitNextStep(int playerIndex = 0)
    {

    }

    protected virtual void NextStepCommonProcess(int playerIndex = 0)
    {
        GetComponent<PNJController>().UpdateOriginalPosition();
    }

    public virtual string GetNextMessage(int index)
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

    public void Reset()
    {
        step = 0;
    }
}
