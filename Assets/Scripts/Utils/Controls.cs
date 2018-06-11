using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public static class Controls {
    public static int keyboardIndex = 5;

    #region Menu Controls
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

    public static bool MenuValidation(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Return));
        }
        else
        {
            return (currentState.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released);
        }
    }

    public static bool MenuIsReady(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Return));
        }
        else
        {
            return (currentState.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released)
                || (currentState.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released);
        }
    }

    public static bool MenuCancellation(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape));
        }
        else
        {
            return (currentState.Buttons.B == ButtonState.Pressed && prevState.Buttons.B == ButtonState.Released);
        }
    }

    public static bool MenuRandomize(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.R));
        }
        else
        {
            return (currentState.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released);
        }
    }

    public static float MenuRotateCharacter(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0);
        }
        else
        {
            return currentState.ThumbSticks.Right.X;
        }
    }

    public static bool MenuBuy(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Space));
        }
        else
        {
            return (currentState.Buttons.Y == ButtonState.Pressed && prevState.Buttons.Y == ButtonState.Released);
        }
    }

    public static bool MenuNextCustomizableType(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.E));
        }
        else
        {
            return (currentState.Buttons.RightShoulder == ButtonState.Pressed && prevState.Buttons.RightShoulder == ButtonState.Released);
        }
    }

    public static bool MenuPreviousCustomizableType(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.A));
        }
        else
        {
            return (currentState.Buttons.LeftShoulder == ButtonState.Pressed && prevState.Buttons.LeftShoulder == ButtonState.Released);
        }
    }

    public static bool MenuNextCustomizableValue(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.D));
        }
        else
        {
            return currentState.ThumbSticks.Left.X > 0.5f && prevState.ThumbSticks.Left.X < 0.5f;
        }
    }

    public static bool MenuPreviousCustomizableValue(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Q));
        }
        else
        {
            return currentState.ThumbSticks.Left.X < -0.5f && prevState.ThumbSticks.Left.X > -0.5f;
        }
    }

    public static bool MenuNextMinigameTypeY(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.S));
        }
        else
        {
            return (currentState.ThumbSticks.Left.Y < -0.5f && prevState.ThumbSticks.Left.Y > -0.5f);
        }
    }

    public static bool MenuPreviousMinigameTypeY(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Z));
        }
        else
        {
            return (currentState.ThumbSticks.Left.Y > 0.5f && prevState.ThumbSticks.Left.Y < 0.5f);
        }
    }
    #endregion
}
