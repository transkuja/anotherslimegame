using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if WINDOWS_UWP
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
#if WINDOWS_UWP
            try {
                state = FillGamePadStateStruct(Gamepad.Gamepads[(int)playerIndex].GetCurrentReading());
            }
            catch {
            state.IsConnected = false;
            }
#else
            state = FillGamePadStateStruct(XInputDotNetPure.GamePad.GetState((XInputDotNetPure.PlayerIndex)playerIndex));
            
#endif
            return state;
        }
#if WINDOWS_UWP
        static GamePadState FillGamePadStateStruct(GamepadReading _state)
        {
            //To Complete

            GamePadState state;

            state.Buttons.A = ButtonState.Released;
            state.Buttons.B = ButtonState.Released;
            state.Buttons.X = ButtonState.Released;
            state.Buttons.Y = ButtonState.Released;
            state.Buttons.DPad.Down = ButtonState.Released;
            state.Buttons.DPad.Right = ButtonState.Released;
            state.Buttons.DPad.Up = ButtonState.Released;
            state.Buttons.LeftShoulder = ButtonState.Released;
            state.Buttons.LeftStick = ButtonState.Released;
            state.Buttons.RightShoulder = ButtonState.Released;
            state.Buttons.RightStick  = ButtonState.Released;
            state.Buttons.Back  = ButtonState.Released;
            state.Buttons.Start = ButtonState.Released;

            state.Triggers.Left = _state.LeftTrigger;
            state.Triggers.Right = _state.RightTrigger;
            state.ThumbSticks.Left.X = _state.LeftThumbstickX;
            state.ThumbSticks.Left.Y = _state.LeftThumbstickY;
            state.ThumbSticks.Right.X = _state.RightThumbstickX;
            state.ThumbSticks.Right.Y = _state.RightThumbstickY;

            switch(_state.Buttons)
            {
                case GamepadButtons.A:
                    state.Buttons.A = ButtonState.Pressed;
                    break;
                case GamepadButtons.B:
                    state.Buttons.B = ButtonState.Pressed;
                    break;
                case GamepadButtons.X:
                    state.Buttons.X = ButtonState.Pressed;
                    break;
                case GamepadButtons.Y:
                    state.Buttons.Y = ButtonState.Pressed;
                    break;
                case GamepadButtons.DPadDown :
                    state.Buttons.DPad.Down = ButtonState.Pressed;
                    break;
                case GamepadButtons.DPadLeft :
                    state.Buttons.DPad.Left = ButtonState.Pressed;
                    break;
                case GamepadButtons.DPadRight :
                    state.Buttons.DPad.Right = ButtonState.Pressed;
                    break;
                case GamepadButtons.DPadUp :
                    state.Buttons.DPad.Up = ButtonState.Pressed;
                    break;
                case GamepadButtons.LeftShoulder :
                    state.Buttons.LeftShoulder = ButtonState.Pressed;
                    break;
                case GamepadButtons.LeftThumbstick :
                    state.Buttons.LeftStick = ButtonState.Pressed;
                    break;
                case GamepadButtons.RightShoulder :
                    state.Buttons.RightShoulder = ButtonState.Pressed;
                    break;
                case GamepadButtons.RightThumbstick :
                    state.Buttons.RightStick  = ButtonState.Pressed;
                    break;
                case GamepadButtons.View :
                      state.Buttons.Back  = ButtonState.Pressed;
                    break;
                case GamepadButtons.Menu :
                    state.Buttons.Start = ButtonState.Pressed;
                    break;
                case GamepadButtons.Paddle1 :
                    break;
                case GamepadButtons.Paddle2 :
                    break;
                case GamepadButtons.Paddle3 :
                    break;
                case GamepadButtons.Paddle4 :
                    break;

                //GamepadButtons.None?
                //state.Buttons.Guide?
            }
        
            state.IsConnected = true;

        /*
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
            state.PacketNumber = _state.PacketNumber;*/

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
#if WINDOWS_UWP
            return null;
#else
            return FillGamePadStateStruct(XInputDotNetPure.GamePad.GetState((XInputDotNetPure.PlayerIndex)playerIndex, (XInputDotNetPure.GamePadDeadZone)deadZone));
#endif
        }

        public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        {
#if WINDOWS_UWP
            Gamepad.Gamepads[(int)playerIndex].Vibration.LeftMotor = leftMotor;
            Gamepad.Gamepads[(int)playerIndex].Vibration.RightMotor = rightMotor;
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
