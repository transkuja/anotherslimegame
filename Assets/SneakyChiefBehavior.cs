using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakyChiefBehavior : MonoBehaviour {

    [SerializeField]
    SneakyChiefMessageContainer[] messageContainer;

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
        if (IsEventOver())
            return;

        if (nextIsABreakable[DatabaseManager.Db.SneakyChiefProgress])
        {
            GameObject pot = Instantiate(sneakyChiefBreakablePrefabs[Random.Range(0, sneakyChiefBreakablePrefabs.Length)], nextTransforms[DatabaseManager.Db.SneakyChiefProgress].position, nextTransforms[DatabaseManager.Db.SneakyChiefProgress].rotation);
            pot.GetComponent<SneakyChiefPot>().Init(gameObject);
            gameObject.SetActive(false);
        }

        transform.position = nextTransforms[DatabaseManager.Db.SneakyChiefProgress].position;
        transform.rotation = nextTransforms[DatabaseManager.Db.SneakyChiefProgress].rotation;
        GetComponent<PNJController>().UpdateOriginalPosition();
    }

    public string GetNextMessage(int _messageIndex)
    {
        return messageContainer[DatabaseManager.Db.SneakyChiefProgress].messages[_messageIndex];
    }

    public int GetNextMessagesLength()
    {
        return messageContainer[DatabaseManager.Db.SneakyChiefProgress].messages.Length;
    }

    public bool IsEventOver()
    {
        return DatabaseManager.Db.SneakyChiefProgress == messageContainer.Length;
    }

    void InitRewards()
    {
        rewards = new RewardType[nextTransforms.Length];
        rewards[0] = new RewardType(transform);
        rewards[1] = new MoneyReward(40, transform);
        rewards[2] = new RuneReward("RuneSneaky1", transform);
    }
}

[System.Serializable]
class SneakyChiefMessageContainer
{
    public string[] messages;
}

[System.Serializable]
class RewardType
{
    [SerializeField]
    public RewardTypeEnum rewardType;
    [SerializeField]
    protected Transform rewardGiver;

    public virtual void GetReward() {}

    public RewardType(Transform _rewardGiver)
    {
        rewardType = RewardTypeEnum.None;
        rewardGiver = _rewardGiver;
    }
}

[System.Serializable]
class MoneyReward : RewardType
{
    int quantity;
    public MoneyReward(int _quantity, Transform _rewardGiver) : base(_rewardGiver)
    {
        rewardType = RewardTypeEnum.Money;
        quantity = _quantity;
    }

    public override void GetReward()
    {
        for (int i = 0; i < quantity; i++)
        {
            GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Money).GetItem(null, rewardGiver.position + Vector3.up * 0.5f, Quaternion.identity, true);
            go.GetComponent<Collectable>().Disperse(i);
        }
    }
}

[System.Serializable]
class RuneReward : RewardType
{
    string runeId;
    public RuneReward(string _runeId, Transform _rewardGiver) : base(_rewardGiver)
    {
        rewardType = RewardTypeEnum.Rune;
        runeId = _runeId;
    }
    public override void GetReward()
    {
        // TODO: play anim get rune
        DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(runeId, true);
        GameManager.Instance.Runes++;
    }
}

[System.Serializable]
class CustomizableReward : RewardType
{

    public CustomizableReward(Transform _rewardGiver, CustomizableType _customizableType, string _customizableId) : base(_rewardGiver)
    {
        rewardType = RewardTypeEnum.Customizable;
        customizableType = _customizableType;
        customizableId = _customizableId;
    }

    [SerializeField]
    CustomizableType customizableType;
    [SerializeField]
    string customizableId;

    public override void GetReward()
    {
        // TODO: play anim get new customizable

        switch (customizableType)
        {
            case CustomizableType.Color:
                DatabaseManager.Db.SetUnlock<DatabaseClass.ColorData>(customizableId, true);
                break;
            case CustomizableType.Face:
                DatabaseManager.Db.SetUnlock<DatabaseClass.FaceData>(customizableId, true);
                break;
            case CustomizableType.Ears:
                DatabaseManager.Db.SetUnlock<DatabaseClass.EarsData>(customizableId, true);
                break;
            case CustomizableType.Hat:
                DatabaseManager.Db.SetUnlock<DatabaseClass.HatData>(customizableId, true);
                break;
            case CustomizableType.Mustache:
                DatabaseManager.Db.SetUnlock<DatabaseClass.MustacheData>(customizableId, true);
                break;
        }
    }
}
