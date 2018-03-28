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
    public PlayerControllerHub associatedPlayerController;

    [SerializeField]
    float notGroundedAttenuationFactor = 0.33f;

    bool previouslyTendedToMiddleRig = true;

    // Small areas settings
    bool forceMiddleRig = false;
    float defaultMinDistanceFromTarget = 0.6f;
    float extremeMinDistanceFromTarget = 5.0f;
    [SerializeField]
    float smallAreaHeight = 15.0f;
    [SerializeField]
    float verySmallAreaHeight = 7.0f;

    public enum CameraState { Default, SmallArea, VerySmallArea }

    private CameraState currentState = CameraState.Default;

    public CameraState CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            currentState = value;
#if UNITY_EDITOR
            DebugTools.UpdatePlayerInfos(DebugTools.DebugPlayerInfos.CameraState, value.ToString());
#endif
        }
    }

    void Start () {
        freelookCamera = GetComponent<Cinemachine.CinemachineFreeLook>();
        startHighOffset = (freelookCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>()).m_TrackedObjectOffset;
        startMidOffset = (freelookCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>()).m_TrackedObjectOffset;
        startLowOffset = (freelookCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>()).m_TrackedObjectOffset;

        // Ugly shit due to camera prefab being shitty
        //cameraXAdjuster = 0.3f;
        //cameraYAdjuster = 0.05f;
        notGroundedAttenuationFactor = 0.33f;
        lerpTendToMiddleRigSpeed = 0.85f;
        defaultMinDistanceFromTarget = 0.6f;
        extremeMinDistanceFromTarget = 5.0f;
        smallAreaHeight = 15.0f;
        verySmallAreaHeight = 7.0f;
        CurrentState = CameraState.Default;
        ///////////////////////////////////////////////////
    }

    void Update () {
        if (associatedPlayerController == null)
            return;

        if (associatedPlayerController.IsUsingAController)
        {
            prevState = state;
            state = GamePad.GetState(playerIndex);

            RecenterBehaviour();
            CameraStickBehaviour();

            // Handle camera behaviour when the player is moving on the sides
            if (TurnCameraWithLThumb && !forceMiddleRig)
                freelookCamera.m_XAxis.m_InputAxisValue += UpdateCameraXAxisValue(state.ThumbSticks.Left.X);
            

            if (Utils.Abs(state.ThumbSticks.Left.Y) > 0.1f)
            {
                needToTendToMiddleRig = true;
                lerpOldValue = freelookCamera.m_YAxis.Value;
                lerpValue = 0.0f;
            }

            if (!forceMiddleRig)
                TendToMiddleRig(associatedPlayerController);
        }
    }

    public void ChangeCameraBehaviour(CameraBehaviour _newBehaviour, bool _reset = false)
    {
        switch (_newBehaviour)
        {
            case CameraBehaviour.SmallArea:
                ChangeCameraState(CameraState.SmallArea);
                break;
            case CameraBehaviour.VSmallArea:
                ChangeCameraState(CameraState.VerySmallArea);
                break;
            default:
                ChangeCameraState(CameraState.Default);
                break;
        }
    }

    /// <summary>
    /// Handle camera states 
    /// </summary>
    /// <param name="_newState"></param>
    void ChangeCameraState(CameraState _newState)
    {
        if (_newState == CurrentState)
            return;

        CurrentState = _newState;
        switch (_newState)
        {
            case CameraState.SmallArea:
                forceMiddleRig = true;
                freelookCamera.m_YAxis.Value = 0.5f;
                freelookCamera.Follow = associatedPlayerController.transform;
                freelookCamera.GetComponent<Cinemachine.CinemachineCollider>().m_MinimumDistanceFromTarget = defaultMinDistanceFromTarget;
                freelookCamera.GetComponent<Cinemachine.CinemachineCollider>().m_AvoidObstacles = true;
                break;
            case CameraState.VerySmallArea:
                forceMiddleRig = true;
                freelookCamera.m_YAxis.Value = 0.5f;
                freelookCamera.Follow = null;
                freelookCamera.GetComponent<Cinemachine.CinemachineCollider>().m_MinimumDistanceFromTarget = extremeMinDistanceFromTarget;
                freelookCamera.GetComponent<Cinemachine.CinemachineCollider>().m_AvoidObstacles = false;
                freelookCamera.transform.position = associatedPlayerController.currentCameraTrigger.vSmallAreaStandbyTransform.position;
                break;
            default:
                forceMiddleRig = false;
                freelookCamera.GetComponent<Cinemachine.CinemachineCollider>().m_MinimumDistanceFromTarget = defaultMinDistanceFromTarget;
                freelookCamera.Follow = associatedPlayerController.transform;
                freelookCamera.GetComponent<Cinemachine.CinemachineCollider>().m_AvoidObstacles = true;
                break;
        }
    }

    /// <summary>
    /// Throw raycasts to check if the player is in a small area, then change camera state if necessary
    /// </summary>
    void SmallAreaBehaviour()
    {
        
                    ChangeCameraState(CameraState.VerySmallArea);
                    ChangeCameraState(CameraState.SmallArea);
            ChangeCameraState(CameraState.Default);
    }

    /// <summary>
    /// Handle behaviours when moving right stick
    /// </summary>
    void CameraStickBehaviour()
    {

        if (Utils.Abs(state.ThumbSticks.Right.X) > 0.1f)
        {
            TurnCameraWithLThumb = false;
            freelookCamera.m_XAxis.m_InputAxisValue = -state.ThumbSticks.Right.X * cameraXAdjuster;
            freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
            needToTendToMiddleRig = false;
        }
        else
            freelookCamera.m_XAxis.m_InputAxisValue = 0;

        if (!forceMiddleRig)
        {
            if (Utils.Abs(state.ThumbSticks.Right.Y) > 0.1f)
            {
                TurnCameraWithLThumb = false;
                freelookCamera.m_YAxis.m_InputAxisValue = state.ThumbSticks.Right.Y * cameraYAdjuster;
                freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
                needToTendToMiddleRig = false;

            }
            else
                freelookCamera.m_YAxis.m_InputAxisValue = 0;
        }

        if ((Utils.Abs(state.ThumbSticks.Right.X) + Utils.Abs(state.ThumbSticks.Right.Y)) < 0.1f)
            TurnCameraWithLThumb = true;
    }

    /// <summary>
    /// Handle camera recenter when pressing the proper button
    /// </summary>
    void RecenterBehaviour()
    {
        if (((prevState.Buttons.RightStick == ButtonState.Released && state.Buttons.RightStick == ButtonState.Pressed)
                || (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed))
                && associatedPlayerController.IsGrounded)
        {

            timer = 0.0f;

            float dotProduct = Vector3.Dot(transform.parent.GetChild(0).forward.normalized, associatedPlayerController.transform.forward.normalized);
            // TODO: this content may be written better
            if (associatedPlayerController.Rb.velocity.magnitude > 0.0f)
            {
                if (dotProduct < -0.8f)
                {
                    freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0.4f;
                    associatedPlayerController.forceCameraRecenter = true;
                    freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
                }
                else if (dotProduct > 0.0f)
                {
                    freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0.7f;
                    freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
                    associatedPlayerController.forceCameraRecenter = false;
                }
            }
            else
            {
                freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime = 0.7f;
                freelookCamera.m_RecenterToTargetHeading.m_enabled = true;
                associatedPlayerController.forceCameraRecenter = false;
            }
        }

        if (freelookCamera.m_RecenterToTargetHeading.m_enabled)
        {
            timer += Time.deltaTime;
            if (timer >= freelookCamera.m_RecenterToTargetHeading.m_RecenterWaitTime + freelookCamera.m_RecenterToTargetHeading.m_RecenteringTime)
            {
                freelookCamera.m_RecenterToTargetHeading.m_enabled = false;
                associatedPlayerController.forceCameraRecenter = false;
            }
        }
    }

    /// <summary>
    /// Handle camera behaviour when moving on sides
    /// </summary>
    /// <param name="_leftStickXInput"></param>
    /// <returns></returns>
    float UpdateCameraXAxisValue(float _leftStickXInput)
    {
        float updateValue = 0.0f;

        if (Utils.Abs(_leftStickXInput) > 0.1f)
        {
            updateValue = (freelookCamera.m_XAxis.m_InvertAxis) ? -1 : 1;     
            updateValue *= (_leftStickXInput * Mathf.Lerp(0.5f, 1.0f, Utils.Abs(_leftStickXInput)) / 2.0f) * cameraXAdjuster * 2.8f;

            if (!associatedPlayerController.IsGrounded) updateValue *= notGroundedAttenuationFactor;
        }

        return updateValue;
    }

    public void TendToMiddleRig(PlayerControllerHub _pc)
    {
        if (needToTendToMiddleRig && lerpValue < 1.0f)
        {
            bool _tendToTopRig =
                // Camera should tend to top rig if player is on a platformGameplay ...
                (associatedPlayerController.GetComponentInParent<PlatformGameplay>() != null)
                // .. but not if there's a camera trigger saying so ...
                && (associatedPlayerController.currentCameraTrigger == null || associatedPlayerController.currentCameraTrigger.behaviour != CameraBehaviour.ShowAbovePlatforms)
                // ... if the player is not charging to spawn platforms ...
                && associatedPlayerController.PlayerState != associatedPlayerController.platformistChargedState
                // ... or if the player is in main tower (may be deprecated)
                && !associatedPlayerController.Player.isInMainTower;

            // Reset lerp value only if lerp target changed (from middle rig to top)
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

            // Lerp camera only on ground
            if (associatedPlayerController.IsGrounded)
            {
                lerpValue += Time.deltaTime * lerpTendToMiddleRigSpeed;
                float rigTarget = _tendToTopRig ? 1.0f : 0.5f;
                freelookCamera.m_YAxis.Value = Mathf.Lerp(lerpOldValue, rigTarget, lerpValue);
                if (lerpValue > 1.0f)
                    needToTendToMiddleRig = false;
            }
        }
    }

}
