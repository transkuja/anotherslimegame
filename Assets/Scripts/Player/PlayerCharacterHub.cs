using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterHub : PlayerCharacter {

    private PlayerControllerHub playerController;

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
    public TeleportState teleportState;

    // Deformer
    private MeshDeformer deformer;

    // Component : 
    private PlayerState playerState;
    private PlayerState previousPlayerState;
    private JumpManager jumpManager;

    // Ground
    public LayerMask groundLayersToCheck;
    private bool isGrounded = true;
    private float raycastDist = 1.5f;
    private float raycastOffsetPlayer;

    //  others
    private bool isGravityEnabled = true;

    private bool pendingStepSound = false;

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
            if (GetComponent<PlayerControllerHub>())
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), (int)GetComponent<PlayerControllerHub>().playerIndex);
            else
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), -1);
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
            if (GetComponent<PlayerControllerHub>())
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), (int)GetComponent<PlayerControllerHub>().playerIndex);
            else
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), -1);
#endif
            if (value == true)
            {
                if (isGrounded == false)
                {
                    jumpState.NbJumpMade = 0;
                    downDashState.nbDashDownMade = 0;
                    dashState.nbDashMade = 0;
#if UNITY_EDITOR
                    if (GetComponent<JumpManager>() != null && (playerController && !playerController.tryByPassJumpStop))
                        GetComponent<JumpManager>().Stop();
#endif
                    Anim.SetBool("isExpulsed", false);

                    if (PendingStepSound)
                    {
                        if (playerState != underwaterState)
                        {
                            if (AudioManager.Instance != null && AudioManager.Instance.sandStepFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.sandStepFx, 10.0f);
                        }
                        PendingStepSound = false;
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
            if (GetComponent<PlayerControllerHub>())
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), (int)GetComponent<PlayerControllerHub>().playerIndex);
            else
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), -1);
#endif
        }
    }
    public bool PendingStepSound { get { return pendingStepSound; } set { pendingStepSound = value; } }
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
        teleportState = new TeleportState(this);
        PlayerState = freeState;

        playerController = GetComponent<PlayerControllerHub>();
    }

    public void Start()
    {
        raycastOffsetPlayer = GetComponent<SphereCollider>().radius;

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

        if (!IsGrounded)
        {
            Vector3 tmp = new Vector3(Rb.velocity.x, 0.0f, Rb.velocity.z);
            float dragForceUsed = 45f * Time.deltaTime * 500f;

            if (tmp.magnitude > 3.0f)
            {
                if (!((tmp.x > 0 && tmp.x - tmp.normalized.x * dragForceUsed < 0)
                || (tmp.x < 0 && tmp.x - tmp.normalized.x * dragForceUsed > 0)
                || (tmp.z > 0 && tmp.z - tmp.normalized.z * dragForceUsed < 0)
                || (tmp.z < 0 && tmp.z - tmp.normalized.z * dragForceUsed > 0)))
                    Rb.AddForce(-tmp.normalized * dragForceUsed);
            }
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

            if (bursts.Length > 0)
            {
                bursts[0].minCount = (short)Mathf.Lerp(2.0f, 6.0f, (collision.relativeVelocity.magnitude / 150.0f));
                bursts[0].maxCount = (short)Mathf.Lerp(3.0f, 8.0f, (collision.relativeVelocity.magnitude / 150.0f));
                LandingParticles.emission.SetBursts(bursts);
                LandingParticles.Play();
            }
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
}