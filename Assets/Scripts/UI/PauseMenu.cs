﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UWPAndXInput;

public class PauseMenu : MonoBehaviour {
    enum PauseMenuChildren { Title, DefaultMenu, Settings, Exit }
    enum PauseMenuState { Default, Settings, Exit, Controls }
    PauseMenuState currentState;

    GameObject defaultMenu;
    GameObject settingsMenu;
    GameObject exitMenu;

    [SerializeField]
    GameObject controlsScreen;

    [SerializeField]
    public GameObject uiProgress;

    int selection = 0;
    Button currentlySelectedButton;
    GamePadState prevControllerState = new GamePadState();
    GamePadState controllerState = new GamePadState();

    [SerializeField]
    GameObject menuCursor;

    [SerializeField]
    Sprite controls1P;
    [SerializeField]
    Sprite controls2P;

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
                controlsScreen.SetActive(false);
                uiProgress.SetActive(true);
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
            else if (value == PauseMenuState.Controls)
            {
                defaultMenu.SetActive(false);
                settingsMenu.SetActive(false);
                exitMenu.SetActive(false);
                controlsScreen.SetActive(true);
                if (GameManager.Instance.IsInHub() && SlimeDataContainer.instance.nbPlayers == 2)
                    controlsScreen.transform.GetChild(0).GetComponent<Image>().sprite = controls2P;
                else
                    controlsScreen.transform.GetChild(0).GetComponent<Image>().sprite = controls1P;

                controlsScreen.transform.GetChild(0).gameObject.SetActive(true);
                controlsScreen.transform.GetChild(1).gameObject.SetActive(false);

                uiProgress.SetActive(false);
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

        if (Controls.HubPNJNextMsg(prevControllerState, controllerState, GameManager.Instance.playerWhoPausedTheGame))
        {
            if (currentState != PauseMenuState.Controls)
            {
                if (CurrentlySelectedButton != null)
                {
                    if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);
                    CurrentlySelectedButton.onClick.Invoke();
                }
            }
            else
            {
                if (controlsScreen.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    controlsScreen.transform.GetChild(0).gameObject.SetActive(false);
                    controlsScreen.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    ChangeState((int)PauseMenuState.Default);
                }
            }
        }

        // TMP: return to 1st screen if B is pressed. Works for now but we may want to just go back 1 page backward at a time.
        if (Controls.HubPNJPreviousMsg(prevControllerState, controllerState, GameManager.Instance.playerWhoPausedTheGame))
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
        if (Controls.MenuDefaultMoveDown(controllerState, prevControllerState, GameManager.Instance.playerWhoPausedTheGame))
        {
            selection++;
            UpdateSelectionVisual();
        }
        else if (Controls.MenuDefaultMoveUp(controllerState, prevControllerState, GameManager.Instance.playerWhoPausedTheGame))
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
        if (GameManager.Instance.IsInHub())
        {
            GameManager.UiReference.transform.GetChild(6).gameObject.SetActive(true);
        }
    }

    public void ExitToMainMenu()
    {
        DatabaseManager.instance.SaveData();
        Resume();
        GameManager.Instance.savedPositionInHub = Vector3.zero;
        LevelLoader.LoadLevelWithFadeOut("Menu");
    }

    public void ExitGame()
    {
        DatabaseManager.instance.SaveData();
        Application.Quit();
    }

    private void OnEnable()
    {
        if (defaultMenu )
            CurrentState = PauseMenuState.Default;
        selection = 0;
        CurrentlySelectedButton = transform.GetChild((int)currentState + 1).GetChild(selection).GetComponentInChildren<Button>();

        if (GameManager.Instance.IsInHub() || GameManager.Instance.CurrentGameMode is Runner3DGameMode)
        {
            if (!GameManager.Instance.PlayerStart)
                return;
            Cinemachine.CinemachineFreeLook curPlayerCamera = GameManager.Instance.PlayerStart.PlayersReference[GameManager.Instance.playerWhoPausedTheGame].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>();

            if (curPlayerCamera.m_XAxis.m_InvertAxis)
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(0).GetComponentInChildren<Text>().text = "X axis inverted";
            else
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(0).GetComponentInChildren<Text>().text = "X axis default";

            if (curPlayerCamera.m_YAxis.m_InvertAxis)
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(1).GetComponentInChildren<Text>().text = "Y axis inverted";
            else
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(1).GetComponentInChildren<Text>().text = "Y axis default";

            if (GameManager.Instance.IsInHub())
            {
                GameManager.UiReference.transform.GetChild(6).gameObject.SetActive(false);
            }
        }
        else
        {
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(0).GetComponentInChildren<Text>().text = "Not available";
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(1).GetComponentInChildren<Text>().text = "Not available";
        }
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
        LevelLoader.LoadLevelWithLoadingScreen("Hub");
    }

    // Inverted by default
    public void InvertXAxis()
    {
        if (GameManager.Instance.IsInHub() || GameManager.Instance.CurrentGameMode is Runner3DGameMode)
        {
            Cinemachine.CinemachineFreeLook curPlayerCamera = GameManager.Instance.PlayerStart.PlayersReference[GameManager.Instance.playerWhoPausedTheGame].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>();
            curPlayerCamera.m_XAxis.m_InvertAxis = !curPlayerCamera.m_XAxis.m_InvertAxis;

            if (curPlayerCamera.m_XAxis.m_InvertAxis)
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "X axis inverted";
            else
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "X axis default";
        }
        else
        {
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "Not available";
        }
    }

    // Not inverted by default
    public void InvertYAxis()
    {
        if (GameManager.Instance.IsInHub() || GameManager.Instance.CurrentGameMode is Runner3DGameMode)
        {
            Cinemachine.CinemachineFreeLook curPlayerCamera = GameManager.Instance.PlayerStart.PlayersReference[GameManager.Instance.playerWhoPausedTheGame].GetComponent<Player>().cameraReference.GetComponentInChildren<Cinemachine.CinemachineFreeLook>();
            curPlayerCamera.m_YAxis.m_InvertAxis = !curPlayerCamera.m_YAxis.m_InvertAxis;

            if (curPlayerCamera.m_YAxis.m_InvertAxis)
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "Y axis inverted";
            else
                transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "Y axis default";
        }
        else
        {
            transform.GetChild((int)PauseMenuChildren.Settings).GetChild(selection).GetComponentInChildren<Text>().text = "Not available";
        }
    }

}