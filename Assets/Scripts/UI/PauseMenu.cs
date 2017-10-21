using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    enum PauseMenuChildren { Title, DefaultMenu, Settings, Exit }
    enum PauseMenuState { Default, Settings, Exit}
    PauseMenuState currentState;

    GameObject defaultMenu;
    GameObject settingsMenu;
    GameObject exitMenu;

    PauseMenuState CurrentState
    {
        set
        {
            currentState = value;
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
        // TODO: handle pause input here?
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

    /*
     * The int value of the state in the PauseMenuState enum
     */
    public void ChangeState(int _newState)
    {
        CurrentState = (PauseMenuState)_newState;
    }

}
