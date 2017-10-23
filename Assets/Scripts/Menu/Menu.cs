using XInputDotNetPure;
using UnityEngine;
using UnityEngine.UI;

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

    public void ToogleCountdownText(bool visible)
    {
        transform.GetChild(2).gameObject.SetActive(visible);
    }

    public void RefreshCountDown(float countdown)
    {
        int minutes = Mathf.FloorToInt(countdown / 60);
        int seconds = (int)countdown % 60;

        transform.GetChild(2).GetComponent<Text>().text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void SetScene()
    {
        isSceneSet = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        playerStart.gameObject.SetActive(true);
    }
}
