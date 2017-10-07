using XInputDotNetPure;
using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState testState;
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
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        if (testState.IsConnected)
        {
            // TODO: optimize?
            prevState = state;
            state = GamePad.GetState(playerIndex);

            HandleMovementWithController();
            HandleJumpWithController();
        }
        else
        {
            jumpButtonWasPressed = jumpPressed;
            jumpPressed = Input.GetKeyDown(KeyCode.Space);
            HandleMovementWithKeyBoard();
            HandleJumpWithKeyboard();
        }
    }

    private void HandleMovementWithKeyBoard()
    {
        Vector3 initialVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        initialVelocity.Normalize();
        initialVelocity *= (Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.95f) ? GameManager.MaxMovementSpeed : GameManager.MaxMovementSpeed / 2.0f;

        player.Rb.velocity = new Vector3(initialVelocity.x, player.Rb.velocity.y, initialVelocity.z);
    }

    private void HandleMovementWithController()
    {
        Vector3 initialVelocity = new Vector3(state.ThumbSticks.Left.X, 0.0f, state.ThumbSticks.Left.Y);
        initialVelocity.Normalize();
        initialVelocity *= (Mathf.Abs(state.ThumbSticks.Left.X) + Mathf.Abs(state.ThumbSticks.Left.Y) > 0.95f) ? GameManager.MaxMovementSpeed : GameManager.MaxMovementSpeed / 2.0f;

        player.Rb.velocity = new Vector3(initialVelocity.x, player.Rb.velocity.y, initialVelocity.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ground>() != null)
        {
            if (testState.IsConnected ? state.Buttons.A == ButtonState.Released : true)
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

        //// Charge jump if A button is pressed for a "long" time and only if on the ground
        //if (jumpPressed && chargeFactor < 1.0f && isReadyForNextJumpInput)
        //{
        //    chargeFactor += jumpChargeSpeed * Time.unscaledDeltaTime;
        //    // Force max charge jump if the charge reach maximum charge
        //    if (chargeFactor > 1.0f)
        //        Jump(GameManager.JumpUnit);
        //}

        //// Jump when the A button is released and only if on the ground
        //if (jumpButtonWasPressed && Input.GetKeyUp(KeyCode.Space) && isReadyForNextJumpInput)
        //    Jump(GameManager.JumpUnit * chargeFactor);

        //// Prevent input in the air
        //if (Input.GetKeyUp(KeyCode.Space) && isWaitingForNextRelease)
        //{
        //    isWaitingForNextRelease = false;
        //    isReadyForNextJumpInput = true;
        //}
    }

    void Jump(float jumpPower)
    {
        player.Rb.AddForce(Vector3.up * jumpPower);
        isReadyForNextJumpInput = false;
        isWaitingForNextRelease = false;
        chargeFactor = 0.0f;
    }
}
