using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using UWPAndXInput;

public class CostArea : MonoBehaviour {

    [SerializeField]
    MinigameType minigameType;
    [SerializeField]
    int minigameVersion;

    [SerializeField]
    GameObject teleporter;

    [SerializeField]
    CollectableType currency;

    [SerializeField]
    Color isActiveColor;

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
    [SerializeField]
    Transform halo;
    [SerializeField]
    [Tooltip("Only on the right prefab.")]
    InitTeleporter teleporterToMiniGame; // ====> TODO: move initTeleporter on teleporter

    [Header("Currency Sprites")]
    [SerializeField]
    Sprite currencyRune;

    Color initialColor;

    DatabaseClass.MinigameData mgData;

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
        mgData = DatabaseManager.Db.GetUnlockedMinigameOfType(minigameType, minigameVersion);
        if (mgData != null)
        {
            // is active -> false
            UnlockAssociatedMinigame(mgData.Id);
        }

        initialColor = halo.GetComponent<ParticleSystem>().main.startColor.color;
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
        
        GameObject rewardPrefab = GetRewardModelFromRewardType();
        if (rewardPrefab != null)
        {
            GameObject rewardFeedback = Instantiate(rewardPrefab, rewardPreview);
            rewardFeedback.transform.localPosition = Vector3.zero;
            // TODO: UGLY, tweak the feedback a bit instead (may be hard for position as the pivot point for hammer is not centered)
            rewardFeedback.transform.localPosition += Vector3.up * 2.0f;
            rewardPreview.localScale = Vector3.one * 0.5f;
        }
        else
        {
            costText.transform.parent.localPosition = costText.transform.parent.localPosition - Vector3.up * 3.0f;
        }
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
        // Force unlock minigame : checking at each frame if a minigame have been unlocked
        if (isActive)
        {
            if (mgData.nbRunesToUnlock == -1 && mgData.costToUnlock == -1)
            {
                // to avoid checking at each frame if a -1 minigame have been unlocked
                isActive = false;
            }

            else if (mgData.nbRunesToUnlock <= GameManager.Instance.Runes)
            {
                // Force unlock if not already ( case at runtime )
                DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(mgData.Id, true, mgData.version);

                // Will avoid checking afterwards : isactive -> false
                UnlockAssociatedMinigame(mgData.Id);
                // Notifiier le joueur ici ? 
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
        // debloque le minijeu
        //if (GetComponent<CreateEnumFromDatabase>() == null)
        //{
        //    Debug.LogError("createEnumFromDatabase component link to the associated rune");
        //    return;
        //}
        //string s = GetComponent<CreateEnumFromDatabase>().enumFromList[GetComponent<CreateEnumFromDatabase>().HideInt];
        //DatabaseManager.Db.SetUnlock<DatabaseClass.MinigameData>(s, true);
        //UnlockAssociatedMinigame(s);
    }

    public void UnlockAssociatedMinigame(string minigameIdFromDatabase)
    {
        isActive = false;
        teleporter.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        teleporter.GetComponent<Renderer>().material.SetColor("_EmissionColor", isActiveColor);
        teleporterToMiniGame.TeleportToMinigame(minigameIdFromDatabase);
        // Replace by the child
        costText.transform.parent.gameObject.SetActive(false);
        //rewardPreview.gameObject.SetActive(false);
        rewardPreview.transform.localPosition += new Vector3(0.0f, 1.0f, 0.0f);
        halo.gameObject.SetActive(false);
        teleporterToMiniGame.gameObject.SetActive(true);
    }

    GameObject GetRewardModelFromRewardType()
    {
        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaKeyFeedback;
   }

    Sprite GetLogoFromCurrency(CollectableType _currency)
    {
        return currencyRune;
    }

    ////////////////////////////////////// EVENTS //////////////////////////////////////////
 
}
