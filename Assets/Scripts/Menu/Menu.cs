using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UWPAndXInput;
using System.Collections.Generic;
using System.Collections;
using System;

public enum CustomizableType { Color, Face, Ears, Mustache, Hat, Skin, Accessory, Chin, Forehead, Size }

public class Menu : MonoBehaviour {
    public enum MenuState { Common, TitleScreen, ConfirmationScreen, ModeSelection, NumberOfPlayers, CustomisationScreen, MinigameSelection }
    MenuState currentState = MenuState.TitleScreen;

    // CustomizableType
    bool[] isNonable = { false, false, true, true, true, true, true, true, true };

    int currentCursor = 0;

    int minigameCurrentCursor = 0;
    int minigameCurrentVerticalCursor = 0;

    // -1 None, 0 Story/Hub, 1 minigame selection
    int selectedMode = -1;
    int nbPlayers = -1;

    Button currentlySelectedButton;
    bool buttonNeedUpdate = false;

    public GameObject playerCustomScreenPrefab;
    public GameObject minigameScreenButtonPrefab;

    List<GameObject> playerCustomScreens = new List<GameObject>();

    private List<DatabaseClass.MinigameData[]> unlockedMinigameFirstVariante = new List<DatabaseClass.MinigameData[]>(); 

    // TODO: add in database missing values Ears, Hands, Tail
    private Dictionary<CustomizableType, List<DatabaseClass.Unlockable>> customizables = new Dictionary<CustomizableType, List<DatabaseClass.Unlockable>>();

    GamePadState[] prevControllerStates = new GamePadState[4];
    GamePadState[] controllerStates = new GamePadState[4];

    // Used to know which option is currently selected
    int[] currentlySelectedOption = new int[4];

    // Used for visual and UI "value" update 
    int[,] selectedCustomizables = new int[(int)CustomizableType.Size, 4];

    // LEGACY
    int[] currentCursorsRow = new int[4];
    bool[] selectedColorFades = new bool[4];
    bool[] selectedRabbits = new bool[4];

    [SerializeField]
    SlimeDataContainer dataContainer;

    bool[] areReady;

    [SerializeField]
    GameObject menuCursor;
    GameObject currentCursorVisual;

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
            currentlySelectedButton = value;
            if (currentlySelectedButton != null)
            {
                if (currentCursorVisual == null)
                {
                    currentCursorVisual = Instantiate(menuCursor, null);
                }

                currentCursorVisual.transform.SetParent(currentlySelectedButton.transform);
                currentCursorVisual.transform.localPosition = Vector3.zero;
                currentCursorVisual.transform.localScale = Vector3.one;
                currentCursorVisual.transform.localRotation = Quaternion.identity;
                Rect textBox = currentlySelectedButton.GetComponentInChildren<Text>().GetComponent<RectTransform>().rect;

                currentCursorVisual.transform.GetChild(0).localPosition = new Vector3(textBox.xMin, -1.0f, 0.0f);
                currentCursorVisual.transform.GetChild(1).localPosition = new Vector3(textBox.xMax, -1.0f, 0.0f);
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
        // Deactivate debug tools in menu
        if (ResourceUtils.Instance != null && ResourceUtils.Instance.debugTools != null)
            ResourceUtils.Instance.debugTools.ActivateDebugMode();

        if (AudioManager.Instance != null)
            AudioManager.Instance.Fade(AudioManager.Instance.musicMenu);

        for (int i = 0; i < (int)CustomizableType.Size; i++)
            customizables.Add((CustomizableType)i, new List<DatabaseClass.Unlockable>());

        foreach (DatabaseClass.ColorData c in DatabaseManager.Db.colors)
        {
            customizables[CustomizableType.Color].Add(c);
        }

        foreach (DatabaseClass.FaceData f in DatabaseManager.Db.faces)
        {
            customizables[CustomizableType.Face].Add(f);
        }

        foreach (DatabaseClass.EarsData ears in DatabaseManager.Db.ears)
        {
            customizables[CustomizableType.Ears].Add(ears);
        }

        foreach (DatabaseClass.MustacheData mustache in DatabaseManager.Db.mustaches)
        {
            customizables[CustomizableType.Mustache].Add(mustache);
        }

        foreach (DatabaseClass.HatData hat in DatabaseManager.Db.hats)
        {
            customizables[CustomizableType.Hat].Add(hat);
        }

        int nbDifferentMinigameType = DatabaseManager.Db.GetNbUnlockedMinigamesOfEachType();
        for (int i = 0; i < nbDifferentMinigameType; i++)
        {
            if (DatabaseManager.Db.GetUnlockedMinigamesOfType((MinigameType)i) != null)
            {
                int nbMinigameOfType = DatabaseManager.Db.GetUnlockedMinigamesOfType((MinigameType)i).Count;
                if( nbMinigameOfType > 0)
                {
                    DatabaseClass.MinigameData[] c = new DatabaseClass.MinigameData[nbMinigameOfType];

                    for (int j = 0; j < nbMinigameOfType; j++)
                    {
                        c[j] = DatabaseManager.Db.GetUnlockedMinigamesOfType((MinigameType)i)[j];
                    }
                    unlockedMinigameFirstVariante.Add(c);
                }
    
            }
        }

        // Load data from container if players come from a minigame. Menu initialized on minigame selection screen.
        if (DataContainer.launchedFromMinigameScreen)
        {
            nbPlayers = dataContainer.nbPlayers;
            selectedMode = (dataContainer.launchedFromMinigameScreen) ? 1 : 0;

            // TODO: load data
            selectedCustomizables = new int[(int)CustomizableType.Size, 4];

            for (int i = 0; i < nbPlayers; i++)
            {
                for (int j = 0; j < customizables[CustomizableType.Color].Count; j++)
                {
                    if (dataContainer.selectedColors[i] == ((DatabaseClass.ColorData)customizables[CustomizableType.Color][j]).color)
                    {
                        selectedCustomizables[(int)CustomizableType.Color, i] = j;
                        break;
                    }
                }

                selectedCustomizables[(int)CustomizableType.Face, i] = dataContainer.selectedFaces[i];

                if (isNonable[(int)CustomizableType.Mustache] && dataContainer.mustachesSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Mustache, i] = customizables[CustomizableType.Mustache].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Mustache, i] = customizables[CustomizableType.Mustache].FindIndex(x => ((DatabaseClass.MustacheData)x).Id == dataContainer.mustachesSelected[i]);

                if (isNonable[(int)CustomizableType.Hat] && dataContainer.hatsSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Hat, i] = customizables[CustomizableType.Hat].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Hat, i] = customizables[CustomizableType.Hat].FindIndex(x => ((DatabaseClass.HatData)x).Id == dataContainer.hatsSelected[i]);

                if (isNonable[(int)CustomizableType.Ears] && dataContainer.hatsSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Ears, i] = customizables[CustomizableType.Ears].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Ears, i] = customizables[CustomizableType.Ears].FindIndex(x => ((DatabaseClass.HatData)x).Id == dataContainer.earsSelected[i]);

            }
            SetState(MenuState.MinigameSelection);
        }
        // Default behaviour. Start on title screen.
        else
        {
            for (int i = 0; i < (int)CustomizableType.Size; i++)
            {
                if (isNonable[i])
                {
                    for (int j = 0; j < 4; j++)
                    {
                        selectedCustomizables[i, j] = customizables[(CustomizableType)i].Count;
                    }
                }
            }
            
            SetState(MenuState.TitleScreen);
        }
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
        if (prevControllerStates[0].Buttons.B == ButtonState.Released && controllerStates[0].Buttons.B == ButtonState.Pressed
                // Keyboard input
                || Input.GetKeyDown(KeyCode.Backspace)
                || Input.GetKeyDown(KeyCode.Escape))
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
                    // Do nothing
                }
                else if (currentState == MenuState.TitleScreen)
                {
                    UpdateSelectionVisual(3, 0);
                }
                else if (currentState == MenuState.ConfirmationScreen)
                {
                    UpdateSelectionVisual(2, 0);
                }
                else if (currentState == MenuState.ModeSelection)
                {
                    UpdateSelectionVisual(2, 0);
                }
                else
                {
                    UpdateSelectionVisual(2, 0);
                }
                buttonNeedUpdate = false;
            }

            if (prevControllerStates[0].Buttons.A == ButtonState.Released && controllerStates[0].Buttons.A == ButtonState.Pressed
                // Keyboard input
                || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
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
                if (prevControllerStates[i].Buttons.B == ButtonState.Released && controllerStates[i].Buttons.B == ButtonState.Pressed
                    // Keyboard input
                    || (i == 1 && Input.GetKeyDown(KeyCode.KeypadPeriod))
                    || (i == 0 && Input.GetKeyDown(KeyCode.Escape)))
                {
                    // Go back to previous state if player 1 is not ready and pressed B
                    if (i == 0 && !areReady[0])
                    {
                        ReturnToPreviousState();
                        return;
                    }
                        
                    areReady[i] = false;

                    // Disable "Ready!" txt
                    playerCustomScreens[i].transform.GetChild(5).gameObject.SetActive(false);
                }

                // If the player i is ready, block all inputs
                if (areReady[i])
                    continue;

                // Press start when you're ready to go
                if (prevControllerStates[i].Buttons.Start == ButtonState.Released && controllerStates[i].Buttons.Start == ButtonState.Pressed
                    || (prevControllerStates[i].Buttons.A == ButtonState.Released && controllerStates[i].Buttons.A == ButtonState.Pressed)
                    // Keyboard input
                    || (i == 0 && Input.GetKeyDown(KeyCode.A))
                    || (i == 1 && Input.GetKeyDown(KeyCode.KeypadEnter)))
                {
                    areReady[i] = true;

                    // Pop "Ready!" txt
                    playerCustomScreens[i].transform.GetChild(5).gameObject.SetActive(true);

                    // If everyone is ready, launch the game or the mini game selection screen
                    if (IsEveryoneReady())
                    {
                        GoToNextStateFromCustomisationScreen();
                        return;
                    }
                }

                // Y axis controls the settings selection
                if (controllerStates[i].ThumbSticks.Left.Y < -0.5f && prevControllerStates[i].ThumbSticks.Left.Y > -0.5f
                    // Keyboard input
                    || (i == 0 && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.S)))
                    || (i == 1 && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
                    )
                {
                    currentlySelectedOption[i]++;
                    currentlySelectedOption[i] = currentlySelectedOption[i] % (int)CustomizableType.Size;
                    UpdatePreview(i);
                }
                else if (controllerStates[i].ThumbSticks.Left.Y > 0.5f && prevControllerStates[i].ThumbSticks.Left.Y < 0.5f)  
                {
                    currentlySelectedOption[i]--;
                    if (currentlySelectedOption[i] < 0)
                        currentlySelectedOption[i] = (int)CustomizableType.Size - 1;
                    else
                        currentlySelectedOption[i] = currentlySelectedOption[i] % (int)CustomizableType.Size;
                    UpdatePreview(i);
                }
                // X axis controls the settings values
                else if (controllerStates[i].ThumbSticks.Left.X > 0.5f && prevControllerStates[i].ThumbSticks.Left.X < 0.5f
                    // Keyboard input
                    || (i == 0 && Input.GetKeyDown(KeyCode.D))
                    || (i == 1 && Input.GetKeyDown(KeyCode.RightArrow))
                    )
                {
                    selectedCustomizables[currentlySelectedOption[i], i]++;
                    UpdatePreview(i);
                }
                else if (controllerStates[i].ThumbSticks.Left.X < -0.5f && prevControllerStates[i].ThumbSticks.Left.X > -0.5f
                    // Keyboard input
                    || (i == 0 && Input.GetKeyDown(KeyCode.Q))
                    || (i == 1 && Input.GetKeyDown(KeyCode.LeftArrow)))
                {
                    selectedCustomizables[currentlySelectedOption[i], i]--;
                    UpdatePreview(i);
                }

                playerCustomScreens[i].transform.GetChild(4).Rotate(Vector3.up, controllerStates[i].ThumbSticks.Right.X * 2.5f);
            }
        }
    }

    private void DefaultCursorControls()
    {
        if ((controllerStates[0].ThumbSticks.Left.X > 0.5f && prevControllerStates[0].ThumbSticks.Left.X < 0.5f)
            || (controllerStates[0].ThumbSticks.Left.Y < -0.75f && prevControllerStates[0].ThumbSticks.Left.Y > -0.75f)
            // Keyboard input
            || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)
            )
        {
            buttonNeedUpdate = true;
            currentCursor++;
        }
        else if ((controllerStates[0].ThumbSticks.Left.X < -0.5f && prevControllerStates[0].ThumbSticks.Left.X > -0.5f)
            || (controllerStates[0].ThumbSticks.Left.Y > 0.75f && prevControllerStates[0].ThumbSticks.Left.Y < 0.75f)
            // Keyboard input
            || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)
            )
        {
            buttonNeedUpdate = true;
            currentCursor--;
        }
    }

    private void MinigameSelectionCursorControls()
    {
        if ((controllerStates[0].ThumbSticks.Left.X > 0.5f && prevControllerStates[0].ThumbSticks.Left.X < 0.5f)
            // Keyboard input
            || (Input.GetKeyDown(KeyCode.D) || (Input.GetKeyDown(KeyCode.RightArrow)))
            )
        {
            minigameCurrentCursor++;
            minigameCurrentCursor %= unlockedMinigameFirstVariante.Count;

            // =======================================
            minigameCurrentVerticalCursor = 0;

            UpdateMinigameSelection();
        }
        else if ((controllerStates[0].ThumbSticks.Left.X < -0.5f && prevControllerStates[0].ThumbSticks.Left.X > -0.5f)
            // Keyboard input
            || (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.LeftArrow)))
            )
        {
            minigameCurrentCursor--;

            // =======================================
            minigameCurrentVerticalCursor = 0;  

            if (minigameCurrentCursor < 0)
                minigameCurrentCursor = unlockedMinigameFirstVariante.Count - 1;
            else
                minigameCurrentCursor %= unlockedMinigameFirstVariante.Count;

            UpdateMinigameSelection();
        }

        // Remi

        if ((controllerStates[0].ThumbSticks.Left.Y > 0.5f && prevControllerStates[0].ThumbSticks.Left.Y < 0.5f)
       // Keyboard input
       || (Input.GetKeyDown(KeyCode.Z) || (Input.GetKeyDown(KeyCode.UpArrow)))
       )
        {
            minigameCurrentVerticalCursor++;
            minigameCurrentVerticalCursor %= unlockedMinigameFirstVariante[minigameCurrentCursor].Length;

            UpdateMinigameSelection();
        }
        else if ((controllerStates[0].ThumbSticks.Left.Y < -0.5f && prevControllerStates[0].ThumbSticks.Left.Y > -0.5f)
            // Keyboard input
            || (Input.GetKeyDown(KeyCode.S) || (Input.GetKeyDown(KeyCode.DownArrow)))
            )
        {
            minigameCurrentVerticalCursor--;
            if (minigameCurrentVerticalCursor < 0)
                minigameCurrentVerticalCursor = unlockedMinigameFirstVariante[minigameCurrentCursor].Length - 1;
            else
                minigameCurrentVerticalCursor %= unlockedMinigameFirstVariante[minigameCurrentCursor].Length;

            UpdateMinigameSelection();
        }
    }

    private void UpdateMinigameSelection()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

        // Currently selected
        transform.GetChild((int)MenuState.MinigameSelection)
                .GetChild(2).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigameFirstVariante[minigameCurrentCursor][minigameCurrentVerticalCursor]);

        // Previous
        int previousMinigameIndex = minigameCurrentCursor - 1;
        if (previousMinigameIndex < 0) previousMinigameIndex = unlockedMinigameFirstVariante.Count - 1;
        // =============================================
        transform.GetChild((int)MenuState.MinigameSelection)
                .GetChild(0).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigameFirstVariante[previousMinigameIndex][0]);


        // =============================================
        // Next
        transform.GetChild((int)MenuState.MinigameSelection)
                .GetChild(1).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigameFirstVariante[(minigameCurrentCursor + 1) % unlockedMinigameFirstVariante.Count][0]);
    }

    // Move the button cursor and highlight it
    void UpdateSelectionVisual(int _nbButtons, int _childOffset)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

        if (currentCursor < 0)
            currentCursor = _nbButtons - 1;
        else
            currentCursor = currentCursor % _nbButtons;
        CurrentlySelectedButton = transform.GetChild((int)currentState).GetChild(_childOffset).GetChild(currentCursor).GetComponent<Button>();
    }

    void UpdatePreview(int _playerIndex)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

        CustomizableType currentCustomType = (CustomizableType)(currentlySelectedOption[_playerIndex]);
        playerCustomScreens[_playerIndex].transform.GetChild(2).GetComponent<Text>().text = currentCustomType.ToString();
        List<DatabaseClass.Unlockable> unlockedList = customizables[currentCustomType];
        if (unlockedList.Count == 0)
        {
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = "None";
        }
        else
        {
            if (selectedCustomizables[(int)currentCustomType, _playerIndex] < 0)
                selectedCustomizables[(int)currentCustomType, _playerIndex] = (isNonable[(int)currentCustomType]) ? unlockedList.Count : unlockedList.Count - 1;
            else
                selectedCustomizables[(int)currentCustomType, _playerIndex] = selectedCustomizables[(int)currentCustomType, _playerIndex] % ((isNonable[(int)currentCustomType]) ? unlockedList.Count + 1: unlockedList.Count);

            bool noneValue = selectedCustomizables[(int)currentCustomType, _playerIndex] == unlockedList.Count;
            playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().color = Color.white;
            bool isUnlocked;
            if (noneValue)
            {
                playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = "None";
                isUnlocked = true;
            }
            else
            {
                playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = customizables[currentCustomType][selectedCustomizables[(int)currentCustomType, _playerIndex]].Id;
                isUnlocked = customizables[currentCustomType][selectedCustomizables[(int)currentCustomType, _playerIndex]].isUnlocked;
                if (!isUnlocked)
                    playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().color = Color.red;
            }
            UpdatePlayerVisual(_playerIndex, currentCustomType, selectedCustomizables[(int)currentCustomType, _playerIndex], noneValue || !isUnlocked);
        }
    }

    void UpdatePreviewFull(int _playerIndex)
    {
        for (int i = 0; i < (int)CustomizableType.Size; ++i)
        {
            CustomizableType currentCustomType = (CustomizableType)(i);
            playerCustomScreens[_playerIndex].transform.GetChild(2).GetComponent<Text>().text = currentCustomType.ToString();
            List<DatabaseClass.Unlockable> unlockedList = customizables[currentCustomType];
            if (unlockedList.Count == 0)
            {
                playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = "None";
            }
            else
            {
                if (selectedCustomizables[(int)currentCustomType, _playerIndex] < 0)
                    selectedCustomizables[(int)currentCustomType, _playerIndex] = (isNonable[(int)currentCustomType]) ? unlockedList.Count : unlockedList.Count - 1;
                else
                    selectedCustomizables[(int)currentCustomType, _playerIndex] = selectedCustomizables[(int)currentCustomType, _playerIndex] % ((isNonable[(int)currentCustomType]) ? unlockedList.Count + 1 : unlockedList.Count);

                bool noneValue = selectedCustomizables[(int)currentCustomType, _playerIndex] == unlockedList.Count;
                if (noneValue)
                    playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = "None";
                else
                    playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = customizables[currentCustomType][selectedCustomizables[(int)currentCustomType, _playerIndex]].Id;

                UpdatePlayerVisual(_playerIndex, currentCustomType, selectedCustomizables[(int)currentCustomType, _playerIndex], noneValue);
            }
        }
    }

    void UpdatePlayerVisual(int _playerIndex, CustomizableType _customizableType, int _unlockedIndex, bool _isNoneValue)
    {
        switch (_customizableType)
        {
            case CustomizableType.Color:
                UpdatePlayerPreviewColor(_playerIndex, _unlockedIndex);
                break;
            case CustomizableType.Face:
                UpdatePlayerPreviewFace(_playerIndex, _unlockedIndex);
                break;
            case CustomizableType.Ears:
                UpdatePlayerPreviewEars(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Hat:
                UpdatePlayerPreviewHat(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Mustache:
                UpdatePlayerPreviewMustache(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Accessory:
            case CustomizableType.Chin:
            case CustomizableType.Forehead:
            default:
                Debug.Log(_customizableType + " is not implemented yet.");
                break;

        }

    }

    void UpdatePlayerPreviewMustache(int _playerIndex, int _selection, bool _isNoneValue)
    {
        PlayerCosmetics playerCosmetics = playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>();

        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().GetSocket(CustomizableType.Mustache);
        if (parent.childCount > 0)
        {
            if (_isNoneValue)
                StartCoroutine(Sad(playerCosmetics));
        }
        else
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCosmetics));
        }

        if (!_isNoneValue)
        {
            playerCosmetics.Mustache = ((DatabaseClass.MustacheData)customizables[CustomizableType.Mustache][_selection]).Id;
        }
        else
        {
            playerCosmetics.Mustache = String.Empty;
        }

    }

    void UpdatePlayerPreviewHat(int _playerIndex, int _selection, bool _isNoneValue)
    {
        PlayerCosmetics playerCosmetics = playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>();

        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Hat) - 2);
        if (parent.childCount < 1)
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCosmetics));
        }

        if (!_isNoneValue)
        {
            playerCosmetics.Hat = ((DatabaseClass.HatData)customizables[CustomizableType.Hat][_selection]).Id;
        }
        else
        {
            playerCosmetics.Hat = String.Empty;
        }
    }

    // Default customizable update function
    void UpdatePlayerPreviewEars(int _playerIndex, int _selection, bool _isNoneValue)
    {
        PlayerCosmetics playerCosmetics = playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>();

        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Ears) - 2);

        if (parent.childCount < 1)
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCosmetics));
        }

        if (!_isNoneValue)
        {
            playerCosmetics.Ears = ((DatabaseClass.EarsData)customizables[CustomizableType.Ears][_selection]).Id;
        }
        else
        {
            playerCosmetics.Ears = String.Empty;
        }
    }

    IEnumerator Happy(PlayerCosmetics _cosmeticsRef)
    {
        _cosmeticsRef.FaceEmotion = FaceEmotion.Winner;
        yield return new WaitForSeconds(1);
        if (_cosmeticsRef.FaceEmotion == FaceEmotion.Winner)
            _cosmeticsRef.FaceEmotion = FaceEmotion.Neutral;
    }

    IEnumerator Sad(PlayerCosmetics _cosmeticsRef)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.shaveFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.shaveFx);

        _cosmeticsRef.FaceEmotion = FaceEmotion.Loser;
        yield return new WaitForSeconds(1);
        if (_cosmeticsRef.FaceEmotion == FaceEmotion.Loser)
            _cosmeticsRef.FaceEmotion = FaceEmotion.Neutral;
    }

    // Change the player color according to current selection
    void UpdatePlayerPreviewColor(int _playerIndex, int _selection)
    {
        // Update text and character
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().ColorFadeType = ColorFadeType.None;
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(((DatabaseClass.ColorData)customizables[CustomizableType.Color][_selection]).color);
    }

    // Change the player face according to current selection
    void UpdatePlayerPreviewFace(int _playerIndex, int _selection)
    {
        // Update text and character
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().FaceType = ((DatabaseClass.FaceData)customizables[CustomizableType.Face][_selection]).indiceForShader;
    }


    public void SetState(MenuState _newState)
    {
        currentCursor = 0;
        minigameCurrentCursor = 0;
        transform.GetChild((int)currentState).gameObject.SetActive(false);
        currentState = _newState;
        transform.GetChild((int)currentState).gameObject.SetActive(true);

        // Mode selection step reset
        if (currentState == MenuState.ModeSelection)
        {
            CurrentlySelectedButton = transform.GetChild((int)currentState).GetChild(0).GetChild(currentCursor).GetComponent<Button>();
            selectedMode = -1;
        }

        // Mode selection step reset
        if (currentState == MenuState.TitleScreen)
        {
            CurrentlySelectedButton = transform.GetChild((int)currentState).GetChild(0).GetChild(currentCursor).GetComponent<Button>();
            selectedMode = -1;
        }

        // Mode selection step reset
        if (currentState == MenuState.ConfirmationScreen)
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
            for (int i = 0; i < 4; i++)
                currentCursorsRow[i] = 0;

            // If a selection has already been made
            if (playerCustomScreens.Count > 0)
            {
                // Reset positions of existing ones, hide "Ready!"
                for (int i = 0; i < playerCustomScreens.Count; i++)
                {
                    playerCustomScreens[i].transform.GetChild(5).gameObject.SetActive(false);

                    if (nbPlayers == 1)
                        playerCustomScreens[i].transform.localPosition = new Vector3(0.0f, -65.0f, 0.0f);
                    if (nbPlayers == 2)
                        playerCustomScreens[i].transform.localPosition = new Vector3(-160 + (2 * i * 160), -65.0f, 0.0f);
                    if (nbPlayers >= 3)
                        playerCustomScreens[i].transform.localPosition = new Vector3(-(160) * Mathf.Pow(-1, i), (i < 2) ? 35.0f : -165.0f, 0.0f);

                    UpdatePreviewFull(i);
                    UpdatePreview(i); // Used to reset cursors last position
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
                    CreatePlayerCustomScreen(i);

                    if (DataContainer.launchedFromMinigameScreen)
                    {
                        UpdatePreviewFull(i);
                    }

                }
                CurrentlySelectedButton = null;
            }
        }

        // Minigame screen reset
        if (currentState == MenuState.MinigameSelection)
        {
            CurrentlySelectedButton = null;

            // Currently selected
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(2).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigameFirstVariante[0][0]);

            // Previous
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(0).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigameFirstVariante[unlockedMinigameFirstVariante.Count - 1][0]);

            // Next
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(1).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigameFirstVariante[Mathf.Min(1, unlockedMinigameFirstVariante.Count - 1)][0]);


            // Adapt players on screen
            int childCount = transform.GetChild((int)MenuState.MinigameSelection).childCount;
            for (int i = 0; i < nbPlayers; i++)
            {
                PlayerCosmetics curPlayerCosmetics = transform.GetChild((int)MenuState.MinigameSelection).GetChild(childCount - 4 + i).GetComponentInChildren<PlayerCosmetics>(true);
                curPlayerCosmetics.SetUniqueColor(((DatabaseClass.ColorData)customizables[CustomizableType.Color][selectedCustomizables[(int)CustomizableType.Color, i]]).color);
                curPlayerCosmetics.FaceType = ((DatabaseClass.FaceData)customizables[CustomizableType.Face][selectedCustomizables[(int)CustomizableType.Face, i]]).indiceForShader;

                // Customizables

                // Mustache //
                UpdatePlayersOnMinigameSelectionScreen(CustomizableType.Mustache, i, childCount);

                // Ears //
                UpdatePlayersOnMinigameSelectionScreen(CustomizableType.Ears, i, childCount);

                // Hat //
                UpdatePlayersOnMinigameSelectionScreen(CustomizableType.Hat, i, childCount);

            }

            for (int i = nbPlayers; i < 4; i++)
            {
                transform.GetChild((int)MenuState.MinigameSelection).GetChild(childCount - 4 + i).gameObject.SetActive(false);
            }
        }
    }

    void UpdatePlayersOnMinigameSelectionScreen(CustomizableType _type, int _playerIndex, int _childCount)
    {
        PlayerCosmetics playerCosmetics = transform.GetChild((int)MenuState.MinigameSelection).GetChild(_childCount - 4 + _playerIndex).GetComponentInChildren<PlayerCosmetics>(true);

        if (selectedCustomizables[(int)_type, _playerIndex] != customizables[_type].Count && customizables[_type].Count > 0)
        {
            if (_type == CustomizableType.Mustache)
                playerCosmetics.Mustache = ((DatabaseClass.MustacheData)customizables[_type][selectedCustomizables[(int)_type, _playerIndex]]).Id;
            else if (_type == CustomizableType.Hat)
            {
                playerCosmetics.Hat = ((DatabaseClass.HatData)customizables[_type][selectedCustomizables[(int)_type, _playerIndex]]).Id;
            }
            else if (_type == CustomizableType.Ears)
                playerCosmetics.Ears = ((DatabaseClass.EarsData)customizables[_type][selectedCustomizables[(int)_type, _playerIndex]]).Id;
        }
    }

    GameObject CreatePlayerCustomScreen(int _newPlayerIndex)
    {
        GameObject go = Instantiate(playerCustomScreenPrefab, transform.GetChild((int)MenuState.CustomisationScreen));
        go.GetComponentInChildren<Text>().text = "Player " + (_newPlayerIndex + 1);

        if (nbPlayers == 1)
            go.transform.localPosition = new Vector3(0.0f, -65.0f, 0.0f);
        if (nbPlayers == 2)
            go.transform.localPosition = new Vector3(-160 + (2 * _newPlayerIndex * 160), -65.0f, 0.0f);
        if (nbPlayers >= 3)
            go.transform.localPosition = new Vector3(-(160) * Mathf.Pow(-1, _newPlayerIndex), (_newPlayerIndex < 2) ? 35.0f : -165.0f , 0.0f);

        go.transform.GetChild(2).GetComponent<Text>().text = ((CustomizableType)0).ToString();
        go.transform.GetChild(3).GetComponent<Text>().text = customizables[0][0].Id;
        go.transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(((DatabaseClass.ColorData)customizables[CustomizableType.Color][0]).color);
        go.transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().FaceType = ((DatabaseClass.FaceData)customizables[CustomizableType.Face][0]).indiceForShader;
        playerCustomScreens.Add(go);

        return go;
    }

    void GoToNextState()
    {
        // Go to next state if not story + customisation or minigames and minigame selection
        if (selectedMode == 0 && currentState == MenuState.CustomisationScreen)
            return;

        // Exit game has been pressed
        if (currentState == MenuState.TitleScreen && currentCursor == 2 )
            return;

        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

        // Continue -> Go to Mode Selection
        if (currentState == MenuState.TitleScreen && currentCursor == 0)
        {
            SetState((MenuState)((int)currentState + 2));
            return;
        }
        // No -> Return to Title
        if (currentState == MenuState.ConfirmationScreen && currentCursor == 1)
        {
            SetState((MenuState)((int)currentState -1));
            return;
        }

        SetState((MenuState)((int)currentState + 1));
    }

    void ReturnToPreviousState()
    {
        if (currentState == MenuState.TitleScreen)
            return;

        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

        // In Mode selection -> Go to tile
        if (currentState == MenuState.ModeSelection)
        {
            SetState((MenuState)((int)currentState - 2));
            return;
        }

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
        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

        for (int i = 0; i < nbPlayers; i++)
        {
            if (customizables[CustomizableType.Mustache].Count != 0 && selectedCustomizables[(int)CustomizableType.Mustache, i] != customizables[CustomizableType.Mustache].Count 
                    && !customizables[CustomizableType.Mustache][selectedCustomizables[(int)CustomizableType.Mustache, i]].isUnlocked)
                selectedCustomizables[(int)CustomizableType.Mustache, i] = customizables[CustomizableType.Mustache].Count;

            if (customizables[CustomizableType.Hat].Count != 0 && selectedCustomizables[(int)CustomizableType.Hat, i] != customizables[CustomizableType.Hat].Count
                    && !customizables[CustomizableType.Hat][selectedCustomizables[(int)CustomizableType.Hat, i]].isUnlocked)
                selectedCustomizables[(int)CustomizableType.Hat, i] = customizables[CustomizableType.Hat].Count;

            if (customizables[CustomizableType.Ears].Count != 0 && selectedCustomizables[(int)CustomizableType.Ears, i] != customizables[CustomizableType.Ears].Count
                    && !customizables[CustomizableType.Ears][selectedCustomizables[(int)CustomizableType.Ears, i]].isUnlocked)
                selectedCustomizables[(int)CustomizableType.Ears, i] = customizables[CustomizableType.Ears].Count;

        }
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
        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

        MinigameSelectionAnim minigameContainer = transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(2).GetComponent<MinigameSelectionAnim>();

        SendDataToContainer(minigameContainer.GetMinigameVersion());
        SceneManager.LoadScene(minigameContainer.GetMinigameId());
    }


    void SendDataToContainer(int _minigameVersion = 0)
    {
        // Send data to data container
        Color[] sc = new Color[nbPlayers];
        int[] sf = new int[nbPlayers];
        string[] selectedMustaches = new string[nbPlayers];
        string[] selectedHats = new string[nbPlayers];
        string[] selectedEars = new string[nbPlayers];

        for (int i = 0; i < nbPlayers; i++)
        {
            //if (selectedColors[i] == unlockedCustomColors.Count)
            //    selectedColorFades[i] = true;
            //else
            {
                selectedColorFades[i] = false; // Line needed in case we come back from minigame selection screen
                sc[i] = ((DatabaseClass.ColorData)customizables[CustomizableType.Color][selectedCustomizables[(int)CustomizableType.Color, i]]).color;
            }

            //if (selectedFaces[i] == unlockedFesses.Count)
            //    selectedRabbits[i] = true;
            //else
                selectedRabbits[i] = false; // Line needed in case we come back from minigame selection screen

            sf[i] = selectedCustomizables[(int)CustomizableType.Face, i];
            if (customizables[CustomizableType.Mustache].Count == 0 || selectedCustomizables[(int)CustomizableType.Mustache, i] == customizables[CustomizableType.Mustache].Count)
                selectedMustaches[i] = "None";
            else
                selectedMustaches[i] = customizables[CustomizableType.Mustache][selectedCustomizables[(int)CustomizableType.Mustache, i]].Id;

            if (customizables[CustomizableType.Hat].Count == 0 || selectedCustomizables[(int)CustomizableType.Hat, i] == customizables[CustomizableType.Hat].Count)
                selectedHats[i] = "None";
            else
                selectedHats[i] = customizables[CustomizableType.Hat][selectedCustomizables[(int)CustomizableType.Hat, i]].Id;

            if (customizables[CustomizableType.Ears].Count == 0 || selectedCustomizables[(int)CustomizableType.Ears, i] == customizables[CustomizableType.Ears].Count)
                selectedEars[i] = "None";
            else
                selectedEars[i] = customizables[CustomizableType.Ears][selectedCustomizables[(int)CustomizableType.Ears, i]].Id; 
        }
        dataContainer.SaveData(nbPlayers, sc, sf, selectedMustaches, selectedHats, selectedEars, _minigameVersion, selectedColorFades, selectedRabbits, selectedMode == 1);
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

    public void NewGamePressed()
    {
        // Reset -> Unlock all -> cowboy / candy / sneakyprogress = 0
        DatabaseManager.Db.NewGameSettings();
    }
}
