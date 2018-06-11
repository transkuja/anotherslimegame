using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public static class Controls {
    public static int keyboardIndex = 5;

    public static bool MenuDefaultMoveUp(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q));
        }
        else
        {
            return (currentState.ThumbSticks.Left.Y > 0.75f && prevState.ThumbSticks.Left.Y < 0.75f) || (currentState.ThumbSticks.Left.X < -0.5f && prevState.ThumbSticks.Left.X > -0.5f);
        }
    }

    public static bool MenuDefaultMoveDown(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D));
        }
        else
        {
            return (currentState.ThumbSticks.Left.X > 0.5f && prevState.ThumbSticks.Left.X < 0.5f)
                || (currentState.ThumbSticks.Left.Y < -0.75f && prevState.ThumbSticks.Left.Y > -0.75f);
        }
    }
}
