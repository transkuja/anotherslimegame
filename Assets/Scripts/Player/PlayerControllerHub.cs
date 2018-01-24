using UWPAndXInput;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Cinemachine;


// Gère les input selon l'input appelle des action codée dans une playerState.

public class PlayerControllerHub : PlayerController
{
    // Component : 
    private PlayerState playerState;
    private PlayerState previousPlayerState;
    private JumpManager jumpManager;

    // gamePad
    GamePadState state;
    GamePadState prevState;
    bool isUsingAController = false;

    // evolution : 
    int selectedEvolution = 0;

    //  others
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

    [SerializeField] public Stats stats;
    [SerializeField] bool isGrounded = true;
    public bool DEBUG_hasBeenSpawnedFromTool = false;

    public bool canDoubleJump = true; // A Priori c'es du legacy, mais j'ai pas toutpigé.

    public float raycastDist = 0.75f;
    float raycastOffsetPlayer;

    public bool IsUnderWater = false;

    public bool forceCameraRecenter = false;

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
                jumpState.nbJumpMade = 20; // Very high value, reset when releasing button AND being grounded
                downDashState.nbDashDownMade = 0;
                dashState.nbDashMade = 0;
                if (GetComponent<JumpManager>() != null)
                    GetComponent<JumpManager>().Stop();
                GetComponent<Player>().Anim.SetBool("isExpulsed", false);
                if (dustTrailParticles && dustTrailParticles.GetComponent<ParticleSystem>() != null)
                {
                    dustTrailParticles.GetComponent<ParticleSystem>().Play();
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
        raycastOffsetPlayer = GetComponent<SphereCollider>().radius;
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
        //if (player.Rb.velocity.y < 0.01f && !isGrounded)
        //{
        //    RaycastHit hitInfo;
        //    // Need to be removed
        //    if (Physics.SphereCast(transform.position + Vector3.up, 1f, -transform.up, out hitInfo, maxDistanceOffset))
        //    {
        //        if (hitInfo.transform.GetComponentInParent<Ground>() != null || hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
        //        {
        //            IsGrounded = true;
        //        }
        //    }
        //    //// test
        //    //if (Physics.SphereCast(transform.position + Vector3.up, 1f, -transform.up, out hitInfo, maxDistanceOffset, 1 << LayerMask.NameToLayer("Ground")))
        //    //{
        //    //    IsGrounded = true;
        //    //}
        //}
        if (Rb.velocity.y < 0.0f && IsGrounded)
        {
            if (!Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.right, Vector3.down, raycastDist)
                    && !Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.right, Vector3.down, raycastDist))
                IsGrounded = false;
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

    public void OnCollisionEnter(Collision collision)
    {
        PlayerState.CollisionEnter(collision);
        float force = 20f;
        ////float forceOffset = 0.1f;
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

        if (!collision.transform.GetComponent<Player>() && !IsUnderWater)
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.forward, Vector3.down, raycastDist)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f + raycastOffsetPlayer * transform.right, Vector3.down, raycastDist)
                    || Physics.Raycast(transform.position + Vector3.up * 0.5f - raycastOffsetPlayer * transform.right, Vector3.down, raycastDist))
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
        // Charge jump if A button is pressed for a "long" time and only if on the ground
        //if (isGrounded)


        if (state.Buttons.A == ButtonState.Pressed && chargeFactor < 1.0f)
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


        if (state.Buttons.A == ButtonState.Released)
        {
            if (IsGrounded) jumpState.nbJumpMade = 0;
            chargeFactor = 0.0f;
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
