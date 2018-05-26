using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerUIStat { Life, Points, Laps, Size}

public delegate void UIfct(int _newValue);

public class Player : MonoBehaviour {
    public uint activeEvolutions = 0;

    public Transform respawnPoint;
    public GameObject cameraReference;

    private bool hasBeenTeleported = false;

    public bool isEdgeAssistActive = true;

    private PlayerController playerController;
    private PlayerCharacter playerCharacter;

    // UI [] typeCollectable
    public UIfct[] OnValuesChange;

    // for miniGame Push
    [SerializeField] private int nbLife = -1;
    [SerializeField] private int nbPoints = 0;

    //for minigame Kart
    [SerializeField] private float finishTime = -1.0f;
    private float timerInvincibilite = 0.07f;
    private float currentTimerInvincibilite = 0;

    public bool[] evolutionTutoShown = new bool[(int)Powers.Size];
    public bool costAreaTutoShown = false;

    PNJMessage refMessage;
    TeleporterToMinigame refInitTeleporter;

    public GameObject activeTutoText;
    private GameObject pendingTutoText;
    float currentTimerPendingTutoText;
    bool tutoTextIsPending = false;

    private bool hasFinishedTheRun = false;

    public int rank = 0;

    public MinigamePickUp.Use currentStoredPickup;
    public float airControlFactor = 0.42f;

    #region Accessors

    public int ID
    {
        get { return (PlayerController != null) ? (int)PlayerController.PlayerIndex : 0; }
    }

    public PlayerController PlayerController
    {
        get
        {
            if (playerController == null)
                playerController = GetComponent<PlayerController>();
            return playerController;
        }

    }

    public bool HasFinishedTheRun
    {
        get
        {
            return hasFinishedTheRun;
        }

        set
        {
            if (value == true)
            {
                if (playerController != null)
                    PlayerController.enabled = false;

                // Making the player to stop in the air 
                if(PlayerCharacter)
                    PlayerCharacter.Rb.Sleep(); // Quelque part là, il y a un sleep
            }

            hasFinishedTheRun = value;
        }
    }

    public GameObject PendingTutoText
    {
        get
        {
            return pendingTutoText;
        }

        set
        {
            if (value != null)
            {
                tutoTextIsPending = true;
                currentTimerPendingTutoText = Utils.timerTutoText + 0.1f;
            }
            pendingTutoText = value;
        }
    }

    public int NbLife
    {
        get
        {
            return nbLife;
        }

        set
        {
            nbLife = value;
            CallOnValueChange(PlayerUIStat.Life, nbLife);
        }
    }

    public int NbPoints
    {
        get
        {
            return nbPoints;
        }

        set
        {
            nbPoints = value;
            CallOnValueChange(PlayerUIStat.Points, nbPoints);
        }
    }

    public float FinishTime
    {
        get { return finishTime; }
        set { finishTime = value; }
    }

    public bool HasBeenTeleported
    {
        get
        {
            return hasBeenTeleported;
        }

        set
        {
            hasBeenTeleported = value;
#if UNITY_EDITOR
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.HasBeenTp, value.ToString(), ID);
#endif
        }
    }

    public PlayerCharacter PlayerCharacter
    {
        get
        {
            if (playerCharacter == null)
                playerCharacter = GetComponent<PlayerCharacter>();
            return playerCharacter;
        }

        set
        {
            playerCharacter = value;
        }
    }

    public TeleporterToMinigame RefInitTeleporter
    {
        get
        {
            return refInitTeleporter;
        }

        set
        {
            refInitTeleporter = value;
        }
    }

    public PNJMessage RefMessage {
        get
        {
            return refMessage;
        }

        set
        {
            refMessage = value;
        }
    }

    #endregion

    public void UpdateCollectableValue(CollectableType type, int pickedValue = 1)
    {
        switch (type)
        {
            case CollectableType.Rune:
                GameManager.Instance.Runes += pickedValue;
                break;
            case CollectableType.Money:
                GameManager.Instance.GlobalMoney += pickedValue;
                break;
            case CollectableType.Points:
                if (!(GameManager.Instance.CurrentGameMode is BreakingGameMode))
                {
                    GameObject feedback = Instantiate(ResourceUtils.Instance.feedbacksManager.scorePointsPrefab, null);
                    if (pickedValue >= 0)
                    {
                        feedback.GetComponentInChildren<Outline>().effectColor = Color.green;
                        feedback.GetComponentInChildren<Text>().text = "+ ";
                    }
                    else
                    {
                        feedback.GetComponentInChildren<Outline>().effectColor = Color.red;
                        feedback.GetComponentInChildren<Text>().text = "- ";
                    }
                    if (!(GameManager.Instance.CurrentGameMode is FoodGameMode))
                        feedback.transform.GetChild(0).position = Camera.main.WorldToScreenPoint(transform.position);
                    else
                    {
                        if (pickedValue < 0)
                        {
                            feedback.transform.GetChild(0).position = ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputTracksHandler.transform.GetChild((int)playerController.playerIndex).position;
                            feedback.transform.GetChild(0).position += (Vector3.right * Random.Range(-50, 50) + Vector3.up * Random.Range(-50, 50));
                            feedback.transform.GetChild(0).localScale = Vector3.one * 1.5f;
                        }
                        else
                        {
                            feedback.transform.GetChild(0).position = Camera.main.WorldToScreenPoint(transform.position);
                            feedback.transform.GetChild(0).position += (Vector3.right * Random.Range(-50, 50) + Vector3.up * Random.Range(-50, 50));
                        }
                    }
                    feedback.GetComponentInChildren<Text>().text += Utils.Abs(pickedValue).ToString();
                    feedback.GetComponentInChildren<Text>().enabled = true;
                }

                NbPoints += pickedValue;  
                break;
            case CollectableType.Fruits:
                NbPoints += pickedValue;
                if (NbPoints < 0)
                {
                    NbPoints = 0;
                }
                break;
            case CollectableType.Bonus:
                NbPoints += pickedValue;
                if (NbPoints < 0)
                {
                    NbPoints = 0;
                }
                break;
            default:
                EvolutionCheck(type);
                break;
        }     
    }

    public void CallOnValueChange(PlayerUIStat type, int _newValue)
    {
        if (OnValuesChange != null)
        {
            if (OnValuesChange.Length > 0)
            {
                if (OnValuesChange[(int)type] != null)
                {
                    OnValuesChange[(int)type](_newValue);
                }
            }
        }
    }

    public bool EvolutionCheck(CollectableType type, bool _launchProcessOnSucess = true)
    {
        Evolution _evolution = GameManager.EvolutionManager.GetEvolutionByCollectableType(type);

        bool canEvolve = ( activeEvolutions <= 1);

        if (!_launchProcessOnSucess)
            return canEvolve;

        if (canEvolve)
            PermanentEvolution(_evolution);

        return canEvolve;       
    }

    private void PermanentEvolution(Evolution evolution)
    {

        // Remove old evolution !!! ghost handles it differently, thx seb
        if(activeEvolutions >= 1)
        {
            if (GetComponent<EvolutionStrength>()) Destroy(GetComponent<EvolutionStrength>());
            if (GetComponent<EvolutionAgile>()) Destroy(GetComponent<EvolutionAgile>());
            if (GetComponent<EvolutionPlatformist>()) Destroy(GetComponent<EvolutionPlatformist>());
            if (GetComponent<EvolutionGhost>()) Destroy(GetComponent<EvolutionGhost>());
        }

        GameManager.EvolutionManager.AddEvolutionComponent(gameObject, evolution);
    }


    private void Update()
    {
        if (tutoTextIsPending)
        {
            currentTimerPendingTutoText -= Time.deltaTime;
            if (currentTimerPendingTutoText < 0.0f)
            {
                // TODO: lot of behaviours here duplicated in Utils => Merge
                if (activeTutoText != null)
                    activeTutoText.SetActive(false);

                activeTutoText = pendingTutoText;
                pendingTutoText.transform.position = cameraReference.GetComponentInChildren<Camera>().WorldToScreenPoint(transform.position)
                                        + Vector3.up * ((GameManager.Instance.PlayerStart.PlayersReference.Count > 2) ? 80.0f : 160.0f);

                pendingTutoText.SetActive(true);
                GameObject.Destroy(pendingTutoText, Utils.timerTutoText);

                tutoTextIsPending = false;
            }
        }
    }

    // Delegate events in RUNNER:
    public delegate void OnPlayerDeath(Player player);
    public OnPlayerDeath OnDeathEvent;

    public void OnDeath()
    {
        //Respawner.RespawnProcess(GetComponent<Player>());
        if (OnDeathEvent != null)
            OnDeathEvent(this);
    }

    public void Clignote()
    {
        currentTimerInvincibilite += Time.deltaTime;
        if(currentTimerInvincibilite> timerInvincibilite)
        {
            playerCharacter.Body.GetComponentInChildren<Renderer>().enabled = !playerCharacter.Body.GetComponentInChildren<Renderer>().enabled;
            currentTimerInvincibilite = 0.0f;
        }
    
    }

    public void ArreteDeClignoter()
    {
        playerCharacter.Body.GetComponentInChildren<Renderer>().enabled = true;
    }
}
