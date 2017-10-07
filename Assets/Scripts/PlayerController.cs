using XInputDotNetPure;
using UnityEngine;


[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    int currentJumpHeight = 0;
    Player player;

    bool isReadyForNextJumpInput = true;
    bool isWaitingForNextRelease = false;
    float chargeFactor = 0.0f;

    [SerializeField]
    [Range(5, 1000)] float jumpChargeSpeed = 15.0f;

    [Header("Jump Settings")]
    [Tooltip("Maximum number of frames forces are added to the player while pressing Jump button")]
    [SerializeField] int maxJumpFrames; // 5
    //[Tooltip("Power of the force added to the player each frame the Jump button is pressed")]
    //[SerializeField] float jumpPower; // 50.0f
    [Tooltip("Delay before next jump after hitting the ground")]
    [SerializeField] float jumpDelay; // 0.3f

    private void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
            Debug.LogWarning("Player component should not be null");
    }

    void Update () {
        // TODO: externaliser pour le comportement multi
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        // TODO: optimize?
        prevState = state;
        state = GamePad.GetState(playerIndex);
        Debug.Log(chargeFactor);

        if (state.Buttons.A == ButtonState.Pressed && chargeFactor < 1.0f && isReadyForNextJumpInput)
        {
            chargeFactor += jumpChargeSpeed * Time.unscaledDeltaTime;
            if (chargeFactor > 1.0f)
                Jump(GameManager.JumpUnit);
        }

        if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released && isReadyForNextJumpInput)
            Jump(GameManager.JumpUnit * chargeFactor);

        if (state.Buttons.A == ButtonState.Released && isWaitingForNextRelease)
        {
            isWaitingForNextRelease = false;
            isReadyForNextJumpInput = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ground>() != null)
        {
            if (state.Buttons.A == ButtonState.Released)
                isReadyForNextJumpInput = true;
            else
                isWaitingForNextRelease = true;
        }
    }

    void Jump(float jumpPower)
    {
        player.Rb.AddForce(Vector3.up * jumpPower);
        isReadyForNextJumpInput = false;
        isWaitingForNextRelease = false;
        chargeFactor = 0.0f;
    }
}
