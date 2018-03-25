using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UWPAndXInput;
using System.Collections.Generic;

public class Menu : MonoBehaviour {
    public enum MenuState { Common, TitleScreenModeSelection, NumberOfPlayers, CustomisationScreen, MinigameSelection }
    MenuState currentState = MenuState.TitleScreenModeSelection;

    int currentCursor = 0;
    int minigameCurrentCursor = 0;

    // -1 None, 0 Story/Hub, 1 minigame selection
    int selectedMode = -1;
    int nbPlayers = -1;

    Button currentlySelectedButton;
    bool buttonNeedUpdate = false;

    public GameObject playerCustomScreenPrefab;
    public GameObject minigameScreenButtonPrefab;

    List<GameObject> playerCustomScreens = new List<GameObject>();
    List<GameObject> minigameButtonsInstantiated = new List<GameObject>();

    private List<DatabaseClass.ColorData> unlockedCustomColors = new List<DatabaseClass.ColorData>();
    private List<DatabaseClass.FaceData> unlockedFaces = new List<DatabaseClass.FaceData>();
    private List<DatabaseClass.MinigameData> unlockedMinigames = new List<DatabaseClass.MinigameData>();

    GamePadState[] prevControllerStates = new GamePadState[4];
    GamePadState[] controllerStates = new GamePadState[4];

    int[] selectedColors = new int[4];
    int[] selectedFaces = new int[4];
    int[] currentCursorsRow = new int[4];
    bool[] selectedColorFades = new bool[4];
    bool[] selectedRabbits = new bool[4];

    [SerializeField]
    SlimeDataContainer dataContainer;

    bool[] areReady;

    public SlimeDataContainer DataContainer
    {
        get
        {
            if (dataContainer == null)
                dataContainer = FindObjectOfType<SlimeDataContainer>();
            return dataContainer;
        }

        set
        {
            dataContainer = value;
        }
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
        foreach (DatabaseClass.ColorData c in DatabaseManager.Db.colors)
        {
            if (c.isUnlocked)
                unlockedCustomColors.Add(c);
        }

        foreach (DatabaseClass.FaceData f in DatabaseManager.Db.faces)
        {
            if (f.isUnlocked)
                unlockedFaces.Add(f);
        }

        foreach (DatabaseClass.MinigameData f in DatabaseManager.Db.minigames)
        {
            if (f.isUnlocked)
                unlockedMinigames.Add(f);
        }

        // Load data from container if players come from a minigame. Menu initialized on minigame selection screen.
        if (DataContainer.launchedFromMinigameScreen)
        {
            nbPlayers = dataContainer.nbPlayers;
            selectedFaces = dataContainer.selectedFaces;
            selectedColorFades = dataContainer.colorFadeSelected;
            selectedRabbits = dataContainer.rabbitSelected;
            selectedMode = (dataContainer.launchedFromMinigameScreen) ? 1 : 0;

            selectedColors = new int[4];
            for (int i = 0; i < nbPlayers; i++)
            {
                selectedColors[i] = -1;
                for (int j = 0; j < unlockedCustomColors.Count; j++)
                {
                    if (dataContainer.selectedColors[i] == unlockedCustomColors[j].color)
                    {
                        selectedColors[i] = j;
                        break;
                    }
                }
                // Handle color fade
                if (selectedColors[i] == -1)
                    selectedColors[i] = unlockedCustomColors.Count;
            }
            SetState(MenuState.MinigameSelection);
        }
        // Default behaviour. Start on title screen.
        else
            SetState(MenuState.TitleScreenModeSelection);
    }

    void Update()
    {

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
            if (currentState != MenuState.TitleScreenModeSelection && currentState != MenuState.CustomisationScreen)
            {
                ReturnToPreviousState();
                return;
            }
        }

        // Standard states input
        if (currentState != MenuState.CustomisationScreen)
        {
            if (currentState == MenuState.MinigameSelection)
                MinigameSelectionCursorControls();
            else
                DefaultCursorControls();
            
            // Update visual feedback
            if (buttonNeedUpdate)
            {
                if (currentState == MenuState.NumberOfPlayers && selectedMode == 1)
                {
                    UpdateSelectionVisual(4, 1);
                }
                else if (currentState == MenuState.MinigameSelection)
                {
                    UpdateSelectionVisualForMinigame();
                }
                else if (currentState == MenuState.TitleScreenModeSelection)
                {
                    UpdateSelectionVisual(3, 0);
                }
                else
                {
                    UpdateSelectionVisual(2, 0);
                }
                buttonNeedUpdate = false;
            }

            if (prevControllerStates[0].Buttons.A == ButtonState.Released && controllerStates[0].Buttons.A == ButtonState.Pressed)
            {
                if (CurrentlySelectedButton != null)
                {
                    CurrentlySelectedButton.onClick.Invoke();
                    GoToNextState();
                }
                else
                {
                    if (selectedMode == 1 && currentState == MenuState.MinigameSelection)
                    {
                        GoToNextStateFromMinigameSelection();
                        return;
                    }
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
                        if (selectedFaces[i] != unlockedFaces.Count)
                        {
                            selectedColors[i]++;
                            UpdatePlayerPreviewColor(i);
                        }
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
                        if (selectedFaces[i] != unlockedFaces.Count)
                        {
                            selectedColors[i]--;
                            UpdatePlayerPreviewColor(i);
                        }
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

    private void DefaultCursorControls()
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
    }

    private void MinigameSelectionCursorControls()
    {
        if (controllerStates[0].ThumbSticks.Left.X > 0.5f && prevControllerStates[0].ThumbSticks.Left.X < 0.5f)
        {
            minigameCurrentCursor++;
            minigameCurrentCursor %= unlockedMinigames.Count;

            // Currently selected
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(0).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[minigameCurrentCursor]);

            // Previous
            int previousMinigameIndex = minigameCurrentCursor - 1;
            if (previousMinigameIndex < 0) previousMinigameIndex = unlockedMinigames.Count - 1;
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(1).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[previousMinigameIndex]);

            // Next
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(2).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[(minigameCurrentCursor + 1)% unlockedMinigames.Count]);

            //buttonNeedUpdate = true;
            //minigameCurrentCursor[0]++;
        }
        else if (controllerStates[0].ThumbSticks.Left.X < -0.5f && prevControllerStates[0].ThumbSticks.Left.X > -0.5f)
        {
            minigameCurrentCursor--;
            if (minigameCurrentCursor < 0)
                minigameCurrentCursor = unlockedMinigames.Count - 1;
            else
                minigameCurrentCursor %= unlockedMinigames.Count;

            // Currently selected
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(0).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[minigameCurrentCursor]);

            // Previous
            int previousMinigameIndex = minigameCurrentCursor - 1;
            if (previousMinigameIndex < 0) previousMinigameIndex = unlockedMinigames.Count - 1;
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(1).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[previousMinigameIndex]);

            // Next
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(2).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[(minigameCurrentCursor + 1) % unlockedMinigames.Count]);
        }
        //else if ((controllerStates[0].ThumbSticks.Left.Y < -0.75f && prevControllerStates[0].ThumbSticks.Left.Y > -0.75f)
        //    || (controllerStates[0].ThumbSticks.Left.Y > 0.75f && prevControllerStates[0].ThumbSticks.Left.Y < 0.75f))
        //{
        //    buttonNeedUpdate = true;
        //    minigameCurrentCursor[1]++;
        //}
    }

    // Move the button cursor and highlight it
    void UpdateSelectionVisual(int _nbButtons, int _childOffset)
    {
        if (currentCursor < 0)
            currentCursor = _nbButtons - 1;
        else
            currentCursor = currentCursor % _nbButtons;
        Debug.Log(currentCursor);
        CurrentlySelectedButton = transform.GetChild((int)currentState).GetChild(_childOffset).GetChild(currentCursor).GetComponent<Button>();
    }

    void UpdateSelectionVisualForMinigame()
    {
        //if (minigameButtonsInstantiated.Count == 4)
        //{
        //    minigameCurrentCursor[0] %= 2;
        //    minigameCurrentCursor[1] %= 2;
        //}
        //else if (minigameButtonsInstantiated.Count == 3)
        //{
        //    if (minigameCurrentCursor[0] == 1 && minigameCurrentCursor[1] == 1)
        //        minigameCurrentCursor[0] = 0;

        //    minigameCurrentCursor[0] %= 2;
        //    minigameCurrentCursor[1] %= 2;
        //}
        //if (minigameButtonsInstantiated.Count <= 2)
        //    minigameCurrentCursor[1] = 0;
        //if (minigameButtonsInstantiated.Count == 1)
        //    minigameCurrentCursor[0] = 0;

        //int childIndex = minigameCurrentCursor[0] + 2 * minigameCurrentCursor[1];
        //CurrentlySelectedButton = transform.GetChild((int)currentState).GetChild(childIndex).GetComponentInChildren<Button>();
    }

    // Change the player color according to current selection
    void UpdatePlayerPreviewColor(int _playerIndex)
    {
        if (selectedFaces[_playerIndex] == unlockedFaces.Count)
            return;

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
            selectedFaces[_playerIndex] = unlockedFaces.Count;
        else
            selectedFaces[_playerIndex] = selectedFaces[_playerIndex] % (unlockedFaces.Count + 1);

        // Update text and character
        Debug.Log(selectedFaces[_playerIndex]);
        if (selectedFaces[_playerIndex] == unlockedFaces.Count)
        {
            playerCustomScreens[_playerIndex].transform.GetChild(1).GetComponent<Text>().text = "Yellow";
            playerCustomScreens[_playerIndex].transform.GetChild(2).GetComponent<Text>().text = "Rabbit";
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(0).gameObject.SetActive(false);
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerCustomScreens[_playerIndex].transform.GetChild(2).GetComponent<Text>().text = unlockedFaces[selectedFaces[_playerIndex]].Id;
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(0).gameObject.SetActive(true);
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(1).gameObject.SetActive(false);
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)unlockedFaces[selectedFaces[_playerIndex]].indiceForShader;
        }
    }


    public void SetState(MenuState _newState)
    {
        currentCursor = 0;
        //minigameCurrentCursor = new int[2];
        minigameCurrentCursor = 0;
        transform.GetChild((int)currentState).gameObject.SetActive(false);
        currentState = _newState;
        transform.GetChild((int)currentState).gameObject.SetActive(true);

        // Mode selection step reset
        if (currentState == MenuState.TitleScreenModeSelection)
        {
            CurrentlySelectedButton = transform.GetChild((int)currentState).GetChild(0).GetChild(currentCursor).GetComponent<Button>();
            selectedMode = -1;
        }

        // Nb of players selection step reset
        if (currentState == MenuState.NumberOfPlayers)
        {
            transform.GetChild((int)currentState).GetChild(selectedMode).gameObject.SetActive(true);
            CurrentlySelectedButton = transform.GetChild((int)currentState).GetChild(selectedMode).GetChild(currentCursor).GetComponent<Button>();
            nbPlayers = -1;
        }

        // Customisation screen reset
        if (currentState == MenuState.CustomisationScreen)
        {
            areReady = new bool[nbPlayers];

            // If a selection has already been made
            if (playerCustomScreens.Count > 0)
            {
                // Reset positions of existing ones, hide "Ready!"
                for (int i = 0; i < playerCustomScreens.Count; i++)
                {
                    playerCustomScreens[i].transform.GetChild(4).gameObject.SetActive(false);

                    if (nbPlayers == 1)
                        playerCustomScreens[i].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    if (nbPlayers == 2)
                        playerCustomScreens[i].transform.localPosition = new Vector3(-(250) + (2 * i) * (250), 0.0f, 0.0f);
                    if (nbPlayers == 3)
                        playerCustomScreens[i].transform.localPosition = new Vector3((-(250) + i * (250)), 0.0f, 0.0f);
                    if (nbPlayers == 4)
                        playerCustomScreens[i].transform.localPosition = new Vector3(-(300) + (i * (200)), 0.0f, 0.0f);
                }

                // Instantiate new screen if more players are selected
                if (nbPlayers > playerCustomScreens.Count)
                {
                    for (int i = playerCustomScreens.Count; i < nbPlayers; i++)
                    {
                        CreatePlayerCustomScreen(i);
                    }
                }
                // Destroy previous screen if less players are selected
                else
                {
                    while (playerCustomScreens.Count > nbPlayers)
                    {
                        Destroy(playerCustomScreens[playerCustomScreens.Count - 1]);
                        playerCustomScreens.RemoveAt(playerCustomScreens.Count - 1);
                    }
                }

                
            }
            // Default
            else
            {

                for (int i = 0; i < nbPlayers; i++)
                {
                    GameObject go = CreatePlayerCustomScreen(i);

                    if (DataContainer.launchedFromMinigameScreen)
                    {
                        UpdatePlayerPreviewColor(i);
                        UpdatePlayerPreviewFace(i);
                    }

                }
                CurrentlySelectedButton = null;
            }

            // Feedback reset
            foreach (GameObject go in playerCustomScreens)
            {
                go.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                go.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
        }

        // Minigame screen reset
        if (currentState == MenuState.MinigameSelection)
        {
            CurrentlySelectedButton = null;
            for (int i = 0; i < 3; ++i)
            {
                transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(i).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[Mathf.Min(i, unlockedMinigames.Count - 1)]);
            }

        }
    }

    GameObject CreatePlayerCustomScreen(int _newPlayerIndex)
    {
        GameObject go = Instantiate(playerCustomScreenPrefab, transform.GetChild((int)MenuState.CustomisationScreen));
        go.GetComponentInChildren<Text>().text = "Player " + (_newPlayerIndex + 1);

        if (nbPlayers == 1)
            go.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        if (nbPlayers == 2)
            go.transform.localPosition = new Vector3(-(250) + (2 * _newPlayerIndex) * (250), 0.0f, 0.0f);
        if (nbPlayers == 3)
            go.transform.localPosition = new Vector3((-(250) + _newPlayerIndex * (250)), 0.0f, 0.0f);
        if (nbPlayers == 4)
            go.transform.localPosition = new Vector3(-(300) + (_newPlayerIndex * (200)), 0.0f, 0.0f);

        go.transform.GetChild(1).GetComponent<Text>().text = unlockedCustomColors[0].Id;
        go.transform.GetChild(2).GetComponent<Text>().text = unlockedFaces[0].Id;
        go.transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(unlockedCustomColors[0].color);
        go.transform.GetChild(3).GetComponentInChildren<PlayerCosmetics>().FaceType = 0;

        playerCustomScreens.Add(go);

        return go;
    }

    void GoToNextState()
    {
        // Go to next state if not story + customisation or minigames and minigame selection
        if (selectedMode == 0 && currentState == MenuState.CustomisationScreen)
            return;

        // Exit game has been pressed
        if (currentCursor == 2 && currentState == MenuState.TitleScreenModeSelection)
            return;
        
        SetState((MenuState)((int)currentState + 1));
    }

    void ReturnToPreviousState()
    {
        if (currentState == MenuState.TitleScreenModeSelection)
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

    void GoToNextStateFromMinigameSelection()
    {
        SendDataToContainer();
        SceneManager.LoadScene(transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(0).GetComponent<MinigameSelectionAnim>().GetMinigameId());
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

            if (selectedFaces[i] == unlockedFaces.Count)
                selectedRabbits[i] = true;
            else
                selectedRabbits[i] = false; // Line needed in case we come back from minigame selection screen
        }
        dataContainer.SaveData(nbPlayers, sc, selectedFaces, selectedColorFades, selectedRabbits, selectedMode == 1);
    }

    private void OnDestroy()
    {
        CurrentlySelectedButton = null;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting this exciting game.");
        Application.Quit();
    }
}
