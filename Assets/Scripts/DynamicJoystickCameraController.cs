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
                freelockCamera.m_XAxis.m_InputAxisName = string.Empty;
                freelockCamera.m_YAxis.m_InputAxisName = string.Empty;
                once = false;
            }

            freelockCamera.m_XAxis.m_InputAxisValue = Mathf.Abs(state.ThumbSticks.Right.X) > 0.1f ? -state.ThumbSticks.Right.X : 0;
            freelockCamera.m_YAxis.m_InputAxisValue = Mathf.Abs(state.ThumbSticks.Right.Y) > 0.1f ? state.ThumbSticks.Right.Y : 0;
            //Need a more complex function ?
            freelockCamera.m_XAxis.m_InputAxisValue += Mathf.Abs(state.ThumbSticks.Left.X) > 0.1f ? -state.ThumbSticks.Left.X* Mathf.Lerp(0.5f, 1.0f, Mathf.Abs(state.ThumbSticks.Left.X)) : 0;
            /*if(freelockCamera.LookAt.GetComponent<Rigidbody>().velocity.magnitude >0.01f)
            {
                freelockCamera.m_RecenterToTargetHeading.m_enabled = true;
            }
            else
            {
                freelockCamera.m_RecenterToTargetHeading.m_enabled = false;
            }*/
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
