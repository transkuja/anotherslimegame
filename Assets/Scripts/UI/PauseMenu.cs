using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UWPAndXInput;

public class PauseMenu : MonoBehaviour {
    enum PauseMenuChildren { Title, DefaultMenu, Settings, Exit }
    enum PauseMenuState { Default, Settings, Exit}
    PauseMenuState currentState;

    GameObject defaultMenu;
    GameObject settingsMenu;
    GameObject exitMenu;

    int selection = 0;
    Button currentlySelectedButton;
    GamePadState prevControllerState = new GamePadState();
    GamePadState controllerState = new GamePadState();

    PauseMenuState CurrentState
    {
        set
        {
            currentState = value;
            selection = 0;
            if (value == PauseMenuState.Default)
            {
                defaultMenu.SetActive(true);
                settingsMenu.SetActive(false);
                exitMenu.SetActive(false);
            }
            else if (value == PauseMenuState.Settings)
            {
                defaultMenu.SetActive(false);
                settingsMenu.SetActive(true);
                exitMenu.SetActive(false);
            }
            else if (value == PauseMenuState.Exit)
            {
                defaultMenu.SetActive(false);
                settingsMenu.SetActive(false);
                exitMenu.SetActive(true);
            }

            CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(selection).GetComponentInChildren<Button>();
        }
    }

    void Start () {
        GameManager.pauseMenuReference = this;
        defaultMenu = transform.GetChild((int)PauseMenuChildren.DefaultMenu).gameObject;
        settingsMenu = transform.GetChild((int)PauseMenuChildren.Settings).gameObject;
        exitMenu = transform.GetChild((int)PauseMenuChildren.Exit).gameObject;
        CurrentState = PauseMenuState.Default;
        gameObject.SetActive(false);
    }

    void Update () {
        prevControllerState = controllerState;
        controllerState = GamePad.GetState((PlayerIndex)GameManager.Instance.playerWhoPausedTheGame);

        DefaultCursorControls();

        if (prevControllerState.Buttons.A == ButtonState.Released && controllerState.Buttons.A == ButtonState.Pressed)
        {
            if (CurrentlySelectedButton != null)
            {
                if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);
                CurrentlySelectedButton.onClick.Invoke();
            }
        }

        // TMP: return to 1st screen if B is pressed. Works for now but we may want to just go back 1 page backward at a time.
        if (prevControllerState.Buttons.B == ButtonState.Released && controllerState.Buttons.B == ButtonState.Pressed)
        {
            if (currentState > 0)
            {
                ChangeState(0);
                if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);
            }
        }
    }

    private void DefaultCursorControls()
    {
        if ((controllerState.ThumbSticks.Left.X > 0.5f && prevControllerState.ThumbSticks.Left.X < 0.5f)
            || (controllerState.ThumbSticks.Left.Y < -0.75f && prevControllerState.ThumbSticks.Left.Y > -0.75f))
        {
            selection++;
            UpdateSelectionVisual();
        }
        else if ((controllerState.ThumbSticks.Left.X < -0.5f && prevControllerState.ThumbSticks.Left.X > -0.5f)
            || (controllerState.ThumbSticks.Left.Y > 0.75f && prevControllerState.ThumbSticks.Left.Y < 0.75f))
        {
            selection--;
            UpdateSelectionVisual();
        }
    }

    // Move the button cursor and highlight it
    void UpdateSelectionVisual()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

        int nbButtons = transform.GetChild((int)currentState + 1).childCount;
        if (selection < 0)
            selection = nbButtons - 1;
        else
            selection = selection % nbButtons;
        CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(selection).GetComponent<Button>();
    }

    public void Resume()
    {
        GameManager.ChangeState(GameState.Normal);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        selection = 0;
        CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(selection).GetComponentInChildren<Button>();
    }

    /*
     * The int value of the state in the PauseMenuState enum => DE LA BITE
     */
    public void ChangeState(int _newState)
    {
        CurrentState = (PauseMenuState)_newState;
    }

    Button CurrentlySelectedButton
    {
        get
        {
            return currentlySelectedButton;
        }

        set
        {
            if (currentlySelectedButton != null)
                currentlySelectedButton.GetComponent<AnimButton>().enabled = false;
            currentlySelectedButton = value;
            if (currentlySelectedButton != null)
            {
                if (currentlySelectedButton.GetComponent<AnimButton>() == null)
                    currentlySelectedButton.gameObject.AddComponent<AnimButton>();
                else
                    currentlySelectedButton.GetComponent<AnimButton>().enabled = true;
            }
        }
    }

}
