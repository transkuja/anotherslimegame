using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using UnityEngine.SceneManagement;

public static class Controls {
    public static int keyboardIndex = 0;

    #region Menu Controls
    public static bool MenuDefaultMoveUp(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q));
        }
        else
        {
            return (currentState.ThumbSticks.Left.Y > 0.5f && prevState.ThumbSticks.Left.Y < 0.5f) || (currentState.ThumbSticks.Left.X < -0.5f && prevState.ThumbSticks.Left.X > -0.5f);
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
                || (currentState.ThumbSticks.Left.Y < -0.5f && prevState.ThumbSticks.Left.Y > -0.5f);
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
            return (Input.GetAxis("Mouse X"));
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

    #region Player Controller Hub
    public static float HubMoveX(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return Input.GetAxisRaw("HorizontalMoveP1");
        }
        else
        {
            return currentState.ThumbSticks.Left.X;
        }
    }

    public static float HubMoveY(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return Input.GetAxisRaw("VerticalMoveP1");
        }
        else
        {
            return currentState.ThumbSticks.Left.Y;
        }
    }

    public static float HubCameraX(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return Input.GetAxis("Mouse X");
        }
        else
        {
            return currentState.ThumbSticks.Right.X;
        }
    }

    public static float HubCameraY(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return Input.GetAxis("Mouse Y");
        }
        else
        {
            return currentState.ThumbSticks.Right.Y;
        }
    }

    public static bool HubJump(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Space));
        }
        else
        {
            return (currentState.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released);
        }
    }

    public static bool HubDash(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetMouseButtonDown(0));
        }
        else
        {
            return (currentState.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released);
        }
    }

    public static bool HubStomp(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetMouseButtonDown(1));
        }
        else
        {
            return (currentState.Buttons.Y == ButtonState.Pressed && prevState.Buttons.Y == ButtonState.Released);
        }
    }

    public static bool HubTeleportToPlayer(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKey(KeyCode.T));
        }
        else
        {
            return (currentState.Buttons.B == ButtonState.Pressed && currentState.Buttons.Y == ButtonState.Pressed);
        }
    }

    public static bool HubPNJNextMsg(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return));
        }
        else
        {
            return (currentState.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released);
        }
    }

    public static bool HubPNJPreviousMsg(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1));
        }
        else
        {
            return (currentState.Buttons.B == ButtonState.Pressed && prevState.Buttons.B == ButtonState.Released);
        }
    }

    public static bool HubInteract(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.E));
        }
        else
        {
            return (currentState.Buttons.B == ButtonState.Pressed && prevState.Buttons.B == ButtonState.Released);
        }
    }

    public static bool PauseGame(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Escape));
        }
        else
        {
            return (currentState.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released);
        }
    }

    public static bool PlatformSpawnPressed(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetMouseButtonDown(1));
        }
        else
        {
            return (prevState.Triggers.Right < 0.1f && currentState.Triggers.Right > 0.1f);
        }
    }

    public static bool PlatformSpawnHold(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetMouseButton(1));
        }
        else
        {
            return (currentState.Triggers.Right > 0.1f);
        }
    }

    public static bool PlatformSpawnReleased(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetMouseButtonUp(1));
        }
        else
        {
            return (prevState.Triggers.Right > 0.1f && currentState.Triggers.Right < 0.1f);
        }
    }

    public static bool ChangePlatformsPattern(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetAxis("Mouse ScrollWheel") > 0.07f || Input.GetAxis("Mouse ScrollWheel") < -0.07f);
        }
        else
        {
            return (prevState.Buttons.LeftShoulder == ButtonState.Released && currentState.Buttons.LeftShoulder == ButtonState.Pressed)
                || (prevState.Buttons.RightShoulder == ButtonState.Released && currentState.Buttons.RightShoulder == ButtonState.Pressed);
        }
    }

    public static bool LeaveATrail(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetMouseButton(1));
        }
        else
        {
            return currentState.Triggers.Right > 0.1f;
        }
    }
    #endregion

    #region Minigame specific

    public static bool FoodLowerButton(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow));
        }
        else
        {
            return (currentState.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released);
        }
    }

    public static bool FoodUpperButton(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow));
        }
        else
        {
            return (currentState.Buttons.Y == ButtonState.Pressed && prevState.Buttons.Y == ButtonState.Released);
        }
    }

    public static bool FoodLeftButton(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow));
        }
        else
        {
            return (currentState.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released);
        }
    }

    public static bool FoodRightButton(GamePadState prevState, GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow));
        }
        else
        {
            return (currentState.Buttons.B == ButtonState.Pressed && prevState.Buttons.B == ButtonState.Released);
        }
    }

    public static bool KartSpeedUp(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow));
        }
        else
        {
            return (currentState.Buttons.A == ButtonState.Pressed);
        }
    }

    public static bool KartSpeedDown(GamePadState currentState, int playerIndex)
    {
        if (playerIndex == keyboardIndex)
        {
            return (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow));
        }
        else
        {
            return (currentState.Buttons.B == ButtonState.Pressed);
        }
    }

    #endregion

    public static int nbPlayersSelectedInMenu = 0;

    public static bool IsKeyboardUsed()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            return nbPlayersSelectedInMenu > keyboardIndex;
        }
        else
        {
            if (GameManager.Instance.DataContainer != null)
            {
                return GameManager.Instance.DataContainer.nbPlayers > keyboardIndex;
            }
            // Default case, should never happen in build
            return false;
        }
    }
    /*  Move zqsd ok
     * Camera mouse axis ok =/
     * Dash clic gauche ok
     * Jump space ok 
     * Stomp clic droit ok
     * Interact E à tester
     * Validate dialog E clic gauche à tester
     * Return backspace clic droit à tester
     * Pause echap ok
     * 
     * Spawn platforms clic droit ok
     * Change platforms pattern roulette souris ok
     * Tp to other player T à tester
     * 
     * Food: tester
     * Kart:
     * avancer
     * reculer
     * */
}
