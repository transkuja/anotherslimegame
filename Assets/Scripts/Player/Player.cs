using UnityEngine;

public enum PlayerChildren { SlimeMesh, ShadowProjector, BubbleParticles, SplashParticles, WaterTrailParticles, CameraTarget, DustTrailParticles, DashParticles, LandingParticles };

public enum PlayerUIStat { Life, Points, Size}

public delegate void UIfct(int _newValue);

public class Player : MonoBehaviour {

    Rigidbody rb;
    bool canDoubleJump = false;

    [Header("Collectables")]
    [SerializeField] int[] collectables;

    public uint activeEvolutions = 0;

    public Transform respawnPoint;
    public GameObject cameraReference;

    Animator anim;
    public bool hasBeenTeleported = false;

    public bool isEdgeAssistActive = true;
    [SerializeField]
    PlayerController playerController;

    // UI [] typeCollectable
    public UIfct[] OnValuesChange;

    // for miniGame Push
    [SerializeField] private int nbLife = -1;
    [SerializeField] private int nbPoints = 0;

    public bool[] evolutionTutoShown = new bool[(int)Powers.Size];
    public bool costAreaTutoShown = false;

    public GameObject activeTutoText;
    private GameObject pendingTutoText;
    float currentTimerPendingTutoText;
    bool tutoTextIsPending = false;

    private bool hasFinishedTheRun = false;

    public int rank = 0;

    // Ugly
    public bool isInMainTower = false;

    public Fruit associateFruit;

    public MinigamePickUp currentStoredPickup;

#region Accessors
    public Rigidbody Rb
    {
        get
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            return rb;
        }

        set
        {
            rb = value;
        }
    }

    public Animator Anim
    {
        get
        {
            if (anim == null)
                anim = GetComponentInChildren<Animator>();
            return anim;
        }

        set
        {
            anim = value;
        }
    }

    public bool CanDoubleJump
    {
        get
        {
            return canDoubleJump;
        }

        set
        {
            canDoubleJump = value;
        }
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
                PlayerController.enabled = false;

                // Making the player to stop in the air 
                Rb.Sleep(); // Quelque part là, il y a un sleep
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

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        collectables = new int[(int)CollectableType.Size];
    }

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
                NbPoints += pickedValue;  
                break;
            case CollectableType.Fruits:
                NbPoints += pickedValue;
                break;
            case CollectableType.Bonus:
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

        bool canEvolve = ( activeEvolutions == 0);

        if (!_launchProcessOnSucess)
            return canEvolve;

        if (canEvolve)
            PermanentEvolution(_evolution);

        return canEvolve;       
    }

    private void PermanentEvolution(Evolution evolution)
    {
        GameManager.EvolutionManager.AddEvolutionComponent(gameObject, evolution);
    }


    private void Start()
    {

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "TriggerMainTower")
            isInMainTower = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "TriggerMainTower")
            isInMainTower = false;
    }
}
