using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterHub : PlayerCharacter {

    [SerializeField] public Stats stats;

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

    // Deformer
    private MeshDeformer deformer;

    // Component : 
    private PlayerState playerState;
    private PlayerState previousPlayerState;
    private JumpManager jumpManager;

    // Ground
    public LayerMask groundLayersToCheck;
    [SerializeField] bool isGrounded = true;
    public float raycastDist = 1.5f;
    public float raycastOffsetPlayer;

    //  others
    private bool isGravityEnabled = true;

    public bool pendingStepSound = false;

    // Particles
    private ParticleSystem dustTrailParticles;
    private ParticleSystem dashParticles;
    private ParticleSystem bubbleParticles;
    private ParticleSystem splashParticles;
    private ParticleSystem waterTrailParticles;
    private ParticleSystem landingParticles;

    // Delegate events in RUNNER:
    public delegate void OnPlayerDeath(int id);
    public OnPlayerDeath OnDeathEvent;


    #region Getters/Setters
    /// StateManagment
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
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.CurrentState, value.ToString(), (int)((PlayerControllerHub)Pc).playerIndex);
#endif
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

    public bool IsGrounded
    {
        get
        {
            return isGrounded;
        }

        set
        {

#if UNITY_EDITOR
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.IsGrounded, value.ToString(), (int)((PlayerControllerHub)Pc).playerIndex);
#endif
            if (value == true)
            {
                if (isGrounded == false)
                {
                    jumpState.NbJumpMade = 0;
                    downDashState.nbDashDownMade = 0;
                    dashState.nbDashMade = 0;
#if UNITY_EDITOR
                    if (GetComponent<JumpManager>() != null && !((PlayerControllerHub)Pc).tryByPassJumpStop)
                        GetComponent<JumpManager>().Stop();
#endif
                    Anim.SetBool("isExpulsed", false);

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
                    if (DustTrailParticles && DustTrailParticles.GetComponent<ParticleSystem>() != null)
                    {
                        DustTrailParticles.GetComponent<ParticleSystem>().Play();
                    }
                }
            }
            else
            {
                if (DustTrailParticles && DustTrailParticles.GetComponent<ParticleSystem>() != null)
                {
                    DustTrailParticles.GetComponent<ParticleSystem>().Stop();
                }

            }

            isGrounded = value;
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
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.GravityEnabled, isGravityEnabled.ToString(), (int)((PlayerControllerHub)Pc).playerIndex);
#endif
        }
    }

    public ParticleSystem DustTrailParticles
    {
        get
        {
            return dustTrailParticles;
        }

        set
        {
            dustTrailParticles = value;
        }
    }
    public ParticleSystem DashParticles
    {
        get
        {
            return dashParticles;
        }

        set
        {
            dashParticles = value;
        }
    }
    public ParticleSystem BubbleParticles
    {
        get
        {
            return bubbleParticles;
        }

        set
        {
            bubbleParticles = value;
        }
    }
    public ParticleSystem SplashParticles
    {
        get
        {
            return splashParticles;
        }

        set
        {
            splashParticles = value;
        }
    }
    public ParticleSystem WaterTrailParticles
    {
        get
        {
            return waterTrailParticles;
        }

        set
        {
            waterTrailParticles = value;
        }
    }
    public ParticleSystem LandingParticles
    {
        get
        {
            return landingParticles;
        }

        set
        {
            landingParticles = value;
        }
    }
    #endregion

    private void Awake()
    {
        Pc = GetComponent<PlayerControllerHub>();


        stats.Init(this);
        jumpState = new JumpState(this, (PlayerControllerHub)Pc);
        wallJumpState = new WalljumpState(this, (PlayerControllerHub)Pc);
        dashState = new DashState(this, (PlayerControllerHub)Pc);
        freeState = new FreeState(this, (PlayerControllerHub)Pc);
        expulsedState = new ExpulsedState(this, (PlayerControllerHub)Pc);
        downDashState = new DashDownState(this, (PlayerControllerHub)Pc);
        platformistChargedState = new PlatformistChargedState(this, (PlayerControllerHub)Pc);
        restrainedByGhostState = new RestrainedByGhostState(this, (PlayerControllerHub)Pc);
        frozenState = new FrozenState(this, (PlayerControllerHub)Pc);
        underwaterState = new UnderwaterState(this, (PlayerControllerHub)Pc);
        pausedState = new PausedState(this, (PlayerControllerHub)Pc);
        PlayerState = freeState;

    }

    public void Start()
    {
        raycastOffsetPlayer = GetComponent<SphereCollider>().radius;

        dustTrailParticles = transform.GetChild((int)PlayerChildren.DustTrailParticles).GetComponent<ParticleSystem>();
        dashParticles = transform.GetChild((int)PlayerChildren.DashParticles).GetComponent<ParticleSystem>();
        bubbleParticles = transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>();
        splashParticles = transform.GetChild((int)PlayerChildren.SplashParticles).GetComponent<ParticleSystem>();
        waterTrailParticles = transform.GetChild((int)PlayerChildren.WaterTrailParticles).GetComponent<ParticleSystem>();
        landingParticles = transform.GetChild((int)PlayerChildren.LandingParticles).GetComponent<ParticleSystem>();

        deformer = GetComponentInChildren<MeshDeformer>();
    }

    public void Update()
    {
        if (PlayerState != null)
            PlayerState.OnUpdate();
        if (Rb.velocity.y > 0.05f && !IsGrounded)
            HandleJumpDeformer();
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

    public void OnCollisionEnter(Collision collision)
    {
        PlayerState.CollisionEnter(collision);

        // If we hit the floor
        float dotProduct = Vector3.Dot(collision.contacts[0].normal, Vector3.up);

        if (dotProduct > 0.9f && dotProduct < 1.1f)
        {
            ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[LandingParticles.emission.burstCount];
            LandingParticles.emission.GetBursts(bursts);

            bursts[0].minCount = (short)Mathf.Lerp(2.0f, 6.0f, (collision.relativeVelocity.magnitude / 150.0f));
            bursts[0].maxCount = (short)Mathf.Lerp(3.0f, 8.0f, (collision.relativeVelocity.magnitude / 150.0f));
            LandingParticles.emission.SetBursts(bursts);
            LandingParticles.Play();
        }
        if (deformer)
        {
            float force = 20f;
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

    public void HandleJumpDeformer()
    {
        float force = 900f;
        float forceOffset = 0.1f;

        if (deformer)
        {
            Vector3 point = transform.position - transform.up;
            point += transform.up * forceOffset;
            deformer.AddDeformingForce(point, -force / 3.0f);
            deformer.AddDeformingForce(point, force / 5.0f);
        }
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

    public void OnDeath()
    {
        //Respawner.RespawnProcess(GetComponent<Player>());
        if (OnDeathEvent != null)
            OnDeathEvent((int)((PlayerControllerHub)Pc).playerIndex);
    }
}
