using XInputDotNetPure;
using UnityEngine;
using System;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
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
    [SerializeField]
    [Range(5, 1000)] float movementSpeed = 25.0f;

    int selectedEvolution = 0;

    // TODO: send this value to jumpManager
    bool isGrounded = true;

    private void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
            Debug.LogWarning("Player component should not be null");
    }

    void Update ()
    {
        // TODO: externaliser pour le comportement multi
        if (!playerIndexSet || !prevState.IsConnected)
        {
            isUsingAController = false;
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                    isUsingAController = true;
                }
            }
        }

        if (isUsingAController)
        {
            // TODO: optimize?
            prevState = state;
            state = GamePad.GetState(playerIndex);

            HandleMovementWithController();
            HandleJumpWithController();
            if (GameManager.GameplayType == 2)
                HandleEvolutionsWithController();
        }
        else
        {
            jumpButtonWasPressed = jumpPressed;
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
            HandleMovementWithKeyBoard();
            HandleJumpWithKeyboard();
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
        Vector3 initialVelocity = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        Debug.Log(Input.GetAxis("Horizontal"));
        initialVelocity.Normalize();
        initialVelocity *= (Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")) > 0.95f) ? GameManager.MaxMovementSpeed : GameManager.MaxMovementSpeed / 2.0f;

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
        if (collision.gameObject.GetComponent<Ground>() != null)
        {
            RaycastHit hitInfo;
            float maxDistance = 2.0f;
            if (Physics.Raycast(transform.position, -transform.up, out hitInfo, maxDistance))
            {
                if (hitInfo.transform.gameObject.GetComponent<Ground>() != null)
                    isGrounded = true;
            }

            if (isUsingAController ? state.Buttons.A == ButtonState.Released : true)
                isReadyForNextJumpInput = true;
            else
                isWaitingForNextRelease = true;
        }
    }

    private void HandleJumpWithController()
    {
        // Charge jump if A button is pressed for a "long" time and only if on the ground
        if (state.Buttons.A == ButtonState.Pressed && chargeFactor < 1.0f && isReadyForNextJumpInput)
        {
            chargeFactor += jumpChargeSpeed * Time.unscaledDeltaTime;
            // Force max charge jump if the charge reach maximum charge
            if (chargeFactor > 1.0f)
                Jump(GameManager.JumpUnit);
        }

        // Jump when the A button is released and only if on the ground
        if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released && isReadyForNextJumpInput)
            Jump(GameManager.JumpUnit * chargeFactor);

        // Prevent input in the air
        if (state.Buttons.A == ButtonState.Released && isWaitingForNextRelease)
        {
            isWaitingForNextRelease = false;
            isReadyForNextJumpInput = true;
        }
    }

    private void HandleJumpWithKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isReadyForNextJumpInput)
            Jump(GameManager.JumpUnit);

    }

    void Jump(float jumpPower)
    {
        isGrounded = false;
        player.Rb.AddForce(Vector3.up * jumpPower);
        isReadyForNextJumpInput = false;
        isWaitingForNextRelease = false;
        chargeFactor = 0.0f;
    }
}
