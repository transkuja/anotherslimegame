using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UWPAndXInput;

public enum CostAreaType { PayAndGetItem, PayAndCallEvent }
public enum CostAreaEvent { None, EndGame, IncreaseWater }
public class CostArea : MonoBehaviour {
    [SerializeField]
    CostAreaType costAreaType;

    [SerializeField]
    CostAreaEvent costAreaEvent;

    [SerializeField]
    CollectableType currency;
    
    [SerializeField]
    int cost;

    [SerializeField]
    bool isActive = true;

    [Header("Children references")]
    [SerializeField]
    Text costText;
    [SerializeField]
    Image currencyLogo;
    [SerializeField]
    Transform rewardPreview;

    [Header("Reward")]
    [SerializeField]
    CollectableType rewardType;
    [SerializeField]
    int rewardQuantity;

    [Header("Currency Sprites")]
    [SerializeField]
    Sprite currencyCoin;
    [SerializeField]
    Sprite currencyRune;

    [Header("Reward model")]
    [SerializeField]
    GameObject rewardKey;
    [SerializeField]
    GameObject rewardPlatformist;
    [SerializeField]
    GameObject rewardGhost;
    [SerializeField]
    GameObject rewardStrength;
    [SerializeField]
    GameObject rewardAgility;
    [SerializeField]
    GameObject rewardVictory;
    [SerializeField]
    GameObject rewardIncreaseWater;

    public CollectableType Currency
    {
        get
        {
            return currency;
        }
    }

    public int Cost
    {
        get
        {
            return cost;
        }
    }

    public void Start()
    {
        Init();
    }

    void Init()
    {
        costText.text = "x " + cost;
        currencyLogo.sprite = GetLogoFromCurrency(currency);
        if (costAreaType != CostAreaType.PayAndCallEvent) costAreaEvent = CostAreaEvent.None;
        GameObject rewardPrefab = GetRewardModelFromRewardType();
        if (rewardPrefab != null)
        {
            GameObject rewardFeedback = Instantiate(rewardPrefab, rewardPreview);
            rewardFeedback.transform.localPosition = Vector3.zero;
            // TODO: UGLY, tweak the feedback a bit instead (may be hard for position as the pivot point for hammer is not centered)
            if (rewardType == CollectableType.Key)
            {
                rewardFeedback.transform.localPosition += Vector3.up * 2.0f;
                rewardPreview.localScale = Vector3.one * 0.5f;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerController playerComponent = other.GetComponent<PlayerController>();
        if (!isActive)
            return;

        if (playerComponent != null)
        {
            if (playerComponent.PrevState.Buttons.B == ButtonState.Pressed && playerComponent.State.Buttons.B == ButtonState.Released)
            {
                if (Pay(playerComponent))
                {
                    isActive = false;

                    GiveReward(playerComponent);
                    // deactivate halo
                    // deactivate price and visual?
                }
                else
                {
                    // Feedback visuel/sonore
                }
            }
        }
    }

    bool Pay(PlayerController _player)
    {
        int money = _player.Player.Collectables[(int)currency];
        if (money >= cost)
        {
            _player.Player.UpdateCollectableValue(Currency, -Cost);
            return true;
        }
        return false;
    }

    void GiveReward(PlayerController _player)
    {
        if (costAreaType == CostAreaType.PayAndGetItem)
        {
            _player.Player.UpdateCollectableValue(rewardType, rewardQuantity);
        }
        else if (costAreaType == CostAreaType.PayAndCallEvent)
        {
            switch (costAreaEvent)
            {
                case CostAreaEvent.EndGame:
                    // TODO: UGLY, EndingTrigger should be renamed and be static
                    GameObject.FindObjectOfType<EndingTrigger>().HasFinishedProcess(_player.Player);
                    break;
                case CostAreaEvent.IncreaseWater:
                    // TODO: UGLY, TheButton should be referenced somewhere I suppose or the event externalized
                    GameObject.FindObjectOfType<TheButton>().StartIncreasing();
                    break;
                default:
                    break;
            }
        }
    }

    GameObject GetRewardModelFromRewardType()
    {
        switch(costAreaType)
        {
            case CostAreaType.PayAndGetItem:
                switch(rewardType)
                {
                    case CollectableType.Key:
                        return ResourceUtils.Instance.refPrefabIle.prefabCostAreaKeyFeedback;
                    case CollectableType.PlatformistEvolution1:
                        return ResourceUtils.Instance.refPrefabIle.prefabCostAreaPlatformistFeedback;
                    case CollectableType.AgileEvolution1:
                        return ResourceUtils.Instance.refPrefabIle.prefabCostAreaAgilityFeedback;
                    case CollectableType.GhostEvolution1:
                        return ResourceUtils.Instance.refPrefabIle.prefabCostAreaGhostFeedback;
                    case CollectableType.StrengthEvolution1:
                        return ResourceUtils.Instance.refPrefabIle.prefabCostAreaStrengthFeedback;
                }
                break;
            case CostAreaType.PayAndCallEvent:
                switch(costAreaEvent)
                {
                    case CostAreaEvent.EndGame:
                        return ResourceUtils.Instance.refPrefabIle.prefabCostAreaTrophyFeedback;
                    case CostAreaEvent.IncreaseWater:
                        return ResourceUtils.Instance.refPrefabIle.prefabCostAreaWaterFeedback;
                }
                break;
            default:
                return null;
        }
        return null;

   }

    Sprite GetLogoFromCurrency(CollectableType _currency)
    {
        switch(_currency)
        {
            case CollectableType.Key:
                return currencyRune;

            case CollectableType.Points:
                return currencyCoin;
        }

        return null;
    }
}
