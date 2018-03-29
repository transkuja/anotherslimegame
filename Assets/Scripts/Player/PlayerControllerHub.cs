using UWPAndXInput;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Cinemachine;


// Gère les input selon l'input appelle des action codée dans une playerState.

public class PlayerControllerHub : PlayerController
{
    #region Controller
    // jump
    [Range(5, 1000)] float jumpChargeSpeed = 15.0f;

    // Plateformist
    bool rightTriggerHasBeenPressed = false;
    float timerRightTriggerPressed = 0.0f;
    #endregion

    #region HUBCharacter
    public bool DEBUG_hasBeenSpawnedFromTool = false;

    [SerializeField] public Stats stats;

    // Aggregations
    public PlayerCollisionCenter collisionCenter;

    // Ground
    public LayerMask groundLayersToCheck;
    [SerializeField] bool isGrounded = true;
    private float raycastDist = 1.5f;
    private float raycastOffsetPlayer;

    //  others
    private bool isGravityEnabled = true;

    // Component : 
    private PlayerState playerState;
    private PlayerState previousPlayerState;
    private JumpManager jumpManager;

    // Deformer
    private MeshDeformer deformer;

    // Particles
    [SerializeField] GameObject dustTrailParticles;
    public GameObject dashParticles;

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
    public UnderwaterState underwaterState;
    public PausedState pausedState;

    // Delegate events in RUNNER:
    public delegate void OnPlayerDeath(int id);
    public OnPlayerDeath OnDeathEvent;
    #endregion

    public bool forceCameraRecenter = false;
    public bool pendingStepSound = false;

#if UNITY_EDITOR
    private bool tryByPassJumpStop;
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
            PreviousPlayerState = playerState;
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
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.CurrentState, value.ToString(), (int)playerIndex);
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

#if UNITY_EDITOR
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.IsGrounded, value.ToString(), (int)playerIndex);
#endif
            if (value == true)
            {
                if (isGrounded == false)
                {
                    jumpState.NbJumpMade = 0;
                    downDashState.nbDashDownMade = 0;
                    dashState.nbDashMade = 0;
#if UNITY_EDITOR
                    if (GetComponent<JumpManager>() != null && !tryByPassJumpStop)
                        GetComponent<JumpManager>().Stop();
#endif
                    GetComponent<Player>().Anim.SetBool("isExpulsed", false);

                    if (pendingStepSound)
                    {
                        if (playerState != underwaterState)
                        {
                            if (AudioManager.Instance != null && AudioManager.Instance.sandStepFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.sandStepFx, 10.0f);
                        }
                        pendingStepSound = false;
                    }
                }
                if (PlayerState != underwaterState)
                {
                    if (dustTrailParticles && dustTrailParticles.GetComponent<ParticleSystem>() != null)
                    {
                        dustTrailParticles.GetComponent<ParticleSystem>().Play();
                    }
                }
            }
            else
            {
                if (dustTrailParticles && dustTrailParticles.GetComponent<ParticleSystem>() != null)
                {
                    dustTrailParticles.GetComponent<ParticleSystem>().Stop();
                }
                    
            }

            isGrounded = value;
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
   

    public PlayerState PreviousPlayerState
    {
        get
        {
            return previousPlayerState;
        }

        set
        {
            previousPlayerState = value;
        }
    }

    public bool IsGravityEnabled
    {
        get
        {
            return isGravityEnabled;
        }

        set
        {
            isGravityEnabled = value;
#if UNITY_EDITOR
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.GravityEnabled, isGravityEnabled.ToString(), (int)playerIndex);
#endif
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
        EvolutionPlatformist platformistComponent = GetComponent<EvolutionPlatformist>();
        if (platformistComponent != null)
        {
            if (platformistComponent.Charges == 0)
                return;

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


            platformistComponent.TimerPlatform += Time.deltaTime;
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
        underwaterState = new UnderwaterState(this);
        pausedState = new PausedState(this);
        PlayerState = freeState;

    }

    void Start()
    {
        Player = GetComponent<Player>();
        Rb = GetComponent<Rigidbody>();
        deformer = GetComponentInChildren<MeshDeformer>();
        collisionCenter = GetComponent<PlayerCollisionCenter>();

        if (Player == null)
            Debug.Log("Player should not be null");
        raycastOffsetPlayer = GetComponent<SphereCollider>().radius;
    }

    public override void Update()
    {
        if (PlayerState != null)
            PlayerState.OnUpdate();
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


        if (isUsingAController)
        {
            base.Update();

            if (GameManager.CurrentState == GameState.Normal)
            {
                HandleJumpWithController();
                HandleMovementWithController();
                HandleDashWithController();
                PlatformistController();
                GhostController();
            }
        }

        if (Rb.velocity.y < 0.0f && IsGrounded)
        {
            if (!Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck))
                IsGrounded = false;
        }
    }

    public void FixedUpdate()
    {
        // handle stateFunction
        if (PlayerState != null)
            PlayerState.OnFixedUpdate();

        PlayerState.HandleGravity();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        //Gizmos.DrawCube(transform.position + Vector3.up*0.5f, Vector3.one);
        //Gizmos.DrawCube(transform.position + Vector3.up*0.5f + Vector3.down*2.0f, Vector3.one);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.forward,
            transform.position + Vector3.up * 0.5f + Vector3.down * raycastDist + raycastOffsetPlayer * transform.forward);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.forward, transform.position + Vector3.up * 0.5f + Vector3.down * raycastDist - raycastOffsetPlayer * transform.forward);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.right, transform.position + Vector3.up * 0.5f + Vector3.down * raycastDist + raycastOffsetPlayer * transform.right);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.right, transform.position + Vector3.up * 0.5f + Vector3.down * raycastDist - raycastOffsetPlayer * transform.right);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f + transform.forward * 2.0f);
    }
#endif

    public void OnCollisionEnter(Collision collision)
    {
        PlayerState.CollisionEnter(collision);

        float force = 20f;

        // If we hit the floor
        float dotProduct = Vector3.Dot(collision.contacts[0].normal, Vector3.up);

        if (dotProduct > 0.9f && dotProduct < 1.1f)
        {
            ParticleSystem ps = transform.GetChild((int)PlayerChildren.LandingParticles).GetComponent<ParticleSystem>();
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[ps.emission.burstCount];
            ps.emission.GetBursts(bursts);

            bursts[0].minCount = (short)Mathf.Lerp(2.0f, 6.0f, (collision.relativeVelocity.magnitude / 150.0f));
            bursts[0].maxCount = (short)Mathf.Lerp(3.0f, 8.0f, (collision.relativeVelocity.magnitude / 150.0f));
            ps.emission.SetBursts(bursts);
            ps.Play();
        }
        if (deformer)
        {
            //float vibforce = collision.relativeVelocity.magnitude/150.0f;
            //GamePad.VibrateForSeconds(playerIndex, vibforce, vibforce, 0.1f);

            float vel = collision.relativeVelocity.magnitude / collision.contacts.Length;
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                Vector3 point = collision.contacts[i].point;
                deformer.AddDeformingForce(point, vel * force);
                
            }
        }

        if (!collision.transform.GetComponent<Player>())
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck))
                IsGrounded = true;
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

    public virtual void HandleMovementWithController()
    {
        Vector3 initialVelocity = PlayerState.HandleSpeedWithController();

        PlayerState.Move(initialVelocity);

        // TMP Animation
        playerState.HandleControllerAnim();
    }
   
    private void HandleJumpWithController()
    {
        if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed && PlayerState != dashState)
        {
#if UNITY_EDITOR
            tryByPassJumpStop = true;
#endif
            playerState.OnJumpPressed();
        }
#if UNITY_EDITOR
        else
            tryByPassJumpStop = false;
#endif

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

    public void HandleJumpDeformer()
    {
        float force = 900f;
        float forceOffset = 0.1f;

        if (deformer)
        {
            Vector3 point = transform.position- transform.up;
            point += transform.up * forceOffset;
            deformer.AddDeformingForce(point, -force/3.0f);
            deformer.AddDeformingForce(point, force / 5.0f);
        }
    }

    public void OnDeath()
    {
        //Respawner.RespawnProcess(GetComponent<Player>());
        if (OnDeathEvent!=null)
            OnDeathEvent((int)playerIndex);
    }
}
