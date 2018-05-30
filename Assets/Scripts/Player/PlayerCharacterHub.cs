using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogState { Normal, Dialog };


public class PlayerCharacterHub : PlayerCharacter {

    private PlayerControllerHub playerController;
    private PNJController pnjController;

    private DialogState dialogState;

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

    public float gravityFactor = 1.0f;

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
            if (playerController)
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.CurrentState, value.ToString(), (int)playerController.playerIndex);
            else
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.CurrentState, value.ToString(), -1);
#endif
        }
    }
    public JumpManager JumpManager
    {
        get
        {
            if (!jumpManager && GetComponent<JumpManager>())
                jumpManager = GetComponent<JumpManager>();

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
            if (playerController)
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.IsGrounded, value.ToString(), (int)playerController.playerIndex);
            else
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.IsGrounded, value.ToString(), -1);
#endif
            if (value == true)
            {
                if (isGrounded == false)
                {
                    jumpState.NbJumpMade = 0;
                    downDashState.nbDashDownMade = 0;
                    dashState.nbDashMade = 0;
#if UNITY_EDITOR
                    if (JumpManager != null && (playerController && !playerController.tryByPassJumpStop))
                        JumpManager.Stop();
#endif
                    Anim.SetBool("isExpulsed", false);

                    if (PendingStepSound)
                    {
                        if (playerState != underwaterState)
                        {
                            if (AudioManager.Instance != null && AudioManager.Instance.sandStepFx != null)
                            {
                                if (pnjController && pnjController.myAudioSource != null)
                                {
                                    pnjController.myAudioSource.PlayOneShot(AudioManager.Instance.sandStepFx, 5);
                                }
                                else
                                {
                                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.sandStepFx, 10.0f, 1.0f);
                                }
                            }
                
                        }
                        PendingStepSound = false;
                    }
                }
                if (PlayerState != underwaterState)
                {
                    if (DustTrailParticles)
                    {
                        DustTrailParticles.Play();
                    }
                }
            }
            else
            {
                gravityFactor = 1.0f;
                if (DustTrailParticles)
                {
                    DustTrailParticles.Stop();
                }
            }
            Anim.SetBool("isGrounded", value);
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
            if (playerController)
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), (int)playerController.playerIndex);
            else
                DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.NbJumpMade, value.ToString(), -1);
#endif
        }
    }
    public bool PendingStepSound { get { return pendingStepSound; } set { pendingStepSound = value; } }

    public DialogState DialogState
    {
        get
        {
            return dialogState;
        }

        set
        {
            if( value == DialogState.Dialog)
            {
                if(GetComponent<PNJController>() && GetComponent<PNJMessage>())
                {
                    // play fx
                    if (AudioManager.Instance != null && AudioManager.Instance.pnjFilleFx && AudioManager.Instance.pnjGarçonFx)
                    {
                        AudioManager.Instance.PlayOneShot(PNJDialogUtils.GetDialogSound(GetComponent<PNJMessage>().pnjName), 1.2f, PNJDialogUtils.GetDialogPitch(GetComponent<PNJMessage>().pnjName));
                    }

                }

            }
            dialogState = value;
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
        teleportState = new TeleportState(this);
        PlayerState = freeState;

        playerController = GetComponent<PlayerControllerHub>();

        pnjController = GetComponent<PNJController>();
    }

    public void Start()
    {
        raycastOffsetPlayer = GetComponent<SphereCollider>().radius;

    }

    public void Update()
    {
        if (PlayerState != null)
            PlayerState.OnUpdate();

        if ((Rb.velocity.y < 0.0f && IsGrounded) || (Rb.velocity.y >= 0.0f && jumpState.NbJumpMade == 0))
        {
            if (!Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck))
            {
                IsGrounded = false;
            }

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
            if (LandingParticles)
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
        
        }

        if (!collision.transform.GetComponent<Player>())
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist, groundLayersToCheck)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.right, Vector3.down, raycastDist, groundLayersToCheck))
            {
                if (dotProduct > 0.8f && dotProduct < 1.2f)
                    gravityFactor = 0.2f;
                IsGrounded = true;
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