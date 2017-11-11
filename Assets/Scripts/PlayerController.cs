using XInputDotNetPure;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PlayerController : MonoBehaviour {
    
        // Component : 
    private PlayerState playerState;
    private JumpManager jumpManager;
    Player player;

    // gamePad
    GamePadState state;
    GamePadState prevState;
    bool isUsingAController = false;

    // evolution : 
    int selectedEvolution = 0;

    //  others
    public PlayerIndex playerIndex;
    bool playerIndexSet = false;
    public bool isGravityEnabled = true;
    float maxDistanceOffset = 2.0f;

    // jump
    float chargeFactor = 0.0f;
    [Range(5, 1000)] float jumpChargeSpeed = 15.0f;

    [SerializeField]public Stats stats;
    [SerializeField]bool isGrounded = true;
    public bool DEBUG_hasBeenSpawnedFromTool = false;

    public bool canDoubleJump = true; // A Priori c'es du legacy, mais j'ai pas toutpigé.

#if UNITY_EDITOR
    [SerializeField]public string curStateName; // debug purpose only
#endif
    #region GetterSetters

    /// StateManagment
    public PlayerState PlayerState
    {
        get
        {
            return playerState;
        }
        set
        {
            if (PlayerState!=null)
            {
                PlayerState.OnEnd();
            }
            playerState = value;
#if UNITY_EDITOR
            curStateName = value.ToString();
#endif
            PlayerState.OnBegin();
        }
    }
    public bool IsGrounded
    {
        get
        {
            return isGrounded;
        }

        private set
        {
            if (value == true && GetComponent<JumpManager>() != null)
                GetComponent<JumpManager>().Stop();
            isGrounded = value;
        }
    }

    public PlayerIndex PlayerIndex
    {
        get
        {
            return playerIndex;
        }

        set
        {
            playerIndex = value;
        }
    }

    public bool IsUsingAController
    {
        get
        {
            return isUsingAController;
        }

        set
        {
            isUsingAController = value;
        }
    }

    public bool PlayerIndexSet
    {
        get
        {
            return playerIndexSet;
        }
        set
        {
            playerIndexSet = value;
        }
    }

    public GamePadState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    public GamePadState PrevState
    {
        get
        {
            return prevState;
        }

        set
        {
            prevState = value;
        }
    }

    public int SelectedEvolution
    {
        get
        {
            return selectedEvolution;
        }

        set
        {
            selectedEvolution = value;
        }
    }

    public Player Player
    {
        get
        {
            return player;
        }

        set
        {
            player = value;
        }
    }

    public JumpManager JumpManager
    {
        get
        {
            return jumpManager;
        }

        set
        {
            jumpManager = value;
        }
    }

    #endregion

    private void Awake()
    {
        stats.Init(this);
    }

    void Start () {
        Player = GetComponent<Player>();
        if (Player == null)
            Debug.Log("Player should not be null");
        PlayerState = new FreeState(this);
	}
	
	// Update is called once per frame
	void Update () {

        if(PlayerState != null)
            PlayerState.OnUpdate();
    }
    private void FixedUpdate()
    {
        PlayerState.HandleGravity();

        if (DEBUG_hasBeenSpawnedFromTool)
            return;
        if (!playerIndexSet)
            return;
        if (!prevState.IsConnected)
        {
            isUsingAController = false;
            for (int i = 0; i < GameManager.Instance.ActivePlayersAtStart; i++)
            {
                GamePadState testState = GamePad.GetState(playerIndex);

                if (testState.IsConnected)
                {
                    playerIndexSet = true;
                    isUsingAController = true;
                    break;
                }
            }
        }
        if (isUsingAController)
        {
            // TODO: optimize?
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (GameManager.CurrentState == GameState.Normal)
            {
                HandleJumpWithController();
                HandleMovementWithController();
                if (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabCollectableAndActivate)
                    HandleEvolutionsWithController();
            }
            // TODO: Externalize "state" to handle pause in PauseMenu? //  Remi : Can't manage GamePade(IndexPlayer) Instead, copy not working
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
                if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
                    GameManager.ChangeState(GameState.Paused);
        }
        else
        {
            // keyboardStuff
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.ChangeState(GameState.Paused);
        }
            // handle stateFunction
     if (PlayerState != null)
            PlayerState.OnFixedUpdate();

        // Handle Grounded
        if (player.Rb.velocity.y <= 0.2f && !isGrounded)
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position + Vector3.up, 1f, -transform.up, out hitInfo, maxDistanceOffset))
            {
                if (hitInfo.transform.gameObject.GetComponentInParent<Ground>() != null)
                {
                    IsGrounded = true;
                }
            }
        }
    }

            // Pour continuer à alleger, ajouter un composant qui s'occupe des evolution ?
    public void HandleEvolutionsWithController()
    {
        if (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed)
        {
            selectedEvolution = selectedEvolution > 0 ? (selectedEvolution - 1) % (int)Powers.Size : 0;
            GameManager.UiReference.NeedUpdate(selectedEvolution.ToString());
        }
        if (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed)
        {
            selectedEvolution = (selectedEvolution + 1) % (int)Powers.Size;
            GameManager.UiReference.NeedUpdate(selectedEvolution.ToString());
        }
        if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
        {
            Evolution selectedEvol = GameManager.EvolutionManager.GetEvolutionByPowerName((Powers)Enum.Parse(typeof(Powers), selectedEvolution.ToString()));
            if (player.Collectables[0] >= selectedEvol.Cost)
            {
                player.EvolveGameplay2(selectedEvol);
            }
            // if has enough => evolve else nothing
        }
       
    }
    public void HandleMovementWithController()
    {
        PlayerState.HandleMovementWithController();
    }
    private void HandleJumpWithController()
    {
        // Charge jump if A button is pressed for a "long" time and only if on the ground
        //if (isGrounded)
        {
            if (state.Buttons.A == ButtonState.Pressed)
            {
                chargeFactor += jumpChargeSpeed * Time.unscaledDeltaTime;
                // Force max charge jump if the charge reach maximum charge
                if (chargeFactor > 1.0f)
                {
                    playerState.Jump();
                }
            }
            else if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released)
            {
                playerState.Jump();
            }
        }
    }

    public void Jump()
    {
        IsGrounded = false;
        JumpManager jm;
        if (jm = GetComponent<JumpManager>())
            jm.Jump(JumpManager.JumpEnum.Basic);
        else
            Debug.LogError("No jump manager attached to player!");
        chargeFactor = 0;
    }


}
