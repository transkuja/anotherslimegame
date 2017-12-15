using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UWPAndXInput;

public enum CostAreaType { PayAndGetItem, PayAndCallEvent, PayAndUnlockMiniGame }
public enum CostAreaEvent { None, EndGame, IncreaseWater }
public enum CostAreaReactivationMode { None, OverTime, OnEvent }
public class CostArea : MonoBehaviour {
    [SerializeField]
    CostAreaType costAreaType;

    [SerializeField]
    CostAreaEvent costAreaEvent;

    [SerializeField]
    MiniGame minigameToUnlock;

    [SerializeField]
    CollectableType currency;
    
    [SerializeField]
    int cost;

    [SerializeField]
    bool isActive = true;

    [Header("Reactivation settings")]
    [SerializeField]
    CostAreaReactivationMode reactivationMode;
    [SerializeField]
    float timeBeforeReactivation;
    float currentTimerBeforeReactivation;

    [Header("Children references")]
    [SerializeField]
    Text costText;
    [SerializeField]
    Image currencyLogo;
    [SerializeField]
    Transform rewardPreview;
    [SerializeField]
    Transform halo;

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

    Color initialColor;

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
        initialColor = halo.GetComponent<ParticleSystem>().main.startColor.color; // <======8 WTFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF???!
        if (!Utils.IsAnEvolutionCollectable(rewardType))
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

    public void InitForEvolution(CollectableType ct)
    {
        rewardType = ct;
        Init();
    }

    public void Reactivate()
    {
        costText.color = Color.white;
        costText.text = "x " + cost;
        currencyLogo.color = Color.white;
        rewardPreview.gameObject.SetActive(true);
        ParticleSystem.MainModule main = halo.GetComponent<ParticleSystem>().main;
        main.startColor = initialColor;

        isActive = true;
    }

    void Update()
    {
        if (reactivationMode == CostAreaReactivationMode.OverTime && !isActive)
        {
            currentTimerBeforeReactivation -= Time.deltaTime;
            if (currentTimerBeforeReactivation < 0.0f)
            {
                Reactivate();
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
            // TODO: rename bool array
            if (!playerComponent.Player.costAreaTutoShown)
            {
                playerComponent.Player.costAreaTutoShown = true;
                Utils.PopTutoText("Press B to buy the reward", playerComponent.Player);
            }
            if (playerComponent.PrevState.Buttons.B == ButtonState.Pressed && playerComponent.State.Buttons.B == ButtonState.Released)
            {
                if (Utils.IsAnEvolutionCollectable(rewardType))
                {
                    if (playerComponent.Player.EvolutionCheck(rewardType, false) == false)
                        return;
                }

                if (Pay(playerComponent))
                {
                    isActive = false;

                    costText.text = "x 0";
                    costText.color = Color.grey;
                    currencyLogo.color = Color.grey;
                    rewardPreview.gameObject.SetActive(false);
                    ParticleSystem.MainModule main = halo.GetComponent<ParticleSystem>().main;
                    main.startColor = Color.white;

                    currentTimerBeforeReactivation = timeBeforeReactivation;
                    GiveReward(playerComponent);
                }
                else
                {
                    // Feedback visuel/sonore
                    GameManager.Instance.PlayerUI.HandleFeedbackNotEnoughPoints(playerComponent.Player, true);
                    if (AudioManager.Instance != null && AudioManager.Instance.cantPayFx != null)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.cantPayFx);
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
            if (rewardType == CollectableType.Key)
            {
                _player.Player.AddKeyInitialPosition(transform, KeyFrom.CostArea);
            }
        }
        else if (costAreaType == CostAreaType.PayAndCallEvent)
        {
            switch (costAreaEvent)
            {
                case CostAreaEvent.EndGame:
                    HasFinishedProcess(_player.Player);
                    break;
                case CostAreaEvent.IncreaseWater:
                    // TODO: UGLY, TheButton should be referenced somewhere I suppose or the event externalized
                    GameObject.FindObjectOfType<TheButton>().StartIncreasing();
                    break;
                default:
                    break;
            }
        }
        else if (costAreaType == CostAreaType.PayAndUnlockMiniGame)
        {
            MinigameManager.GetSceneNameFromMinigame(minigameToUnlock);
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

    ////////////////////////////////////// EVENTS //////////////////////////////////////////
    public void HasFinishedProcess(Player _player)
    {
        _player.HasFinishedTheRun = true;
        GameManager.Instance.ScoreScreenReference.RefreshScores(_player);
    }
}
