using XInputDotNetPure;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour {
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    [SerializeField]
    int maxJumpFrames = 30;
    int currentJumpHeight = 0;
    Player player;
    [SerializeField]
    float jumpPower = 7.0f;

    private void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
            Debug.LogWarning("Player component should not be null");
    }

    void Update () {
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

        prevState = state;
        state = GamePad.GetState(playerIndex);

        if (state.Buttons.A == ButtonState.Pressed && currentJumpHeight < maxJumpFrames)
            Jump();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Ground>() != null)
            currentJumpHeight = 0;
    }

    void Jump()
    {
        player.Rb.AddForce(Vector3.up * jumpPower);
        currentJumpHeight++;
    }
}
