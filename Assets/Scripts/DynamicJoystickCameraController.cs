using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class DynamicJoystickCameraController : MonoBehaviour {
    bool isUsingAController = false;
    bool playerIndexSet = false;
    public PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    bool once = false;

    Cinemachine.CinemachineFreeLook freelookCamera;
    // Use this for initialization
    void Start () {
        freelookCamera = GetComponent<Cinemachine.CinemachineFreeLook>();
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        if (GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponent<PlayerController>().IsUsingAController)
        {
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if (prevState.IsConnected != state.IsConnected)
            {
                freelookCamera.m_XAxis.m_InputAxisName = string.Empty;
                freelookCamera.m_YAxis.m_InputAxisName = string.Empty;
                once = false;
            }

            freelookCamera.m_XAxis.m_InputAxisValue = Mathf.Abs(state.ThumbSticks.Right.X) > 0.1f ? -state.ThumbSticks.Right.X : 0;
            freelookCamera.m_YAxis.m_InputAxisValue = Mathf.Abs(state.ThumbSticks.Right.Y) > 0.1f ? state.ThumbSticks.Right.Y : 0;
            //Need a more complex function ?
            if (GameManager.Instance.PlayerStart.PlayersReference[0].GetComponent<PlayerController>().IsGrounded)
                freelookCamera.m_XAxis.m_InputAxisValue += Mathf.Abs(state.ThumbSticks.Left.X) > 0.1f ? -state.ThumbSticks.Left.X* Mathf.Lerp(0.5f, 1.0f, Mathf.Abs(state.ThumbSticks.Left.X))/2.0f : 0;
            else
                freelookCamera.m_XAxis.m_InputAxisValue += Mathf.Abs(state.ThumbSticks.Left.X) > 0.1f ? (-state.ThumbSticks.Left.X * Mathf.Lerp(0.5f, 1.0f, Mathf.Abs(state.ThumbSticks.Left.X)))/2.0f : 0;

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
                freelookCamera.m_XAxis.m_InputAxisName = "Mouse X";
                freelookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            }

        }
    }
}
