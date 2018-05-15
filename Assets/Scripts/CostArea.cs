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
    bool isActive = true;

    [Header("Children references")]
    [SerializeField]
    Text costText;
    [SerializeField]
    Image currencyLogo;
    [SerializeField]
    Transform rewardPreview;
    [SerializeField]
    Transform isActiveParticles;

    [Header("Currency Sprites")]
    [SerializeField]
    Sprite currencyRune;

    DatabaseClass.MinigameData mgData;

    [SerializeField]
    GameObject[] minigameName;

    public CollectableType Currency
    {
        get
        {
            return currency;
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
        else
        {
            mgData = DatabaseManager.Db.minigames.Find(a => a.type == minigameType && a.version == minigameVersion);
        }

        Init();  
    }

    void Init()
    {
        if (mgData != null)
            costText.text = "x " + mgData.nbRunesToUnlock;
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
    }

    public void Reactivate()
    {
        costText.color = Color.white;
        if (mgData != null)
            costText.text = "x " + mgData.nbRunesToUnlock;
        currencyLogo.color = Color.white;
        rewardPreview.gameObject.SetActive(true);
        isActive = true;
    }

    void Update()
    {
        // Force unlock minigame : checking at each frame if a minigame have been unlocked
        if (isActive)
        {
            if (mgData == null)
                return;

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
        teleporter.GetComponent<TeleporterToMinigame>().TeleportToMinigame(minigameIdFromDatabase, minigameVersion);

        // Replace by the child
        costText.transform.parent.gameObject.SetActive(false);
        //rewardPreview.gameObject.SetActive(false);
        if (DatabaseManager.Db.GetRuneFromMinigame(minigameType, minigameVersion).isUnlocked)
            rewardPreview.gameObject.SetActive(false);
        else
            rewardPreview.transform.localPosition = Vector3.up * rewardPreview.transform.localPosition.y;

        isActiveParticles.gameObject.SetActive(true);
        teleporter.GetComponent<TeleporterToMinigame>().isTeleporterActive = true;
    }

    GameObject GetRewardModelFromRewardType()
    {
        return ResourceUtils.Instance.feedbacksManager.prefabCostAreaKeyFeedback;
    }

    Sprite GetLogoFromCurrency(CollectableType _currency)
    {
        return currencyRune;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            minigameName[(int)other.GetComponent<PlayerController>().PlayerIndex].SetActive(true);
            foreach (TextMesh tm in minigameName[(int)other.GetComponent<PlayerController>().PlayerIndex].GetComponentsInChildren<TextMesh>())
                tm.text = MinigameDataUtils.GetTitle(mgData.Id, minigameVersion);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            minigameName[(int)other.GetComponent<PlayerController>().PlayerIndex].SetActive(false);
        }
    }
}
