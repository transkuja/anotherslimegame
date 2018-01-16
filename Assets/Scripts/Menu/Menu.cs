using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using UWPAndXInput;

public class Menu : MonoBehaviour {
    public enum MenuState { TitleScreen, ModeSelection, NumberOfPlayers, CustomisationScreen, MinigameSelection }
    MenuState currentState = MenuState.TitleScreen;

    int currentCursor = 0;
    GamePadState prevControllerState;
    GamePadState controllerState;

    int selectedMode = -1;
    int nbPlayers = -1;

    public void SetMode(int _modeSelected)
    {
        selectedMode = _modeSelected;
    }

    public void SetNbPlayers(int _nbPlayersSelected)
    {
        nbPlayers = _nbPlayersSelected;
    }

    private void Start()
    {
        SetState(MenuState.TitleScreen);
    }

    void Update()
    {
        prevControllerState = controllerState;
        controllerState = GamePad.GetState(0);

        if (Input.anyKey && currentState == MenuState.TitleScreen)
        {
            GoToNextState();
        }

        if (controllerState.ThumbSticks.Left.X > 0.25f && prevControllerState.ThumbSticks.Left.X < 0.25f || controllerState.ThumbSticks.Left.Y > 0.25f && prevControllerState.ThumbSticks.Left.Y < 0.25f)
        {
            currentCursor++;
            // Update visual feedback
            if (currentState == MenuState.NumberOfPlayers && selectedMode == 1)
            {
                currentCursor = currentCursor % 4;
                transform.GetChild((int)currentState).GetChild(3).GetChild(currentCursor).GetComponent<Button>().Select();
            }
            else
            {
                currentCursor = currentCursor % 2;
                transform.GetChild((int)currentState).GetChild(2).GetChild(currentCursor).GetComponent<Button>().Select();
            }
        }
        //else if (controllerState.ThumbSticks.Left.X < -0.25f && prevControllerState.ThumbSticks.Left.X > -0.25f 
        //    || controllerState.ThumbSticks.Left.Y < -0.25f && prevControllerState.ThumbSticks.Left.Y > -0.25f)
        //{
        //    currentCursor--;
        //    // Update visual feedback
        //    transform.GetChild((int)MenuState.ModeSelection).GetChild(2 + currentCursor).GetComponent<Button>().Select();
        //}

        if (prevControllerState.Buttons.A == ButtonState.Released && prevControllerState.Buttons.A == ButtonState.Pressed)
        {
            //GoToNextState();
            transform.GetChild((int)MenuState.ModeSelection).GetChild(2 + currentCursor).GetComponent<Button>().onClick.Invoke();
        }
        else if (prevControllerState.Buttons.B == ButtonState.Released && prevControllerState.Buttons.B == ButtonState.Pressed)
        {
            ReturnToPreviousState();
        }
        Debug.Log(currentCursor);
    }

    public void SetState(MenuState _newState)
    {
        currentCursor = 0;
        transform.GetChild((int)currentState).gameObject.SetActive(false);
        currentState = _newState;
        transform.GetChild((int)currentState).gameObject.SetActive(true);
    }

    void GoToNextState()
    {
        // Go to next state if not story + customisation or minigames and minigame selection
        SetState((MenuState)((int)currentState + 1));
    }

    void ReturnToPreviousState()
    {
        SetState((MenuState)((int)currentState - 1));

    }
}
