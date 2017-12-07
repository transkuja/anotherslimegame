using UWPAndXInput;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Cinemachine;


// Gère les input selon l'input appelle des action codée dans une playerState.

public class PlayerController : MonoBehaviour
{
    // Component : 
    private PlayerState playerState;
    private JumpManager jumpManager;
    private Rigidbody rb;
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
    public float chargeFactor = 0.0f;
    [Range(5, 1000)] float jumpChargeSpeed = 15.0f;

    // plateformisttmp
    bool rightTriggerHasBeenPressed = false;
    float timerRightTriggerPressed = 0.0f;

    // Deformer
    private MeshDeformer deformer;
    //private DeformerComputeShader deformer;

    // All PlayerStateCreation once and for all.
    public JumpState jumpState;
    public WalljumpState wallJumpState;
    public DashState dashState;
    public FreeState freeState;
    public DashDownState downDashState;
    public ExpulsedState expulsedState;
    public PlatformistChargedState platformistChargedState;
    public RestrainedByGhostState restrainedByGhostState;
    public FrozenState frozenState;

    [SerializeField] public Stats stats;
    [SerializeField] bool isGrounded = true;
    public bool DEBUG_hasBeenSpawnedFromTool = false;

    public bool canDoubleJump = true; // A Priori c'es du legacy, mais j'ai pas toutpigé.

#if UNITY_EDITOR
    [SerializeField] public string curStateName; // debug purpose only
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
            if (value == null)
                Debug.Log("State not created");
            else if (!value.stateAvailable)
                return;
            if (PlayerState != null)
            {
                PlayerState.OnEnd();
            }
            playerState = value;
            PlayerState.OnBegin();
#if UNITY_EDITOR
            curStateName = value.ToString();
#endif
        }
    }
    public bool IsGrounded
    {
        get
        {
            return isGrounded;
        }

        set
        {
            if (value == true)
            {
                jumpState.nbJumpMade = 0;
                downDashState.nbDashDownMade = 0;
                dashState.nbDashMade = 0;
                if (GetComponent<JumpManager>() != null)
                    GetComponent<JumpManager>().Stop();
                GetComponent<Player>().Anim.SetBool("isExpulsed", false);
            }

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
    public Rigidbody Rb
    {
        get
        {
            return rb;
        }

        set
        {
            rb = value;
        }
    }

    private void GhostController()
    {
        if (GetComponent<EvolutionGhost>())
        {
            if(IsGrounded)
                GetComponent<EvolutionGhost>().HandleTrail(state);
        }
    }

    private void PlatformistController()
    {
        // /!\ WARNING: code conflictuel si on combine les évolutions
        if (GetComponent<EvolutionPlatformist>())
        {
            EvolutionPlatformist platformistComponent = GetComponent<EvolutionPlatformist>();

            if (prevState.Triggers.Right < 0.1f && state.Triggers.Right > 0.1f)
                rightTriggerHasBeenPressed = true;

            if (rightTriggerHasBeenPressed && state.Triggers.Right > 0.1f)
                timerRightTriggerPressed += Time.deltaTime;

            if (timerRightTriggerPressed > platformistComponent.ChargeTime)
            {
                // Show pattern + buttons to swap
                // Tant qu'on a pas relaché la gachette
                PlayerState = platformistChargedState;
                platformistComponent.IndexSelection(prevState, state);
            }

            if (prevState.Triggers.Right > 0.1f && state.Triggers.Right < 0.1f)
            {
                PlayerState = freeState;

                rightTriggerHasBeenPressed = false;

                if (timerRightTriggerPressed > platformistComponent.ChargeTime)
                {
                    platformistComponent.CreatePlatforms();
                }

                timerRightTriggerPressed = 0.0f;
            }


            GetComponent<EvolutionPlatformist>().TimerPlatform += Time.deltaTime;
        }
    }





    #endregion

    private void Awake()
    {
        stats.Init(this);
        jumpState = new JumpState(this);
        wallJumpState = new WalljumpState(this);
        dashState = new DashState(this);
        freeState = new FreeState(this);
        expulsedState = new ExpulsedState(this);
        downDashState = new DashDownState(this);
        platformistChargedState = new PlatformistChargedState(this);
        restrainedByGhostState = new RestrainedByGhostState(this);
        frozenState = new FrozenState(this);
    }

    void Start()
    {
        Player = GetComponent<Player>();
        Rb = GetComponent<Rigidbody>();
        deformer = GetComponentInChildren<MeshDeformer>();
        //deformer = GetComponentInChildren<DeformerComputeShader>();
        if (Player == null)
            Debug.Log("Player should not be null");
        PlayerState = freeState;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerState != null)
            PlayerState.OnUpdate();
        if (rb.velocity.y < 0.2f && !IsGrounded)
            HandleBouncing();
        if (rb.velocity.y > 0.05f && !isGrounded)
            HandleJumpDeformer();       
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

        PlayerState.HandleGravity();

        if (isUsingAController)
        {
            // TODO: optimize?
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (GameManager.CurrentState == GameState.Normal)
            {
                HandleJumpWithController();
                HandleMovementWithController();
                HandleDashWithController();
                PlatformistController();
                GhostController();

                if (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabCollectableAndActivate)
                    HandleEvolutionsWithController();
            }
            // TODO: Externalize "state" to handle pause in PauseMenu? //  Remi : Can't manage GamePade(IndexPlayer) Instead, copy not working
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
                if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
                    GameManager.ChangeState(GameState.Paused);
        }



        // handle stateFunction
        if (PlayerState != null)
            PlayerState.OnFixedUpdate();

        // Handle Grounded
        if (player.Rb.velocity.y <= 0.2f && !isGrounded)
        {
            RaycastHit hitInfo;
            // Need to be removed
            if (Physics.SphereCast(transform.position + Vector3.up, 1f, -transform.up, out hitInfo, maxDistanceOffset))
            {
                if (hitInfo.transform.gameObject.GetComponentInParent<Ground>() != null)
                {
                    IsGrounded = true;
                }
            }
            // test
            if (Physics.SphereCast(transform.position + Vector3.up, 1f, -transform.up, out hitInfo, maxDistanceOffset, 1 << LayerMask.NameToLayer("Ground")))
            {
                IsGrounded = true;
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        PlayerState.CollisionEnter(collision);
        float force = 20f;
        ////float forceOffset = 0.1f;


        if (deformer)
        {
            float vel = collision.relativeVelocity.magnitude / collision.contacts.Length;
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                Vector3 point = collision.contacts[i].point;
                deformer.AddDeformingForce(point, vel * force);
            }
        }

    }
    public void OnCollisionStay(Collision collision)
    {
        PlayerState.CollisionStay(collision);
    }
    public void OnCollisionExit(Collision collision)
    {
        PlayerState.CollisionExit(collision);
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
    public virtual void HandleMovementWithController()
    {
        Vector3 initialVelocity = PlayerState.HandleSpeedWithController();

        PlayerState.Move(initialVelocity);

        // TMP Animation
        playerState.HandleControllerAnim();
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
                    playerState.OnJumpPressed();
                }
            }
            else if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released)
            {
                playerState.OnJumpPressed();
            }
        }
    }
    public virtual void HandleDashWithController()
    {
        if (PrevState.Buttons.X == ButtonState.Released && State.Buttons.X == ButtonState.Pressed)
        {
            playerState.OnDashPressed();
        }
        if (GetComponent<EvolutionStrength>() != null)
            if (PrevState.Buttons.Y == ButtonState.Released && State.Buttons.Y == ButtonState.Pressed)
            {
                playerState.OnDownDashPressed();
            }
    }
    public void HandleBouncing()
    {
       
        //Ray ray = new Ray(transform.position, Vector3.down);
        //RaycastHit hit;
        //if (Physics.SphereCast(ray, 1.0f, out hit))
        //{
        //    float force = 2000f;
        //    float forceOffset = 0.1f;
        //    if (deformer)
        //    {
        //        Vector3 point = hit.point;
        //        point += hit.normal * forceOffset;
        //        deformer.AddDeformingForce(point, -force);
        //        deformer.AddDeformingForce(point, force / 5);
        //    }
        //}
    }
    public void HandleJumpDeformer()
    {
        float force = 900f;
        float forceOffset = 0.1f;
        MeshDeformer deformer = GetComponentInChildren<MeshDeformer>();
        if (deformer)
        {
            Vector3 point = transform.position- transform.up;
            point += transform.up * forceOffset;
            deformer.AddDeformingForce(point, -force/3.0f);
            deformer.AddDeformingForce(point, force / 5.0f);
        }
    }

    // TODO : Remi , Export this in camera controls
    public void ChangeDampingValuesCameraFreeLook(float _newValues)
    {
        if (player.cameraReference != null && player.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>())
        {
            //Body
            CinemachineTransposer tr;
            tr = (player.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().GetRig(0).GetCinemachineComponent<CinemachineTransposer>());
            tr.m_XDamping = _newValues;
            tr.m_YDamping = _newValues;
            tr.m_ZDamping = _newValues;

            tr = (player.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().GetRig(1).GetCinemachineComponent<CinemachineTransposer>());
            tr.m_XDamping = _newValues;
            tr.m_YDamping = _newValues;
            tr.m_ZDamping = _newValues;

            tr = (player.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().GetRig(2).GetCinemachineComponent<CinemachineTransposer>());
            tr.m_XDamping = _newValues;
            tr.m_YDamping = _newValues;
            tr.m_ZDamping = _newValues;
        }
    }

}
