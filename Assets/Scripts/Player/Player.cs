﻿using UnityEngine;

public enum PlayerChildren { SlimeMesh, ShadowProjector, BubbleParticles, SplashParticles, WaterTrailParticles, CameraTarget, DustTrailParticles, DashParticles, LandingParticles };

public enum PlayerUIStat { Life, Points, Size}

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

    public bool[] evolutionTutoShown = new bool[(int)Powers.Size];
    public bool costAreaTutoShown = false;

    public GameObject activeTutoText;
    private GameObject pendingTutoText;
    float currentTimerPendingTutoText;
    bool tutoTextIsPending = false;

    private bool hasFinishedTheRun = false;

    public int rank = 0;

    public Fruit associateFruit;

    public MinigamePickUp.Use currentStoredPickup;

    bool activateAerialDrag = false;

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
                PlayerController.enabled = false;

                // Making the player to stop in the air 
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
        if (PlayerController.GetType() == typeof(PlayerControllerHub))
        {
            activateAerialDrag = (!((PlayerCharacterHub)PlayerCharacter).IsGrounded);
            if (activateAerialDrag)
            {
                Vector3 tmp = new Vector3(PlayerCharacter.Rb.velocity.x, 0.0f, PlayerCharacter.Rb.velocity.z);
                //Vector3 fwd = playerController.transform.forward;

                float dragForceUsed = 45f * Time.deltaTime * 500f;//(playerController.PreviousPlayerState == playerController.dashState) ? dragForceDash : dragForce;

                if (tmp.magnitude > 3.0f)// && Vector3.Dot(playerController.transform.forward, tmp) > 0)
                {
                    if ((tmp.x > 0 && tmp.x - tmp.normalized.x * dragForceUsed < 0)
                    || (tmp.x < 0 && tmp.x - tmp.normalized.x * dragForceUsed > 0)
                    || (tmp.z > 0 && tmp.z - tmp.normalized.z * dragForceUsed < 0)
                    || (tmp.z < 0 && tmp.z - tmp.normalized.z * dragForceUsed > 0))
                    { }
                    //playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
                    else
                    {
                        PlayerCharacter.Rb.AddForce(-tmp.normalized * dragForceUsed);
                    }
                }
                else
                {
                    //playerController.Player.Rb.velocity = playerController.Player.Rb.velocity.y * Vector3.up;
                }
            }
        }
        else
            activateAerialDrag = false;
    }
}
