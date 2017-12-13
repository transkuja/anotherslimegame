using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using Cinemachine;

public class DynamicJoystickCameraController : MonoBehaviour {
    public PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    Vector3 startLowOffset;
    Vector3 startMidOffset;
    Vector3 startHighOffset;

    public bool TurnCameraWithLThumb = true;

    public float cameraXAdjuster = 0.4f;
    public float cameraYAdjuster = 0.4f;

    private float timer = 0.0f;

    Cinemachine.CinemachineFreeLook freelookCamera;

    bool needToTendToMiddleRig = false;
    float lerpOldValue;
    float lerpValue;

    [Range(0.1f, 2.0f)]
    public float lerpTendToMiddleRigSpeed = 0.85f;
    public PlayerController associatedPlayerController;

    [SerializeField]
    float notGroundedAttenuationFactor = 0.1f;

    bool previouslyTendedToMiddleRig = true;

    void Start () {
        freelookCamera = GetComponent<Cinemachine.CinemachineFreeLook>();
        startHighOffset = (freelookCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>()).m_TrackedObjectOffset;
        startMidOffset = (freelookCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>()).m_TrackedObjectOffset;
        startLowOffset = (freelookCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>()).m_TrackedObjectOffset;
    }
	
	void Update () {
        if (associatedPlayerController == null)
            return;

        if (associatedPlayerController.IsUsingAController)
        {
            prevState = state;
            state = GamePad.GetState(playerIndex);

            if ((prevState.Buttons.RightStick == ButtonState.Released && state.Buttons.RightStick == ButtonState.Pressed) 
                || (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                timer = 0.0f;
                freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
            }

            if (freelookCamera.m_RecenterToTargetHeading.m_enabled)
            {
                timer += Time.deltaTime;
                if (timer >= freelookCamera.m_RecenterToTargetHeading.m_RecenterWaitTime + freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime)
                {
                    freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
                }
            }

            if (Utils.Abs(state.ThumbSticks.Right.X) > 0.1f)
            {
                TurnCameraWithLThumb = false;
                freelookCamera.m_XAxis.m_InputAxisValue = -state.ThumbSticks.Right.X * cameraXAdjuster;
                freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
                needToTendToMiddleRig = false;
            }
            else
                freelookCamera.m_XAxis.m_InputAxisValue = 0;

            if (Utils.Abs(state.ThumbSticks.Right.Y) > 0.1f)
            {
                TurnCameraWithLThumb = false;
                freelookCamera.m_YAxis.m_InputAxisValue = state.ThumbSticks.Right.Y * cameraYAdjuster;
                freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
                needToTendToMiddleRig = false;

            }
            else
                freelookCamera.m_YAxis.m_InputAxisValue = 0;

            if ((Utils.Abs(state.ThumbSticks.Right.X) + Utils.Abs(state.ThumbSticks.Right.Y)) < 0.1f)
                TurnCameraWithLThumb = true;

            if (TurnCameraWithLThumb)
            {
                ////Need a more complex function ?
                freelookCamera.m_XAxis.m_InputAxisValue += 
                        Utils.Abs(state.ThumbSticks.Left.X) > 0.1f ? 
                            (freelookCamera.m_XAxis.m_InvertAxis ? -1 : 1) * state.ThumbSticks.Left.X * Mathf.Lerp(0.5f, 1.0f, Utils.Abs(state.ThumbSticks.Left.X)) / 2.0f *
                                ((associatedPlayerController.IsGrounded) ? 1 : notGroundedAttenuationFactor)
                            : 0
                        ;
            }

            if (Utils.Abs(state.ThumbSticks.Left.Y) > 0.1f)
            {
                needToTendToMiddleRig = true;
                lerpOldValue = freelookCamera.m_YAxis.Value;
                lerpValue = 0.0f;
            }

            TendToMiddleRig(associatedPlayerController);
        }
    }

    public void TendToMiddleRig(PlayerController _pc)
    {
        if (needToTendToMiddleRig && lerpValue < 1.0f)
        {
            bool _tendToTopRig = 
                (associatedPlayerController.GetComponentInParent<PlatformGameplay>() != null || !associatedPlayerController.IsGrounded)
                && associatedPlayerController.PlayerState != associatedPlayerController.platformistChargedState;

            if (previouslyTendedToMiddleRig && _tendToTopRig)
            {
                previouslyTendedToMiddleRig = false;
                lerpOldValue = freelookCamera.m_YAxis.Value;
                lerpValue = 0.0f;
            }
            else
            {
                previouslyTendedToMiddleRig = true;
            }

            lerpValue += Time.deltaTime * lerpTendToMiddleRigSpeed;
            float rigTarget = _tendToTopRig ? 1.0f : 0.5f;
            freelookCamera.m_YAxis.Value = Mathf.Lerp(lerpOldValue, rigTarget, lerpValue);
            if (lerpValue > 1.0f)
                needToTendToMiddleRig = false;
        }
    }

}
