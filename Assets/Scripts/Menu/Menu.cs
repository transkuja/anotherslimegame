using XInputDotNetPure;
using UnityEngine;

public class Menu : MonoBehaviour {
    // Controls
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    bool isUsingAController = false;
    GamePadState state;
    GamePadState prevState;

    public GameObject playerStart;
    private bool isSceneSet = false;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSceneSet) return;

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
            for (int i = 0; i < 20; i++)
            {
                if (Input.GetKeyDown("joystick 1 button " + i.ToString()))
                {
                    SetScene();
                }
            }
        }
        else
        {
            if (Input.anyKey)
            {
                SetScene();
            }
        }
    }

    public void SetScene()
    {
        isSceneSet = true;
        transform.gameObject.SetActive(false);
        playerStart.gameObject.SetActive(true);
    }
}
