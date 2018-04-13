using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakyChiefBehavior : MonoBehaviour {

    [SerializeField]
    PNJMessages messages;

    [SerializeField]
    GameObject[] sneakyChiefBreakablePrefabs;

    // Reward list
    RewardType[] rewards;

    // Next transform
    [SerializeField]
    Transform[] nextTransforms;

    // next transform need something to be broken
    bool[] nextIsABreakable = { false, false, true };

    private void Start()
    {
        messages = new PNJMessages(PNJDialogUtils.GetDefaultMessages(PNJName.SneakyChief),
            PNJDialogUtils.GetQuestMessages(PNJName.SneakyChief),
            PNJDialogUtils.GetDefaultEmotions(PNJName.SneakyChief),
            PNJDialogUtils.GetQuestEmotions(PNJName.SneakyChief));

        InitRewards();

        // TODO: Load current step from database    
        NextStepCommonProcess();
    }

    public void InitNextStep()
    {
        rewards[DatabaseManager.Db.SneakyChiefProgress].GetReward();
        DatabaseManager.Db.SneakyChiefProgress++;

        if (IsEventOver())
            return;

        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
        NextStepCommonProcess();

    }

    void NextStepCommonProcess()
    {
        transform.position = nextTransforms[(DatabaseManager.Db.SneakyChiefProgress == nextTransforms.Length) ? nextTransforms.Length - 1 : DatabaseManager.Db.SneakyChiefProgress].position;
        transform.rotation = nextTransforms[(DatabaseManager.Db.SneakyChiefProgress == nextTransforms.Length) ? nextTransforms.Length - 1 : DatabaseManager.Db.SneakyChiefProgress].rotation;
        GetComponent<PNJController>().UpdateOriginalPosition();

        if (IsEventOver())
            return;

        if (nextIsABreakable[DatabaseManager.Db.SneakyChiefProgress])
        {
            GameObject pot = Instantiate(sneakyChiefBreakablePrefabs[Random.Range(0, sneakyChiefBreakablePrefabs.Length)], nextTransforms[DatabaseManager.Db.SneakyChiefProgress].position, nextTransforms[DatabaseManager.Db.SneakyChiefProgress].rotation);
            pot.GetComponent<SneakyChiefPot>().Init(gameObject);
            gameObject.SetActive(false);
        }
    }

    public string GetNextMessage(int _messageIndex)
    {
        return messages.GetQuestMessages(DatabaseManager.Db.SneakyChiefProgress).messages[_messageIndex];
    }

    public int GetNextMessagesLength()
    {
        return messages.GetQuestMessages(DatabaseManager.Db.SneakyChiefProgress).messages.Length;
    }

    public bool IsEventOver()
    {
        return DatabaseManager.Db.SneakyChiefProgress == messages.QuestMessagesNbr();
    }

    void InitRewards()
    {
        rewards = new RewardType[nextTransforms.Length];
        rewards[0] = new RewardType(transform);
        rewards[1] = new MoneyReward(40, transform);
        rewards[2] = new RuneReward("RuneSneaky1", transform);
    }
}
