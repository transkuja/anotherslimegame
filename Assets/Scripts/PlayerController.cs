using XInputDotNetPure;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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

    int currentJumpHeight = 0;
    Player player;

    bool isReadyForNextJumpInput = true;
    bool isWaitingForNextRelease = false;
    float chargeFactor = 0.0f;

    [SerializeField]
    [Range(5, 1000)] float jumpChargeSpeed = 15.0f;

    int selectedEvolution = 0;
    [SerializeField]
    [Range(10, 100000)]
    float customGravity;
    // TODO: send this value to jumpManager
    bool isGrounded = true;

    public bool isGravityEnabled = true;

    // TMP??
    RaycastHit hitInfo;
    float maxDistanceOffset = 2.0f;

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
    }

    public GamePadState PrevState
    {
        get
        {
            return prevState;
        }
    }

    private void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
            Debug.LogWarning("Player component should not be null");
    }

    void FixedUpdate ()
    {
        if (isGravityEnabled)
        {
            player.Rb.AddForce(-customGravity * Vector3.up, ForceMode.Acceleration);
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
                HandleMovementWithController();
                HandleJumpWithController();
                if (GameManager.CurrentGameMode.evolutionMode == EvolutionMode.GrabCollectableAndActivate)
                    HandleEvolutionsWithController();
            }
            // TODO: Externalize "state" to handle pause in PauseMenu?
            if( SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0) )
                if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
                    GameManager.ChangeState(GameState.Paused);

        }
        else
        {
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
        initialVelocity *= (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.95f) ? GameManager.MaxMovementSpeed : GameManager.MaxMovementSpeed / 2.0f;

        player.Rb.velocity = new Vector3(initialVelocity.x, player.Rb.velocity.y, initialVelocity.z);

        Vector3 camVectorForward = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward + initialVelocity.x * Camera.main.transform.right + Vector3.up * player.Rb.velocity.y;

        player.Rb.velocity = velocityVec;
        transform.LookAt(transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z));

    }

    private void HandleMovementWithController()
    {
        Vector3 initialVelocity = new Vector3(state.ThumbSticks.Left.X, 0.0f, state.ThumbSticks.Left.Y);
        initialVelocity.Normalize();
        initialVelocity *= (Mathf.Abs(state.ThumbSticks.Left.X) + Mathf.Abs(state.ThumbSticks.Left.Y) > 0.95f) ? GameManager.MaxMovementSpeed : GameManager.MaxMovementSpeed / 2.0f;

        Vector3 camVectorForward = new Vector3(Camera.main.transform.forward.x, 0.0f, Camera.main.transform.forward.z);
        camVectorForward.Normalize();

        Vector3 velocityVec = initialVelocity.z * camVectorForward + initialVelocity.x * Camera.main.transform.right + Vector3.up * player.Rb.velocity.y;
        
        player.Rb.velocity = velocityVec;
        transform.LookAt(transform.position + new Vector3(velocityVec.x, 0.0f, velocityVec.z));
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
                isReadyForNextJumpInput = true;
            else
                isWaitingForNextRelease = true;
        }
    }

    private void Update()
    {
        if (player.Rb.velocity.y <= 0.2f && !isGrounded)
        {
            if (Physics.Raycast(transform.position, -transform.up, out hitInfo, maxDistanceOffset))
            {
                if (hitInfo.transform.gameObject.GetComponentInParent<Ground>() != null)
                {
                    IsGrounded = true;
                }
            }
        }
    }

    private void HandleJumpWithController()
    {
        if (!IsGrounded)
            return;

        // Charge jump if A button is pressed for a "long" time and only if on the ground
        if (state.Buttons.A == ButtonState.Pressed && chargeFactor < 1.0f && isReadyForNextJumpInput)
        {
            chargeFactor += jumpChargeSpeed * Time.unscaledDeltaTime;
            // Force max charge jump if the charge reach maximum charge
            if (chargeFactor > 1.0f)
                Jump();
        }

        // Jump when the A button is released and only if on the ground
        if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released && isReadyForNextJumpInput)
            Jump();

        // Prevent input in the air
        if (state.Buttons.A == ButtonState.Released && isWaitingForNextRelease)
        {
            isWaitingForNextRelease = false;
            isReadyForNextJumpInput = true;
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
            jm.Jump(20,JumpManager.JumpEnum.Basic);
        else
            Debug.LogError("No jump manager attached to player!");

        isReadyForNextJumpInput = false;
        isWaitingForNextRelease = false;
        chargeFactor = 0.0f;
    }
}
