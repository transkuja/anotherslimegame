using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class DynamicJoystickCameraController : MonoBehaviour {
    bool isUsingAController = false;
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    bool once = false;

    Cinemachine.CinemachineFreeLook freelockCamera;
    // Use this for initialization
    void Start () {
        freelockCamera = GetComponent<Cinemachine.CinemachineFreeLook>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!playerIndexSet || !prevState.IsConnected)
        {
            isUsingAController = false;
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                    isUsingAController = true;
                }
            }
        }

        if (isUsingAController)
        {
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (prevState.IsConnected != state.IsConnected)
            {
                freelockCamera.m_XAxis.m_InputAxisName = "Joystick X";
                freelockCamera.m_YAxis.m_InputAxisName = "Joystick Y";
                once = false;
            }
        }
        else
        {
            if (!once)
            {
                once = true;
                freelockCamera.m_XAxis.m_InputAxisName = "Mouse X";
                freelockCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            }

        }
    }
}
