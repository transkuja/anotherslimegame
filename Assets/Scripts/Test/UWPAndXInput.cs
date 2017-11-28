using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if NETFX_CORE
using Windows.Gaming.Input;
#endif

namespace UWPAndXInput
{
    public class GamePad
    {
        public GamePad()
        {

        }

        public static GamePadState GetState(PlayerIndex playerIndex)
        {
            GamePadState state;
#if NETFX_CORE
            state = FillGamePadStateStruct(Gamepad.Gamepads[(int)playerIndex].GetCurrentReading());
#else
            state = FillGamePadStateStruct(XInputDotNetPure.GamePad.GetState((XInputDotNetPure.PlayerIndex)playerIndex));
#endif
            return state;
        }
#if NETFX_CORE
        static GamePadState FillGamePadStateStruct(GamepadReading _state)
        {
            GamePadState state;
            state.Buttons.A = (ButtonState)_state.Buttons.A;
            state.Buttons.B = (ButtonState)_state.Buttons.B;
            state.Buttons.X = (ButtonState)_state.Buttons.X;
            state.Buttons.Y = (ButtonState)_state.Buttons.Y;

            state.Buttons.Back = (ButtonState)_state.Buttons.Back;
            state.Buttons.Start = (ButtonState)_state.Buttons.Start;
            state.Buttons.Guide = (ButtonState)_state.Buttons.Guide;

            state.Buttons.LeftShoulder = (ButtonState)_state.Buttons.LeftShoulder;
            state.Buttons.RightShoulder = (ButtonState)_state.Buttons.RightShoulder;

            state.Buttons.LeftStick = (ButtonState)_state.Buttons.LeftStick;
            state.Buttons.RightStick = (ButtonState)_state.Buttons.RightStick;

            state.DPad.Down = (ButtonState)_state.DPad.Down;
            state.DPad.Up = (ButtonState)_state.DPad.Up;
            state.DPad.Left = (ButtonState)_state.DPad.Left;
            state.DPad.Right = (ButtonState)_state.DPad.Right;

            state.Triggers.Left = _state.Triggers.Left;
            state.Triggers.Right = _state.Triggers.Right;

            state.ThumbSticks.Left.X = _state.ThumbSticks.Left.X;
            state.ThumbSticks.Left.Y = _state.ThumbSticks.Left.Y;
            state.ThumbSticks.Right.X = _state.ThumbSticks.Right.X;
            state.ThumbSticks.Right.Y = _state.ThumbSticks.Right.Y;

            state.IsConnected = _state.IsConnected;
            state.PacketNumber = _state.PacketNumber;

            return state;
        }
#else
        static GamePadState FillGamePadStateStruct(XInputDotNetPure.GamePadState _state)
        {
            GamePadState state;
            state.Buttons.A = (ButtonState)_state.Buttons.A;
            state.Buttons.B = (ButtonState)_state.Buttons.B;
            state.Buttons.X = (ButtonState)_state.Buttons.X;
            state.Buttons.Y = (ButtonState)_state.Buttons.Y;

            state.Buttons.Back = (ButtonState)_state.Buttons.Back;
            state.Buttons.Start = (ButtonState)_state.Buttons.Start;
            state.Buttons.Guide = (ButtonState)_state.Buttons.Guide;

            state.Buttons.LeftShoulder = (ButtonState)_state.Buttons.LeftShoulder;
            state.Buttons.RightShoulder = (ButtonState)_state.Buttons.RightShoulder;

            state.Buttons.LeftStick = (ButtonState)_state.Buttons.LeftStick;
            state.Buttons.RightStick = (ButtonState)_state.Buttons.RightStick;

            state.DPad.Down = (ButtonState)_state.DPad.Down;
            state.DPad.Up = (ButtonState)_state.DPad.Up;
            state.DPad.Left = (ButtonState)_state.DPad.Left;
            state.DPad.Right = (ButtonState)_state.DPad.Right;

            state.Triggers.Left = _state.Triggers.Left;
            state.Triggers.Right = _state.Triggers.Right;

            state.ThumbSticks.Left.X = _state.ThumbSticks.Left.X;
            state.ThumbSticks.Left.Y = _state.ThumbSticks.Left.Y;
            state.ThumbSticks.Right.X = _state.ThumbSticks.Right.X;
            state.ThumbSticks.Right.Y = _state.ThumbSticks.Right.Y;

            state.IsConnected = _state.IsConnected;
            state.PacketNumber = _state.PacketNumber;

            return state;
        }
#endif

        public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZone)
        {
            Debug.Log("WARNING: GetState(Playerindex, GamePadDeadZone) not yet supported for UWP");
#if NETFX_CORE
            return null;
#else
            return FillGamePadStateStruct(XInputDotNetPure.GamePad.GetState((XInputDotNetPure.PlayerIndex)playerIndex, (XInputDotNetPure.GamePadDeadZone)deadZone));
#endif
        }
        public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        {
#if NETFX_CORE
            
#else
            XInputDotNetPure.GamePad.SetVibration((XInputDotNetPure.PlayerIndex)playerIndex, leftMotor, rightMotor);
#endif
        }
    }

    public struct GamePadState
    {
        public uint PacketNumber;
        public bool IsConnected;
        public GamePadButtons Buttons;
        public GamePadDPad DPad;
        public GamePadTriggers Triggers;
        public GamePadThumbSticks ThumbSticks;
    }

    public struct GamePadButtons
    {
        public ButtonState Start;
        public ButtonState Back;
        public ButtonState LeftStick;
        public ButtonState RightStick;
        public ButtonState LeftShoulder;
        public ButtonState RightShoulder;
        public ButtonState Guide;
        public ButtonState A;
        public ButtonState B;
        public ButtonState X;
        public ButtonState Y;
    }

    public struct GamePadDPad
    {
        public ButtonState Up;
        public ButtonState Down;
        public ButtonState Left;
        public ButtonState Right;
    }

    public struct GamePadTriggers
    {
        public float Left;
        public float Right;
    }

    public struct GamePadThumbSticks
    {
        public StickValue Left;
        public StickValue Right;

        public struct StickValue
        {
            public float X;
            public float Y;
        }
    }

    public enum ButtonState
    {
        Pressed = 0,
        Released = 1
    }

    public enum PlayerIndex
    {
        One = 0,
        Two = 1,
        Three = 2,
        Four = 3
    }

    public enum GamePadDeadZone
    {
        Circular = 0,
        IndependentAxes = 1,
        None = 2
    }
}
