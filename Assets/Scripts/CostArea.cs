using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UWPAndXInput;

public enum CostAreaType { PayAndGetItem, PayAndCallEvent, PayAndUnlockMiniGame, DontPayAndUnlockMinigame }
public enum CostAreaEvent { None, EndGame, IncreaseWater }
public enum CostAreaReactivationMode { None, OverTime, OnEvent }
public class CostArea : MonoBehaviour {
    [SerializeField]
    public CostAreaType costAreaType;

    [SerializeField]
    CostAreaEvent costAreaEvent;

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
    [SerializeField]
    [Tooltip("Only on the right prefab.")]
    InitTeleporter teleporterToMiniGame;

    [Header("Reward")]
    [SerializeField]
    public CollectableType rewardType;
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
        // Desactive la cost area si le minijeu a deja été debloqué
        if (costAreaType == CostAreaType.PayAndUnlockMiniGame || costAreaType == CostAreaType.DontPayAndUnlockMinigame )
        {
            if (GetComponent<CreateEnumFromDatabase>() == null)
            {
                Debug.LogError("Start :It's a rune, it need a createEnumFromDatabase component link to the associated rune");
                return;
            }

            string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
            if (DatabaseManager.Db.IsUnlock<DatabaseClass.MinigameData>(s))
            {
                // is active -> false
                UnlockAssociatedMinigame(s);
            }
        }

        if (costAreaType == CostAreaType.PayAndGetItem && rewardType == CollectableType.Color)
        {
            if (GetComponent<CreateEnumFromDatabase>() == null)
            {
                Debug.LogError("Start :It's a color, it need a createEnumFromDatabase component link to the associated color");
                return;
            }

            string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
            if (DatabaseManager.Db.IsUnlock<DatabaseClass.ColorData>(s))
            {
                Desactivate();
                return;
            }
        }

        initialColor = halo.GetComponent<ParticleSystem>().main.startColor.color; // <======8 WTFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF???!
        // Remi : Pour que l'evolution recuperer saffiche
        //if (!Utils.IsAnEvolutionCollectable(rewardType))
            Init();
  
    }

    void Init()
    {
        costText.text = "x " + cost;
        currencyLogo.sprite = GetLogoFromCurrency(currency);
        if (currency == CollectableType.Rune)
        {
            // Ugly ugly ugly wooooooohohooooo uuuuuglyyyy tudududidadidu
            currencyLogo.color = new Color(0, 146, 255, 255);
        }
        if (costAreaType != CostAreaType.PayAndCallEvent) costAreaEvent = CostAreaEvent.None;
        GameObject rewardPrefab = GetRewardModelFromRewardType();
        if (rewardPrefab != null)
        {
            GameObject rewardFeedback = Instantiate(rewardPrefab, rewardPreview);
            rewardFeedback.transform.localPosition = Vector3.zero;
            // TODO: UGLY, tweak the feedback a bit instead (may be hard for position as the pivot point for hammer is not centered)
            if (rewardType == CollectableType.Rune)
            {
                rewardFeedback.transform.localPosition += Vector3.up * 2.0f;
                rewardPreview.localScale = Vector3.one * 0.5f;
            }
        }
        else
        {
            costText.transform.parent.localPosition = costText.transform.parent.localPosition - Vector3.up * 3.0f;
        }
    }

    public void InitForEvolution(CollectableType ct)
    {
        rewardType = ct;
        Init();
    }

    public void Desactivate()
    {
        isActive = false;

        costText.text = "x 0";
        costText.color = Color.grey;
        currencyLogo.color = Color.grey;
        rewardPreview.gameObject.SetActive(false);
        ParticleSystem.MainModule main = halo.GetComponent<ParticleSystem>().main;
        main.startColor = Color.white;
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

        // Force unlock minigame : checking at each frame if a minigame have been unlocked
        if (isActive && (costAreaType == CostAreaType.PayAndUnlockMiniGame || costAreaType == CostAreaType.DontPayAndUnlockMinigame)
             && GameManager.Instance.IsInHub() && GetComponent<CreateEnumFromDatabase>() != null)
        {
            string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
            if (DatabaseManager.Db.minigames.Find(a => a.Id == s).nbRunesToUnlock == -1 && DatabaseManager.Db.minigames.Find(a => a.Id == s).costToUnlock == -1)
            {
                // to avoid checking at each frame if a -1 minigame have been unlocked
                isActive = false;
            }

            else if (DatabaseManager.Db.minigames.Find(a => a.Id == s).nbRunesToUnlock <= GameManager.Instance.Runes)
            {
                // Force unlock if not already ( case at runtime )
                DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(s, true);

                // Will avoid checking afterwards : isactive -> false
                UnlockAssociatedMinigame(s);
                // Notifiier le joueur ici ? 
            }


        }
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerControllerHub playerComponent = other.GetComponent<PlayerControllerHub>();
        if (!isActive || costAreaType == CostAreaType.DontPayAndUnlockMinigame)
            return;

        if (playerComponent != null)
        {
            // TODO: rename bool array
            
            if (!playerComponent.Player.costAreaTutoShown && !GameManager.Instance.CurrentGameMode.IsMiniGame())
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
                    Desactivate();

                    currentTimerBeforeReactivation = timeBeforeReactivation;
                    GiveReward(playerComponent);
                }
                else
                {
                    // Feedback visuel/sonore
                    GameManager.UiReference.HandleFeedbackCantPay(currency);
                    if (AudioManager.Instance != null && AudioManager.Instance.cantPayFx != null)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.cantPayFx);
                }
            }
        }
    }

    bool Pay(PlayerControllerHub _player)
    {
        int money = 0;
        if(currency == CollectableType.Money)
            money = GameManager.Instance.GlobalMoney;
        else if (currency == CollectableType.Rune)
            money = GameManager.Instance.Runes;

        if (money >= cost)
        {
            _player.Player.UpdateCollectableValue(Currency, -Cost);
            return true;
        }
        return false;
    }

    void GiveReward(PlayerControllerHub _player)
    {
        if (costAreaType == CostAreaType.PayAndGetItem)
        {
            // Color test tmp
            // @Remi debloque la color
            if (rewardType == CollectableType.Color)
            {
                if (GetComponent<CreateEnumFromDatabase>() == null)
                {
                    Debug.LogError("Attract fct : It's a rune, it need a createEnumFromDatabase component link to the associated rune");
                    return;
                }
                string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
                DatabaseManager.Db.SetUnlock<DatabaseClass.ColorData>(s, true);
                return;
            }
            // Fin tmp
         
            _player.Player.UpdateCollectableValue(rewardType, rewardQuantity);

            // @Remi debloque la rune
            if (rewardType == CollectableType.Rune)
            {
                if (GetComponent<CreateEnumFromDatabase>() == null)
                {
                    Debug.LogError("Attract fct : It's a rune, it need a createEnumFromDatabase component link to the associated rune");
                    return;
                }
                string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
                DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(s, true);
            } 
        }
        else if (costAreaType == CostAreaType.PayAndCallEvent)
        {
            switch (costAreaEvent)
            {
                case CostAreaEvent.EndGame:
                    GameManager.Instance.CurrentGameMode.PlayerHasFinished(_player.Player);
                    break;
                case CostAreaEvent.IncreaseWater:
                    HUBManager.instance.StartIncreasing();
                    break;
                default:
                    break;
            }
        }
        else
        {
            // debloque le minijeu
            if (GetComponent<CreateEnumFromDatabase>() == null)
            {
                Debug.LogError("createEnumFromDatabase component link to the associated rune");
                return;
            }
            string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
            DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(s, true);
            UnlockAssociatedMinigame(s);
        }
    }

    public void UnlockAssociatedMinigame(string minigameIdFromDatabase)
    {
        if (costAreaType == CostAreaType.PayAndUnlockMiniGame || costAreaType == CostAreaType.DontPayAndUnlockMinigame)
        {
            isActive = false;
            teleporterToMiniGame.TeleportToMinigame(minigameIdFromDatabase);
            // Replace by the child
            costText.transform.parent.gameObject.SetActive(false);
            //rewardPreview.gameObject.SetActive(false);
            rewardPreview.transform.localPosition += new Vector3(0.0f, 1.0f, 0.0f);
            halo.gameObject.SetActive(false);
            teleporterToMiniGame.gameObject.SetActive(true);
        }
    }

    GameObject GetRewardModelFromRewardType()
    {
        switch (costAreaType)
        {
            case CostAreaType.PayAndGetItem:
                switch (rewardType)
                {
                    case CollectableType.Rune:
                        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaKeyFeedback;
                    case CollectableType.PlatformistEvolution1:
                        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaPlatformistFeedback;
                    case CollectableType.AgileEvolution1:
                        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaAgilityFeedback;
                    case CollectableType.GhostEvolution1:
                        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaGhostFeedback;
                    case CollectableType.StrengthEvolution1:
                        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaStrengthFeedback;
                }
                break;
            case CostAreaType.PayAndCallEvent:
                // UGLY condition, prefabs should not be linked to hubmanager but we have to for the milestone
                switch (costAreaEvent)
                {
                    case CostAreaEvent.EndGame:
                        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaTrophyFeedback;
                    case CostAreaEvent.IncreaseWater:
                        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaWaterFeedback;
                }
                break;
            case CostAreaType.DontPayAndUnlockMinigame:
                switch (GetComponent<CreateEnumFromDatabase>().HideInt)
                {
                    case 0:
                        return ResourceUtils.Instance.feedbacksManager.prefabJumpFeedback;
                    case 1:
                        return ResourceUtils.Instance.feedbacksManager.prefabPushFeedback;
                    case 2:
                        return ResourceUtils.Instance.feedbacksManager.prefabKartFeedback;
                    case 3:
                        return ResourceUtils.Instance.feedbacksManager.prefabRunnerFeedback;
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
            case CollectableType.Rune:
                return currencyRune;

            case CollectableType.Money:
                return currencyCoin;
        }

        return null;
    }

    ////////////////////////////////////// EVENTS //////////////////////////////////////////
 
}
