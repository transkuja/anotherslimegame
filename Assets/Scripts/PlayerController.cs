using XInputDotNetPure;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum SkillState
{
    Ready,
    Charging,
    Dashing,
    Cooldown
}


public enum BrainState
{
    Free,
    Occupied
}


[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {

    bool playerIndexSet = false;

    public PlayerIndex playerIndex;
    bool isUsingAController = false;
    GamePadState state;
    GamePadState prevState;

    // TMP
    bool jumpPressed = false;
    bool jumpButtonWasPressed = false;
    [HideInInspector]public bool canMoveXZ = true;
    [HideInInspector]public bool canJump = true;
    int currentJumpHeight = 0;
    Player player;

    bool isReadyForNextJumpInput = true;
    bool isWaitingForNextRelease = false;
    float chargeFactor = 0.0f;

    [SerializeField] public Stats stats = new Stats(); // tu mens intellisense
    [SerializeField]
    [Range(5, 1000)] float jumpChargeSpeed = 15.0f;

    int selectedEvolution = 0;
    [SerializeField]
    [Range(70, 250)]
    float customGravity; // 90 seems good
    [SerializeField]
    [Range(0, 250)]
    float airForce;

    public bool isFreeFalling = false;
    // TODO: send this value to jumpManager
    bool isGrounded = true;
    public bool canDoubleJump = true;
    bool hasJumpButtonBeenReleased = true;

    public bool isGravityEnabled = true;

    // TMP??
    RaycastHit hitInfo;
    float maxDistanceOffset = 2.0f;

    // Dashing variables
    private BrainState brainState;
    private SkillState dashingState;
    private SkillState strenghState;


    public float dashingTimer;
    public float dashingMaxTimer;
    public float dashingCooldownTimer;
    public float dashingCooldownMaxTimer;
    public float dashingVelocity;
    public float distanceToReach;
    //public Vector3 myPositionBeforeDashing;


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
    private void Awake()
    {
        stats.Init();
        JumpManager jumpManager = GetComponent<JumpManager>();
        if (jumpManager != null)
            customGravity = jumpManager.GetGravity(stats.Get(Stats.StatType.GROUND_SPEED));
    }

    public SkillState DashingState
    {
        get
        {
            return dashingState;
        }

        set
        {
            switch (value)
            {
                case SkillState.Charging:
                case SkillState.Dashing:
                    isGravityEnabled = false;
                    BrainState = BrainState.Occupied;
                    break;
                default:
                    if (!isGravityEnabled) isGravityEnabled = true;
                    BrainState = BrainState.Free;
                    break;
            }
            dashingState = value;
        }
    }

    public SkillState StrenghState
    {
        get
        {
            return strenghState;
        }

        set
        {
            switch (value)
            {
                case SkillState.Charging:
                    isGravityEnabled = false;
                    BrainState = BrainState.Occupied;
                    break;
                case SkillState.Dashing:
                    BrainState = BrainState.Occupied;
                    break;
                default:
                    if (!isGravityEnabled) isGravityEnabled = true;
                    BrainState = BrainState.Free;
                    break;
            }
            strenghState = value;
        }
    }

    public BrainState BrainState
    {
        get
        {
            return brainState;
        }

        set
        {
            if (value == BrainState.Occupied)
            {
                canJump = false;
                canMoveXZ = false;
            }
            else {
                canJump = true;
                canMoveXZ = true;
            }
            brainState = value;
        }
    }

    private void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
            Debug.LogWarning("Player component should not be null");

        // Dashing initialisation
        dashingMaxTimer = 0.15f;
        dashingTimer = dashingMaxTimer;
        dashingCooldownMaxTimer = 0.5f;
        dashingCooldownTimer = dashingCooldownMaxTimer;
        dashingVelocity = 100.0f;
        dashingState = SkillState.Cooldown;
        brainState = BrainState.Free;
    }

    void FixedUpdate ()
    {
        if (isGravityEnabled)
        {
            // TODO : Vector.down remove the minus ???? lol
            player.Rb.AddForce(-customGravity * Vector3.up, ForceMode.Acceleration);
            if (player.Rb.velocity.y < -10.0f)
            {
                // No Inputs Mode
                isFreeFalling = true;
            }
            else
            {
                isFreeFalling = false;
            }

        }
        //player.Rb.velocity = new Vector3(player.Rb.velocity.x, -customGravity, player.Rb.velocity.z);
        // TODO: externaliser pour le comportement multi
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
                if (canMoveXZ)
                    HandleMovementWithController();
                if (canJump)
                    HandleJumpWithController();
                if (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabCollectableAndActivate)
                    HandleEvolutionsWithController();

                // Dash
                DashControllerState();
                // Strengh
                if (GetComponent<EvolutionStrengh>() != null)
                    StrenghControllerState();
            }
            // TODO: Externalize "state" to handle pause in PauseMenu? //  Remi : Can't manage GamePade(IndexPlayer) Instead, copy not working
            if( SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0) )
                if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
                    GameManager.ChangeState(GameState.Paused);

        }
        else
        {
            // Keyboard
            if (GameManager.CurrentState == GameState.Normal)
            {
                jumpButtonWasPressed = jumpPressed;
                jumpPressed = Input.GetKeyDown(KeyCode.Space);
                HandleMovementWithKeyBoard();
                HandleJumpWithKeyboard();
            }
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
                if (Input.GetKeyDown(KeyCode.Escape))
                    GameManager.ChangeState(GameState.Paused);
        }
    }

    private void DashControllerState()
    {
        switch (dashingState)
        {
            case SkillState.Ready:
                if (brainState == BrainState.Occupied) return;

                if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed)
                {
                    GetComponent<JumpManager>().Stop();
                    DashingState = SkillState.Dashing;
                    ChangeDumpingValuesCameraFreeLook(0f);
                }
                break;
            case SkillState.Dashing:
                player.Rb.velocity = transform.forward * dashingVelocity;
    
                dashingTimer -= Time.fixedDeltaTime;
                // ? Timer ?
                if (dashingTimer <= 0.0f)
                {
                    dashingTimer = dashingMaxTimer;
                    DashingState = SkillState.Cooldown;
                }
                break;
            case SkillState.Cooldown:

                dashingCooldownTimer -= Time.fixedDeltaTime;
                if (dashingCooldownTimer <= 0.0f)
                {
                    ChangeDumpingValuesCameraFreeLook(0.2f);
                    dashingCooldownTimer = dashingCooldownMaxTimer;
                    DashingState = SkillState.Ready;
                }
                break;
            default: break;
        }
    }
    private void StrenghControllerState()
    {
        switch (strenghState)
        {
            case SkillState.Ready:
                if (brainState == BrainState.Occupied) return;

                if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
                {
                    ChangeDumpingValuesCameraFreeLook(0f);
                    GetComponent<EvolutionStrengh>().DashStart();
                    StrenghState = SkillState.Charging;
                }

                break;
            case SkillState.Charging:

                if (state.Buttons.Y == ButtonState.Pressed)
                {
                    ChangeDumpingValuesCameraFreeLook(0f);
                    GetComponent<EvolutionStrengh>().Levitate();
                }
                else if (state.Buttons.Y == ButtonState.Released)
                {

                    GetComponent<EvolutionStrengh>().LaunchDash();
                    StrenghState = SkillState.Dashing;
                }
                break;
            case SkillState.Dashing:
                ChangeDumpingValuesCameraFreeLook(0.2f); // TODO : Remi ,Need to be call only once
                break;
            case SkillState.Cooldown:
                break;
            default: break;
        }
    }

    private void HandleEvolutionsWithController()
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

    private void HandleMovementWithKeyBoard()
    {
        
        Vector3 initialVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        initialVelocity.Normalize();
        initialVelocity *= (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.95f) ? stats.Get(Stats.StatType.GROUND_SPEED) : stats.Get(Stats.StatType.GROUND_SPEED) / 2.0f;

        player.Rb.velocity = new Vector3(initialVelocity.x, player.Rb.velocity.y, initialVelocity.z);

        Vector3 camVectorForward = new Vector3(player.cameraReference.transform.GetChild(0).forward.x, 0.0f, player.cameraReference.transform.GetChild(0).transform.forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward + initialVelocity.x * player.cameraReference.transform.GetChild(0).right + Vector3.up * player.Rb.velocity.y;

        player.Rb.velocity = velocityVec;
        transform.LookAt(transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z));

    }

    private void HandleMovementWithController()
    {          
        Vector3 initialVelocity = new Vector3(state.ThumbSticks.Left.X, 0.0f, state.ThumbSticks.Left.Y);

        initialVelocity.Normalize();
        if (!isFreeFalling)
            initialVelocity *= (Mathf.Abs(state.ThumbSticks.Left.X) + Mathf.Abs(state.ThumbSticks.Left.Y) > 0.95f) ? stats.Get(Stats.StatType.GROUND_SPEED) : stats.Get(Stats.StatType.GROUND_SPEED) / 2.0f;
        else
            initialVelocity *= (Mathf.Abs(state.ThumbSticks.Left.X) + Mathf.Abs(state.ThumbSticks.Left.Y) > 0.95f) ? stats.Get(Stats.StatType.AIR_CONTROL) : stats.Get(Stats.StatType.AIR_CONTROL) / 2.0f;

        Vector3 camVectorForward = new Vector3(player.cameraReference.transform.GetChild(0).forward.x, 0.0f, player.cameraReference.transform.GetChild(0).forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward + Vector3.up * player.Rb.velocity.y;
        if (isGrounded)
            velocityVec += initialVelocity.x * player.cameraReference.transform.GetChild(0).right;

        player.Rb.velocity = velocityVec;
        transform.LookAt(transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z) + initialVelocity.x * player.cameraReference.transform.GetChild(0).right);


        // Animation
        player.GetComponent<Player>().Anim.SetFloat("MouvementSpeed", Mathf.Abs(state.ThumbSticks.Left.X) > Mathf.Abs(state.ThumbSticks.Left.Y) ? Mathf.Abs(state.ThumbSticks.Left.X) : Mathf.Abs(state.ThumbSticks.Left.Y));
        player.GetComponent<Player>().Anim.SetBool("isWalking", ((Mathf.Abs(state.ThumbSticks.Left.X) > 0.02f) || Mathf.Abs(state.ThumbSticks.Left.Y) > 0.02f) && player.GetComponent<PlayerController>().IsGrounded);
        }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<Ground>() != null)
        {
            //if (Physics.Raycast(transform.position, -transform.up, out hitInfo, maxDistanceOffset))
            //{
            //    if (hitInfo.transform.gameObject.GetComponentInParent<Ground>() != null)
            //        IsGrounded = true;
            //}

            //Debug.Log("normal" + collision.contacts[0].normal);
            //Debug.Log("angle" + Vector3.Angle(collision.contacts[0].normal, transform.up));
            //if (Vector3.Angle(collision.contacts[0].normal, transform.up) < 45)
            //{
            //    IsGrounded = true;
            //}

            if (isUsingAController ? state.Buttons.A == ButtonState.Released : true)
            {
                isReadyForNextJumpInput = true;
                canDoubleJump = true;
            }
            else
            {
                canDoubleJump = false;
                isWaitingForNextRelease = true;
            }
        }
   
    }

    private void Update()
    {
        if (player.Rb.velocity.y <= 0.2f && !isGrounded)
        {
            if (Physics.SphereCast(transform.position + Vector3.up, 1f, -transform.up, out hitInfo, maxDistanceOffset))
            {
                if (hitInfo.transform.gameObject.GetComponentInParent<Ground>() != null)
                {
                    IsGrounded = true;
                }
            }
        }
        stats.Update();
    }

    private void HandleJumpWithController()
    {
        if (!IsGrounded && !canDoubleJump)
            return;

        // Charge jump if A button is pressed for a "long" time and only if on the ground
        if (isGrounded)
        {
            if (state.Buttons.A == ButtonState.Pressed && chargeFactor < 1.0f && isReadyForNextJumpInput)
            {
                chargeFactor += jumpChargeSpeed * Time.unscaledDeltaTime;
                // Force max charge jump if the charge reach maximum charge
                if (chargeFactor > 1.0f)
                {
                    Jump();
                }
            }

            if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released && isReadyForNextJumpInput)
            {
                Jump();
            }
        }

        if (state.Buttons.A == ButtonState.Released)
            hasJumpButtonBeenReleased = true;

        // Jump when the A button is released and only if on the ground
        if (!isReadyForNextJumpInput && state.Buttons.A == ButtonState.Pressed && canDoubleJump && hasJumpButtonBeenReleased)
        {
            GetComponent<JumpManager>().Stop();
            canDoubleJump = false;
            Jump();
        }

        // Prevent input in the air
        if (state.Buttons.A == ButtonState.Released && isWaitingForNextRelease)
        {
            isWaitingForNextRelease = false;
            isReadyForNextJumpInput = true;
            canDoubleJump = true;
        }
    }

    private void HandleJumpWithKeyboard()
    {
        if (!IsGrounded)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && isReadyForNextJumpInput)
            Jump();

    }

    void Jump()
    {
        IsGrounded = false;
        JumpManager jm;
        if (jm = GetComponent<JumpManager>())
            jm.Jump(stats.Get(Stats.StatType.GROUND_SPEED),JumpManager.JumpEnum.Basic);
        else
            Debug.LogError("No jump manager attached to player!");

        isReadyForNextJumpInput = false;
        isWaitingForNextRelease = false;
        hasJumpButtonBeenReleased = false;
        chargeFactor = 0.0f;
    }

  
    // TODO : Remi , Export this in camera controls
    public void ChangeDumpingValuesCameraFreeLook(float _newValues)
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            //Body
            CinemachineTransposer tr;
            tr = ((CinemachineTransposer)(player.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().GetRig(0).GetCinemachineComponent(CinemachineCore.Stage.Body)));
            tr.m_XDamping = _newValues;
            tr.m_YDamping = _newValues;
            tr.m_ZDamping = _newValues;

            tr = ((CinemachineTransposer)(player.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().GetRig(1).GetCinemachineComponent(CinemachineCore.Stage.Body)));
            tr.m_XDamping = _newValues;
            tr.m_YDamping = _newValues;
            tr.m_ZDamping = _newValues;

            tr = ((CinemachineTransposer)(player.cameraReference.transform.GetChild(1).GetComponent<Cinemachine.CinemachineFreeLook>().GetRig(2).GetCinemachineComponent(CinemachineCore.Stage.Body)));
            tr.m_XDamping = _newValues;
            tr.m_YDamping = _newValues;
            tr.m_ZDamping = _newValues;
        }
    }
}
