using XInputDotNetPure;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    // Controls
    bool playerIndexSet = false;
    bool isUsingAController = false;

    public GameObject playerStart;
    private bool isSceneSet = false;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSceneSet) return;

        //// TODO: externaliser pour le comportement multi
        //if (!playerIndexSet)
        //{
        //    isUsingAController = false;
        //    for (int i = 0; i < 4; ++i)
        //    {
        //        PlayerIndex testPlayerIndex = (PlayerIndex)i;
        //        GamePadState testState = GamePad.GetState(testPlayerIndex);
        //        if (testState.IsConnected)
        //        {
        //            playerIndexSet = true;
        //            isUsingAController = true;
        //        }
        //    }
        //}


        if (Input.anyKey)
        {
            SetScene();
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
