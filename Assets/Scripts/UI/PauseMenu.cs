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

    [SerializeField]
    GameObject menuCursor;

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
                if (GameManager.Instance.IsInHub() || (!GameManager.Instance.IsInHub() && SlimeDataContainer.instance && SlimeDataContainer.instance.launchedFromMinigameScreen))
                {
                    exitMenu.transform.GetChild(0).gameObject.SetActive(true);
                    exitMenu.transform.GetChild(1).gameObject.SetActive(false);
                }
                else
                {
                    exitMenu.transform.GetChild(0).gameObject.SetActive(false);
                    exitMenu.transform.GetChild(1).gameObject.SetActive(true);
                }

            }

            if (value == PauseMenuState.Exit)
            {
                if (GameManager.Instance.IsInHub() || (!GameManager.Instance.IsInHub() && SlimeDataContainer.instance.launchedFromMinigameScreen))
                {
                    CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(0).GetChild(selection).GetComponentInChildren<Button>();
                }
                else
                {
                    CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(1).GetChild(selection).GetComponentInChildren<Button>();
                }
            }
            else
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

        if (currentState == PauseMenuState.Exit)
        {
            CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetComponentsInChildren<Button>()[selection];
        }
        else
        {
            CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(selection).GetComponent<Button>();
        }
    }

    public void Resume()
    {
        GameManager.ChangeState(GameState.Normal);
    }

    public void ExitToMainMenu()
    {
        Resume();
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        if (defaultMenu )
            CurrentState = PauseMenuState.Default;
        selection = 0;
        CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(selection).GetComponentInChildren<Button>();

        Cinemachine.CinemachineFreeLook curPlayerCamera = GameManager.Instance.PlayerStart.PlayersReference[GameManager.Instance.playerWhoPausedTheGame].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>();


        if (curPlayerCamera.m_XAxis.m_InvertAxis)
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(0).GetComponentInChildren<Text>().text = "X axis inverted";
        else
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(0).GetComponentInChildren<Text>().text = "X axis default";

        if (curPlayerCamera.m_YAxis.m_InvertAxis)
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(1).GetComponentInChildren<Text>().text = "Y axis inverted";
        else
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(1).GetComponentInChildren<Text>().text = "Y axis default";
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
            currentlySelectedButton = value;
            if (currentlySelectedButton != null)
            {
                menuCursor.transform.SetParent(currentlySelectedButton.transform);
                menuCursor.transform.localPosition = Vector3.zero;
                menuCursor.transform.localScale = Vector3.one;
                menuCursor.transform.localRotation = Quaternion.identity;
                Rect textBox = currentlySelectedButton.GetComponentInChildren<Text>().GetComponent<RectTransform>().rect;

                menuCursor.transform.GetChild(0).localPosition = new Vector3(textBox.xMin, -1.0f, 0.0f);
                menuCursor.transform.GetChild(1).localPosition = new Vector3(textBox.xMax, -1.0f, 0.0f);
            }
        }
    }

    public void ExitToHub()
    {
        Resume();
        SceneManager.LoadScene(1);
    }

    // Inverted by default
    public void InvertXAxis()
    {
        Cinemachine.CinemachineFreeLook curPlayerCamera = GameManager.Instance.PlayerStart.PlayersReference[GameManager.Instance.playerWhoPausedTheGame].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>();
        curPlayerCamera.m_XAxis.m_InvertAxis = !curPlayerCamera.m_XAxis.m_InvertAxis;

        if (curPlayerCamera.m_XAxis.m_InvertAxis)
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "X axis inverted";
        else
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "X axis default";
    }

    // Not inverted by default
    public void InvertYAxis()
    {
        Cinemachine.CinemachineFreeLook curPlayerCamera = GameManager.Instance.PlayerStart.PlayersReference[GameManager.Instance.playerWhoPausedTheGame].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>();
        curPlayerCamera.m_YAxis.m_InvertAxis = !curPlayerCamera.m_YAxis.m_InvertAxis;

        if (curPlayerCamera.m_YAxis.m_InvertAxis)
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "Y axis inverted";
        else
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "Y axis default";
    }

}
