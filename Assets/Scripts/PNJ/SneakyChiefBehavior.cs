using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakyChiefBehavior : PNJDefaultBehavior
{
    [SerializeField]
    GameObject[] sneakyChiefBreakablePrefabs;

    // Next transform
    [SerializeField]
    Transform[] nextTransforms;

    // next transform need something to be broken
    bool[] nextIsABreakable = { false, false, true, false, false, true, true };

    protected override void Start()
    {
        base.Start();
    }

    public override void InitNextStep(int playerIndex = 0)
    {
        rewards[DatabaseManager.Db.SneakyChiefProgress].GetReward();
        step = ++DatabaseManager.Db.SneakyChiefProgress;

        if (IsEventOver())
            return;

        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
        NextStepCommonProcess();

    }

    protected override void NextStepCommonProcess(int playerIndex = 0)
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
        return DatabaseManager.Db.SneakyChiefProgress == messages.QuestMessagesLength();
    }

    protected override void InitRewards()
    {
        rewards = new RewardType[nextTransforms.Length];
        rewards[0] = new RewardType(transform);
        rewards[1] = new MoneyReward(40, transform);
        rewards[2] = new RuneReward("RuneSneaky1", transform);
        rewards[3] = new MoneyReward(40, transform);
        rewards[4] = new MoneyReward(40, transform);
        rewards[5] = new MoneyReward(50, transform);
        rewards[6] = new RuneReward("RuneSneaky2", transform);
    }
}
