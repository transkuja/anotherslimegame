using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardType
{
    [SerializeField]
    public RewardTypeEnum rewardType;
    [SerializeField]
    protected Transform rewardGiver;

    public virtual void GetReward() { }

    public RewardType(Transform _rewardGiver)
    {
        rewardType = RewardTypeEnum.None;
        rewardGiver = _rewardGiver;
    }
}

[System.Serializable]
public class MoneyReward : RewardType
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
public class RuneReward : RewardType
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
public class CustomizableReward : RewardType
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