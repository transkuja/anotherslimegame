using UnityEngine;

public enum PlayerChildren { SlimeMesh, ShadowProjector, BubbleParticles, SplashParticles, CameraTarget, DustTrailParticles, DashParticles, LandingParticles };
public enum KeyFrom { Shelter, CostArea };

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

    [SerializeField]
    KeyReset[] keysReset;

    public bool isEdgeAssistActive = true;
    PlayerController playerController;

    public bool[] evolutionTutoShown = new bool[(int)Powers.Size];
    public bool costAreaTutoShown = false;

    public GameObject activeTutoText;
    private GameObject pendingTutoText;
    float currentTimerPendingTutoText;
    bool tutoTextIsPending = false;

    private bool hasFinishedTheRun = false;

    public int rank = 0;

    private bool feedbackCantPayActive = false;
    float timerFeedbackCantPay = 2.0f;
    float currentFeedbackCantPay = 0.0f;

    // Ugly
    public bool isInMainTower = false;

    // for miniGame Push
    [SerializeField]private int nbLife = -1;
    //

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

                // TODO: REACTIVATE INSTEAD OF INSTANTIATE (keys must not be destroyed too)
                if (!GameManager.Instance.isTimeOver)
                {
                    for (int i = 0; i < Utils.GetMaxValueForCollectable(CollectableType.Rune); i++)
                    {
                        if (KeysReset[i] == null)
                            break;

                        if (KeysReset[i].from == KeyFrom.CostArea)
                        {
                            // ConvertArrayOfBoolToString
                            KeysReset[i].initialTransform.GetComponent<CostArea>().Reactivate();
                        }
                        else if (KeysReset[i].from == KeyFrom.Shelter)
                        {
                            // TODO: reactivate
                            ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                                KeysReset[i].initialPosition,
                                KeysReset[i].initialRotation,
                                null,
                                CollectableType.Rune)
                            .GetComponent<Collectable>().Init();
                        }
                    }
                }
            }

            hasFinishedTheRun = value;
        }
    }

    public KeyReset[] KeysReset
    {
        get
        {
            return keysReset;
        }
    }

    public bool FeedbackCantPayActive
    {
        get
        {
            return feedbackCantPayActive;
        }

        set
        {
            if (value == true)
                currentFeedbackCantPay = timerFeedbackCantPay;
            feedbackCantPayActive = value;
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

    public int[] Collectables
    {
        get
        {
            return collectables;
        }

        set
        {
            collectables = value;
        }
    }
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {     
        keysReset = new KeyReset[Utils.GetMaxValueForCollectable(CollectableType.Rune)];
        collectables = new int[(int)CollectableType.Size];
    }

    public void UpdateCollectableValue(CollectableType type, int pickedValue)
    {
        if (type == CollectableType.Rune)
        {
            GameManager.Instance.Runes += 1;
            GameManager.Instance.PlayerUI.RefreshKeysPlayerUi(this, GameManager.Instance.Runes);
        }
		else if (type == CollectableType.Money)
		{
			GameManager.Instance.GlobalMoney += pickedValue;
			// TODO: update UI
		}
		// All collectables that are not money or runes should be handled like this
        else
        {
            collectables[(int)type] = Mathf.Clamp(collectables[(int)type] + pickedValue, 0, Utils.GetMaxValueForCollectable(type));
			if (type == CollectableType.Points)
			{
				GameManager.Instance.PlayerUI.RefreshPointsPlayerUi(this, collectables[(int)type], cameraReference.transform.GetSiblingIndex());
			}
        }

        if (!Utils.IsAnEvolutionCollectable(type))
            return;

        EvolutionCheck(type);
    }

    public void AddKeyInitialPosition(Transform _tr, KeyFrom _from)
    {
        int currentlyHold = collectables[(int)CollectableType.Rune];
        KeysReset[currentlyHold - 1] = new KeyReset(_tr, _from);
    }

    public void AddKeyInitialPosition(KeyReset _keyData)
    {
        int currentlyHold = collectables[(int)CollectableType.Rune];
        KeysReset[currentlyHold - 1] = _keyData;
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
        GameManager.EvolutionManager.AddEvolutionComponent(gameObject, evolution, true);
    }

    private void Update()
    {
        if (FeedbackCantPayActive)
        {
            currentFeedbackCantPay -= Time.deltaTime;
            if (currentFeedbackCantPay < 0.0f)
            {
                GameManager.Instance.PlayerUI.HandleFeedbackNotEnoughPoints(this, false);
                FeedbackCantPayActive = false;
            }
        }

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

public class KeyReset
{
    public Transform initialTransform;
    public KeyFrom from;

    public Vector3 initialPosition;
    public Quaternion initialRotation;

    public KeyReset(Transform _initialTransform, KeyFrom _from)
    {
        initialTransform = _initialTransform;
        if (_initialTransform != null)
        {
            initialPosition = _initialTransform.position;
            initialRotation = _initialTransform.rotation;
        }
        from = _from;
    }
}