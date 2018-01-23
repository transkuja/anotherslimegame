using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UWPAndXInput;
using System.Collections.Generic;

public class Menu : MonoBehaviour {
    public enum MenuState { Common, TitleScreen, ModeSelection, NumberOfPlayers, CustomisationScreen, MinigameSelection }
    MenuState currentState = MenuState.TitleScreen;

    int currentCursor = 0;

    int selectedMode = -1;
    int nbPlayers = -1;

    Button currentlySelectedButton;
    bool buttonNeedUpdate = false;

    public GameObject playerCustomScreenPrefab;

    List<GameObject> playerCustomScreens = new List<GameObject>();

    private List<DatabaseClass.ColorData> unlockedCustomColors = new List<DatabaseClass.ColorData>();
    private List<DatabaseClass.FaceData> unlockedFaces = new List<DatabaseClass.FaceData>();

    GamePadState[] prevControllerStates = new GamePadState[4];
    GamePadState[] controllerStates = new GamePadState[4];

    int[] selectedColors = new int[4];
    int[] selectedFaces = new int[4];
    int[] currentCursorsRow = new int[4];
    bool[] selectedColorFades = new bool[4];

    [SerializeField]
    SlimeDataContainer dataContainer;

    bool[] areReady;

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
        foreach (DatabaseClass.ColorData c in dataContainer.databaseReference.colors)
        {
            if (c.isUnlocked)
                unlockedCustomColors.Add(c);
        }

        foreach (DatabaseClass.FaceData f in dataContainer.databaseReference.faces)
        {
            if (f.isUnlocked)
                unlockedFaces.Add(f);
        }

        SetState(MenuState.TitleScreen);
    }

    void TitleScreenInputHandling()
    {
        if (Input.anyKey)
        {
            GoToNextState();
            return;
        }
    }

    void Update()
    {
        // Title screen inputs
        if (currentState == MenuState.TitleScreen)
        {
            TitleScreenInputHandling();
            return;
        }


        // Save all players input
        for (int i = 0; i < 4; i++)
        {
            prevControllerStates[i] = controllerStates[i];
            controllerStates[i] = GamePad.GetState((PlayerIndex)i);
        }

        // Player 1 has the lead and can rewind the menu state by pressing B
        if (prevControllerStates[0].Buttons.B == ButtonState.Released && controllerStates[0].Buttons.B == ButtonState.Pressed)
        {
            // For CustomisationScreen, we want to be sure that Player 1 is not in "ready" state so we handle rewind elsewhere
            if (currentState != MenuState.TitleScreen && currentState != MenuState.CustomisationScreen)
            {
                ReturnToPreviousState();
                return;
            }
        }

        // Standard states input
        if (currentState != MenuState.CustomisationScreen)
        {
            if ((controllerStates[0].ThumbSticks.Left.X > 0.5f && prevControllerStates[0].ThumbSticks.Left.X < 0.5f)
                    || (controllerStates[0].ThumbSticks.Left.Y < -0.75f && prevControllerStates[0].ThumbSticks.Left.Y > -0.75f))
            {
                buttonNeedUpdate = true;
                currentCursor++;
            }
            else if ((controllerStates[0].ThumbSticks.Left.X < -0.5f && prevControllerStates[0].ThumbSticks.Left.X > -0.5f)
                    || (controllerStates[0].ThumbSticks.Left.Y > 0.75f && prevControllerStates[0].ThumbSticks.Left.Y < 0.75f))
            {
                buttonNeedUpdate = true;
                currentCursor--;
            }
        

            // Update visual feedback
            if (buttonNeedUpdate)
            {
                if (currentState == MenuState.NumberOfPlayers && selectedMode == 1)
                {
                    UpdateSelectionVisual(4, 1);                   
                }
                else if (currentState == MenuState.MinigameSelection)
                {
                  //  UpdateSelectionVisual()
                }
                else
                {
                    UpdateSelectionVisual(2, 0);
                }
                buttonNeedUpdate = false;
            }

            if (prevControllerStates[0].Buttons.A == ButtonState.Released && controllerStates[0].Buttons.A == ButtonState.Pressed)
            {
                if (currentlySelectedButton != null)
                {
                    currentlySelectedButton.onClick.Invoke();
                    GoToNextState();
                }
            }
        }
        // Customisation screen inputs & behaviours
        else
        {
            // if nb of players is not specified here, there must be a bug in state transition
            if (nbPlayers == -1)
                return;

            // Check all players input
            for (int i = 0; i < nbPlayers; i++)
            {
                // Unready player 
                if (prevControllerStates[i].Buttons.B == ButtonState.Released && controllerStates[i].Buttons.B == ButtonState.Pressed)
                {
                    // Go back to previous state if player 1 is not ready and pressed B
                    if (i == 0 && !areReady[0])
                    {
                        ReturnToPreviousState();
                        return;
                    }
                        
                    areReady[i] = false;

                    // Reactivate position feedback and reset cursor
                    currentCursorsRow[i] = 0;
                    playerCustomScreens[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

                    // Disable "Ready!" txt
                    playerCustomScreens[i].transform.GetChild(4).gameObject.SetActive(false);
                }

                // If the player i is ready, block all inputs
                if (areReady[i])
                    continue;

                // Press start when you're ready to go
                if (prevControllerStates[i].Buttons.Start == ButtonState.Released && controllerStates[i].Buttons.Start == ButtonState.Pressed)
                {
                    areReady[i] = true;
                    // Deactivate feedbacks
                    playerCustomScreens[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    playerCustomScreens[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    // Pop "Ready!" txt
                    playerCustomScreens[i].transform.GetChild(4).gameObject.SetActive(true);

                    // If everyone is ready, launch the game or the mini game selection screen
                    if (IsEveryoneReady())
                    {
                        GoToNextStateFromCustomisationScreen();
                        return;
                    }
                }

                // Y axis controls the settings selection
                if (controllerStates[i].ThumbSticks.Left.Y > 0.5f && prevControllerStates[i].ThumbSticks.Left.Y < 0.5f
                    || (controllerStates[i].ThumbSticks.Left.Y < -0.5f && prevControllerStates[i].ThumbSticks.Left.Y > -0.5f))
                {
                    currentCursorsRow[i]++;
                    currentCursorsRow[i] = currentCursorsRow[i] % 2;
                    playerCustomScreens[i].transform.GetChild(0).GetChild(currentCursorsRow[i]).gameObject.SetActive(true);
                    playerCustomScreens[i].transform.GetChild(0).GetChild((currentCursorsRow[i] + 1)%2).gameObject.SetActive(false);
                }
                // X axis controls the settings values
                else if (controllerStates[i].ThumbSticks.Left.X > 0.5f && prevControllerStates[i].ThumbSticks.Left.X < 0.5f)
                {
                    if (currentCursorsRow[i] == 0)
                    {
                        selectedColors[i]++;
                        UpdatePlayerPreviewColor(i);
                    }
                    else
                    {
                        selectedFaces[i]++;
                        UpdatePlayerPreviewFace(i);
                    }
                }
                else if (controllerStates[i].ThumbSticks.Left.X < -0.5f && prevControllerStates[i].ThumbSticks.Left.X > -0.5f)
                {
                    if (currentCursorsRow[i] == 0)
                    {
                        selectedColors[i]--;
                        UpdatePlayerPreviewColor(i);
                    }
                    else
                    {
                        selectedFaces[i]--;
                        UpdatePlayerPreviewFace(i);
                    }
                }
            }
        }
    }

    // Move the button cursor and highlight it
    void UpdateSelectionVisual(int _nbButtons, int _childOffset)
    {
        if (currentCursor < 0)
            currentCursor = _nbButtons - 1;
        else
            currentCursor = currentCursor % _nbButtons;
        currentlySelectedButton = transform.GetChild((int)currentState).GetChild(_childOffset).GetChild(currentCursor).GetComponent<Button>();
        currentlySelectedButton.Select();
    }

    // Change the player color according to current selection
    void UpdatePlayerPreviewColor(int _playerIndex)
    {
        if (selectedColors[_playerIndex] < 0)
            selectedColors[_playerIndex] = unlockedCustomColors.Count;
        else
            selectedColors[_playerIndex] = selectedColors[_playerIndex] % (unlockedCustomColors.Count + 1);
        // Update text and character
        if (selectedColors[_playerIndex] == unlockedCustomColors.Count)
        {
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().UseColorFade = true;
            playerCustomScreens[_playerIndex].transform.GetChild(1).GetComponent<Text>().text = "Rainbow";
        }
        else
        {
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().UseColorFade = false;
            playerCustomScreens[_playerIndex].transform.GetChild(1).GetComponent<Text>().text = unlockedCustomColors[selectedColors[_playerIndex]].Id;
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(unlockedCustomColors[selectedColors[_playerIndex]].color);
        }
    }

    // Change the player face according to current selection
    void UpdatePlayerPreviewFace(int _playerIndex)
    {
        if (selectedFaces[_playerIndex] < 0)
            selectedFaces[_playerIndex] = unlockedFaces.Count - 1;
        else
            selectedFaces[_playerIndex] = selectedFaces[_playerIndex] % unlockedFaces.Count;

        // Update text and character
        playerCustomScreens[_playerIndex].transform.GetChild(2).GetComponent<Text>().text = unlockedFaces[selectedFaces[_playerIndex]].Id;
        playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)unlockedFaces[selectedFaces[_playerIndex]].indiceForShader;
    }


    public void SetState(MenuState _newState)
    {
        currentCursor = 0;
        transform.GetChild((int)currentState).gameObject.SetActive(false);
        currentState = _newState;
        transform.GetChild((int)currentState).gameObject.SetActive(true);

        // Mode selection step reset
        if (currentState == MenuState.ModeSelection)
        {
            currentlySelectedButton = transform.GetChild((int)currentState).GetChild(0).GetChild(currentCursor).GetComponent<Button>();
            currentlySelectedButton.Select();
            selectedMode = -1;
        }

        // Nb of players selection step reset
        if (currentState == MenuState.NumberOfPlayers)
        {
            currentlySelectedButton = transform.GetChild((int)currentState).GetChild(selectedMode).GetChild(currentCursor).GetComponent<Button>();
            currentlySelectedButton.Select();
            nbPlayers = -1;
        }

        // Customisation screen reset
        if (currentState == MenuState.CustomisationScreen)
        {
            areReady = new bool[nbPlayers];

            if (playerCustomScreens.Count > 0)
            {
                foreach (GameObject go in playerCustomScreens)
                    Destroy(go);
            }

            playerCustomScreens.Clear();
            for (int i = 0; i < nbPlayers; i++)
            {
                GameObject go = Instantiate(playerCustomScreenPrefab, transform.GetChild((int)MenuState.CustomisationScreen));
                go.GetComponentInChildren<Text>().text = "Player " + (i + 1);

                if (nbPlayers == 1)
                    go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                if (nbPlayers == 2)
                    go.transform.localPosition = new Vector3(-(250) + (2 * i) * (250), 0.0f, 0.0f);
                if (nbPlayers == 3)
                    go.transform.localPosition = new Vector3((-(250) + i * (250)), 0.0f, 0.0f);
                if (nbPlayers == 4)
                    go.transform.localPosition = new Vector3(-(300) + (i * (200)), 0.0f, 0.0f);

                go.transform.GetChild(1).GetComponent<Text>().text = unlockedCustomColors[0].Id;
                go.transform.GetChild(2).GetComponent<Text>().text = unlockedFaces[0].Id;
                go.transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(unlockedCustomColors[0].color);
                go.transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().FaceType = 0;

                playerCustomScreens.Add(go);
            }
        }

    }

    void GoToNextState()
    {
        // Go to next state if not story + customisation or minigames and minigame selection
        if ((selectedMode == 0 && currentState == MenuState.CustomisationScreen)
            || (selectedMode == 1 && currentState == MenuState.MinigameSelection))
            return;
        SetState((MenuState)((int)currentState + 1));
    }

    void ReturnToPreviousState()
    {
        if (currentState == MenuState.ModeSelection || currentState == MenuState.TitleScreen)
            return;
        SetState((MenuState)((int)currentState - 1));
    }

    bool IsEveryoneReady()
    {
        for (int i = 0; i < areReady.Length; i++)
        {
            if (!areReady[i])
                return false;
        }
        return true;
    }

    void GoToNextStateFromCustomisationScreen()
    {
        // Launch HUB
        if (selectedMode == 0)
        {
            SendDataToContainer();
            SceneManager.LoadScene(1);
            return;
        }
        // Go to minigame selection screen
        else
        {
            GoToNextState();
            return;
        }
    }

    void SendDataToContainer()
    {
        // Send data to data container
        Color[] sc = new Color[nbPlayers];
        for (int i = 0; i < nbPlayers; i++)
        {
            if (selectedColors[i] == unlockedCustomColors.Count)
                selectedColorFades[i] = true;
            else
            {
                selectedColorFades[i] = false; // Line needed in case we come back from minigame selection screen
                sc[i] = unlockedCustomColors[selectedColors[i]].color;
            }
        }
        dataContainer.SaveData(nbPlayers, sc, selectedFaces, selectedColorFades);
    }
}
