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

    Button currentlySelectedButton;
    bool buttonNeedUpdate = false;

    public GameObject playerCustomScreenPrefab;

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


        if ((controllerState.ThumbSticks.Left.X > 0.5f && prevControllerState.ThumbSticks.Left.X < 0.5f)
                || (controllerState.ThumbSticks.Left.Y < -0.75f && prevControllerState.ThumbSticks.Left.Y > -0.75f))
        {
            buttonNeedUpdate = true;
            currentCursor++;
        }
        else if ((controllerState.ThumbSticks.Left.X < -0.5f && prevControllerState.ThumbSticks.Left.X > -0.5f)
                || (controllerState.ThumbSticks.Left.Y > 0.75f && prevControllerState.ThumbSticks.Left.Y < 0.75f))
        {
            buttonNeedUpdate = true;
            currentCursor--;
        }

        // Update visual feedback
        if (buttonNeedUpdate)
        {
            if (currentState == MenuState.NumberOfPlayers && selectedMode == 1)
            {
                if (currentCursor < 0)
                    currentCursor = 3;
                else
                    currentCursor = currentCursor % 4;
                currentlySelectedButton = transform.GetChild((int)currentState).GetChild(3).GetChild(currentCursor).GetComponent<Button>();
                currentlySelectedButton.Select();
            }
            else
            {
                if (currentCursor < 0)
                    currentCursor = 1;
                else
                    currentCursor = currentCursor % 2;
                currentlySelectedButton = transform.GetChild((int)currentState).GetChild(2).GetChild(currentCursor).GetComponent<Button>();
                currentlySelectedButton.Select();
            }
            buttonNeedUpdate = false;
        }

        if (prevControllerState.Buttons.A == ButtonState.Released && controllerState.Buttons.A == ButtonState.Pressed)
        {
            if (currentlySelectedButton != null)
            {
                currentlySelectedButton.onClick.Invoke();
                GoToNextState();
            }
        }

        else if (prevControllerState.Buttons.B == ButtonState.Released && controllerState.Buttons.B == ButtonState.Pressed)
        {
            if (currentState != MenuState.TitleScreen)
                ReturnToPreviousState();
        }
    }

    public void SetState(MenuState _newState)
    {
        currentCursor = 0;
        transform.GetChild((int)currentState).gameObject.SetActive(false);
        currentState = _newState;
        transform.GetChild((int)currentState).gameObject.SetActive(true);
        if (currentState == MenuState.ModeSelection) selectedMode = -1;
        if (currentState == MenuState.NumberOfPlayers) nbPlayers = -1;

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
