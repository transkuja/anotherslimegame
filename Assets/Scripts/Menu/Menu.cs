using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UWPAndXInput;
using System.Collections.Generic;
using System.Collections;
using System;

public enum CustomizableType { Color, Face, Ears, Mustache, Hat, Hands, Tail, Size }

public class Menu : MonoBehaviour {
    public enum MenuState { Common, TitleScreenModeSelection, NumberOfPlayers, CustomisationScreen, MinigameSelection }
    MenuState currentState = MenuState.TitleScreenModeSelection;
    bool[] isNonable = { false, false, true, true, true, true, true };

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

    private List<DatabaseClass.ColorData> unlockedCustomColors = new List<DatabaseClass.ColorData>();
    private List<DatabaseClass.FaceData> unlockedFesses = new List<DatabaseClass.FaceData>();
    private List<DatabaseClass.MinigameData> unlockedMinigames = new List<DatabaseClass.MinigameData>();
    // TODO: add in database missing values Ears, Hands, Tail
    private Dictionary<CustomizableType, List<DatabaseClass.Unlockable>> unlockedCustomizables = new Dictionary<CustomizableType, List<DatabaseClass.Unlockable>>();

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
            //if (currentlySelectedButton != null)
            //    currentlySelectedButton.GetComponent<AnimButton>().enabled = false;
            currentlySelectedButton = value;
            if (currentlySelectedButton != null)
            {
                //if (currentlySelectedButton.GetComponent<AnimButton>() == null)
                //    currentlySelectedButton.gameObject.AddComponent<AnimButton>();
                //else
                //    currentlySelectedButton.GetComponent<AnimButton>().enabled = true;

                if (currentCursorVisual == null)
                {
                    currentCursorVisual = Instantiate(menuCursor, null);
                }

                currentCursorVisual.transform.SetParent(currentlySelectedButton.transform);
                currentCursorVisual.transform.localPosition = Vector3.zero;
                currentCursorVisual.transform.localScale = Vector3.one;
                currentCursorVisual.transform.localRotation = Quaternion.identity;
                int textLength = currentlySelectedButton.GetComponentInChildren<Text>().text.Length;

                currentCursorVisual.transform.GetChild(0).localPosition = new Vector3(-7 * (textLength), -1.0f, 0.0f);
                currentCursorVisual.transform.GetChild(1).localPosition = new Vector3(7 * (textLength), -1.0f, 0.0f);
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
            unlockedCustomizables.Add((CustomizableType)i, new List<DatabaseClass.Unlockable>());

        foreach (DatabaseClass.ColorData c in DatabaseManager.Db.colors)
        {
            if (c.isUnlocked)
            {
                unlockedCustomColors.Add(c);
                unlockedCustomizables[CustomizableType.Color].Add(c);
            }
        }

        foreach (DatabaseClass.FaceData f in DatabaseManager.Db.faces)
        {
            if (f.isUnlocked)
            {
                unlockedFesses.Add(f);
                unlockedCustomizables[CustomizableType.Face].Add(f);
            }
        }

        foreach (DatabaseClass.MustacheData mustache in DatabaseManager.Db.mustaches)
        {
            if (mustache.isUnlocked)
            {
                unlockedCustomizables[CustomizableType.Mustache].Add(mustache);
            }
        }

        foreach (DatabaseClass.HatData hat in DatabaseManager.Db.hats)
        {
            if (hat.isUnlocked)
            {
                unlockedCustomizables[CustomizableType.Hat].Add(hat);
            }
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
            selectedMode = (dataContainer.launchedFromMinigameScreen) ? 1 : 0;

            // TODO: load data
            selectedCustomizables = new int[(int)CustomizableType.Size, 4];

            for (int i = 0; i < nbPlayers; i++)
            {
                for (int j = 0; j < unlockedCustomizables[CustomizableType.Color].Count; j++)
                {
                    if (dataContainer.selectedColors[i] == ((DatabaseClass.ColorData)unlockedCustomizables[CustomizableType.Color][j]).color)
                    {
                        selectedCustomizables[(int)CustomizableType.Color, i] = j;
                        break;
                    }
                }

                selectedCustomizables[(int)CustomizableType.Face, i] = dataContainer.selectedFaces[i];
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
                        selectedCustomizables[i, j] = unlockedCustomizables[(CustomizableType)i].Count;
                    }
                }
            }
            
            SetState(MenuState.TitleScreenModeSelection);
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
                    // Do nothing
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

                    // Reactivate position feedback and reset cursor
                    currentCursorsRow[i] = 0;
                    playerCustomScreens[i].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);

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
                    // Deactivate feedbacks
                    playerCustomScreens[i].transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                    playerCustomScreens[i].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
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
                if (controllerStates[i].ThumbSticks.Left.Y > 0.5f && prevControllerStates[i].ThumbSticks.Left.Y < 0.5f
                    || (controllerStates[i].ThumbSticks.Left.Y < -0.5f && prevControllerStates[i].ThumbSticks.Left.Y > -0.5f)
                    // Keyboard input
                    || (i == 0 && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.S)))
                    || (i == 1 && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)))
                    )
                {
                    currentCursorsRow[i]++;
                    currentCursorsRow[i] = currentCursorsRow[i] % 2;
                    playerCustomScreens[i].transform.GetChild(1).GetChild(currentCursorsRow[i]).gameObject.SetActive(true);
                    playerCustomScreens[i].transform.GetChild(1).GetChild((currentCursorsRow[i] + 1)%2).gameObject.SetActive(false);
                }
                // X axis controls the settings values
                else if (controllerStates[i].ThumbSticks.Left.X > 0.5f && prevControllerStates[i].ThumbSticks.Left.X < 0.5f
                    // Keyboard input
                    || (i == 0 && Input.GetKeyDown(KeyCode.D))
                    || (i == 1 && Input.GetKeyDown(KeyCode.RightArrow))
                    )
                {
                    if (currentCursorsRow[i] == 0)
                    {
                        currentlySelectedOption[i]++;
                        currentlySelectedOption[i] = currentlySelectedOption[i] % (int)CustomizableType.Size;

                    }
                    else
                    {
                        selectedCustomizables[currentlySelectedOption[i], i]++;
                    }
                    UpdatePreview(i);

                }
                else if (controllerStates[i].ThumbSticks.Left.X < -0.5f && prevControllerStates[i].ThumbSticks.Left.X > -0.5f
                    // Keyboard input
                    || (i == 0 && Input.GetKeyDown(KeyCode.Q))
                    || (i == 1 && Input.GetKeyDown(KeyCode.LeftArrow)))
                {
                    if (currentCursorsRow[i] == 0)
                    {
                        currentlySelectedOption[i]--;
                        if (currentlySelectedOption[i] < 0)
                            currentlySelectedOption[i] = (int)CustomizableType.Size - 1;
                        else
                            currentlySelectedOption[i] = currentlySelectedOption[i] % (int)CustomizableType.Size;
                    }
                    else
                    {
                        selectedCustomizables[currentlySelectedOption[i], i]--;
                    }
                    UpdatePreview(i);
                }
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
            minigameCurrentCursor %= unlockedMinigames.Count;

            UpdateMinigameSelection();
        }
        else if ((controllerStates[0].ThumbSticks.Left.X < -0.5f && prevControllerStates[0].ThumbSticks.Left.X > -0.5f)
            // Keyboard input
            || (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.LeftArrow)))
            )
        {
            minigameCurrentCursor--;
            if (minigameCurrentCursor < 0)
                minigameCurrentCursor = unlockedMinigames.Count - 1;
            else
                minigameCurrentCursor %= unlockedMinigames.Count;

            UpdateMinigameSelection();
        }
    }

    private void UpdateMinigameSelection()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

        // Currently selected
        transform.GetChild((int)MenuState.MinigameSelection)
                .GetChild(2).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[minigameCurrentCursor]);

        // Previous
        int previousMinigameIndex = minigameCurrentCursor - 1;
        if (previousMinigameIndex < 0) previousMinigameIndex = unlockedMinigames.Count - 1;
        transform.GetChild((int)MenuState.MinigameSelection)
                .GetChild(0).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[previousMinigameIndex]);

        // Next
        transform.GetChild((int)MenuState.MinigameSelection)
                .GetChild(1).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[(minigameCurrentCursor + 1) % unlockedMinigames.Count]);
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
        List<DatabaseClass.Unlockable> unlockedList = unlockedCustomizables[currentCustomType];
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
            if (noneValue)
                playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = "None";
            else
                playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = unlockedCustomizables[currentCustomType][selectedCustomizables[(int)currentCustomType, _playerIndex]].Id;

            UpdatePlayerVisual(_playerIndex, currentCustomType, selectedCustomizables[(int)currentCustomType, _playerIndex], noneValue);
        }
    }

    void UpdatePreviewFull(int _playerIndex)
    {
        for (int i = 0; i < (int)CustomizableType.Size; ++i)
        {
            CustomizableType currentCustomType = (CustomizableType)(i);
            playerCustomScreens[_playerIndex].transform.GetChild(2).GetComponent<Text>().text = currentCustomType.ToString();
            List<DatabaseClass.Unlockable> unlockedList = unlockedCustomizables[currentCustomType];
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
                    playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = unlockedCustomizables[currentCustomType][selectedCustomizables[(int)currentCustomType, _playerIndex]].Id;

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
                break;
            case CustomizableType.Hands:
                break;
            case CustomizableType.Hat:
                UpdatePlayerPreviewHat(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Mustache:
                UpdatePlayerPreviewMustache(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Tail:
            default:
                Debug.Log(_customizableType + " is not implemented yet.");
                break;

        }

    }

    void UpdatePlayerPreviewMustache(int _playerIndex, int _selection, bool _isNoneValue)
    {
        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Mustache) - 2);
        if (parent.childCount > 0)
        {
            Destroy(parent.GetChild(0).gameObject);
            if (_isNoneValue)
                StartCoroutine(Sad(playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>()));
        }
        else
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>()));
        }

        if (!_isNoneValue)
        {
            Instantiate(Resources.Load(((DatabaseClass.MustacheData)unlockedCustomizables[CustomizableType.Mustache][_selection]).model), parent);
        }

    }

    void UpdatePlayerPreviewHat(int _playerIndex, int _selection, bool _isNoneValue)
    {
        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Hat) - 2);
        if (parent.childCount > 0)
        {
            Destroy(parent.GetChild(0).gameObject);
        }
        else
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>()));
        }

        if (!_isNoneValue)
        {
            Instantiate(Resources.Load(((DatabaseClass.HatData)unlockedCustomizables[CustomizableType.Hat][_selection]).model), parent);
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
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().UseColorFade = false;
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(((DatabaseClass.ColorData)unlockedCustomizables[CustomizableType.Color][_selection]).color);
    }

    // Change the player face according to current selection
    void UpdatePlayerPreviewFace(int _playerIndex, int _selection)
    {
        // Update text and character
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetChild(0).gameObject.SetActive(true);
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetChild(1).gameObject.SetActive(false);
        playerCustomScreens[_playerIndex].transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)((DatabaseClass.FaceData)unlockedCustomizables[CustomizableType.Face][_selection]).indiceForShader;
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

            // Feedback reset
            foreach (GameObject go in playerCustomScreens)
            {
                go.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                go.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            }
        }

        // Minigame screen reset
        if (currentState == MenuState.MinigameSelection)
        {
            CurrentlySelectedButton = null;

            // Currently selected
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(2).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[0]);

            // Previous
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(0).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[unlockedMinigames.Count - 1]);

            // Next
            transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(1).GetComponent<MinigameSelectionAnim>().SetMinigame(unlockedMinigames[Mathf.Min(1, unlockedMinigames.Count - 1)]);


            // Adapt players on screen
            int childCount = transform.GetChild((int)MenuState.MinigameSelection).childCount;
            for (int i = 0; i < nbPlayers; i++)
            {
                PlayerCosmetics curPlayerCosmetics = transform.GetChild((int)MenuState.MinigameSelection).GetChild(childCount - 4 + i).GetComponentInChildren<PlayerCosmetics>();
                
                curPlayerCosmetics.SetUniqueColor(((DatabaseClass.ColorData)unlockedCustomizables[CustomizableType.Color][selectedCustomizables[(int)CustomizableType.Color, i]]).color);
                curPlayerCosmetics.FaceType = (FaceType)((DatabaseClass.FaceData)unlockedCustomizables[CustomizableType.Face][selectedCustomizables[(int)CustomizableType.Face, i]]).indiceForShader;

                // Customizables

                // Mustache //
                Transform parent = transform.GetChild((int)MenuState.MinigameSelection).GetChild(childCount - 4 + i).GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Mustache) - 2);
                if (parent.childCount > 0)
                    Destroy(parent.GetChild(0).gameObject);

                if (selectedCustomizables[(int)CustomizableType.Mustache, i] != unlockedCustomizables[CustomizableType.Mustache].Count)
                {
                    Instantiate(Resources.Load(((DatabaseClass.MustacheData)unlockedCustomizables[CustomizableType.Mustache][selectedCustomizables[(int)CustomizableType.Mustache, i]]).model), parent);
                }
                // End Mustache //

                // Hat //
                parent = transform.GetChild((int)MenuState.MinigameSelection).GetChild(childCount - 4 + i).GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Hat) - 2);
                if (parent.childCount > 0)
                    Destroy(parent.GetChild(0).gameObject);

                if (selectedCustomizables[(int)CustomizableType.Hat, i] != unlockedCustomizables[CustomizableType.Hat].Count)
                {
                    Instantiate(Resources.Load(((DatabaseClass.HatData)unlockedCustomizables[CustomizableType.Hat][selectedCustomizables[(int)CustomizableType.Hat, i]]).model), parent);
                }
                // End Hat //

            }

            for (int i = nbPlayers; i < 4; i++)
            {
                transform.GetChild((int)MenuState.MinigameSelection).GetChild(childCount - 4 + i).gameObject.SetActive(false);
            }
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
        go.transform.GetChild(3).GetComponent<Text>().text = unlockedCustomizables[0][0].Id;
        go.transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(((DatabaseClass.ColorData)unlockedCustomizables[CustomizableType.Color][0]).color);
        go.transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().FaceType = (FaceType)((DatabaseClass.FaceData)unlockedCustomizables[CustomizableType.Face][0]).indiceForShader;
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

        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

        SetState((MenuState)((int)currentState + 1));
    }

    void ReturnToPreviousState()
    {
        if (currentState == MenuState.TitleScreenModeSelection)
            return;

        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

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

        SendDataToContainer();
        SceneManager.LoadScene(transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(2).GetComponent<MinigameSelectionAnim>().GetMinigameId());
    }


    void SendDataToContainer()
    {
        // Send data to data container
        Color[] sc = new Color[nbPlayers];
        int[] sf = new int[nbPlayers];
        string[] selectedMustaches = new string[nbPlayers];
        string[] selectedHats = new string[nbPlayers];

        for (int i = 0; i < nbPlayers; i++)
        {
            //if (selectedColors[i] == unlockedCustomColors.Count)
            //    selectedColorFades[i] = true;
            //else
            {
                selectedColorFades[i] = false; // Line needed in case we come back from minigame selection screen
                sc[i] = ((DatabaseClass.ColorData)unlockedCustomizables[CustomizableType.Color][selectedCustomizables[(int)CustomizableType.Color, i]]).color;
            }

            //if (selectedFaces[i] == unlockedFesses.Count)
            //    selectedRabbits[i] = true;
            //else
                selectedRabbits[i] = false; // Line needed in case we come back from minigame selection screen

            sf[i] = selectedCustomizables[(int)CustomizableType.Face, i];
            if (selectedCustomizables[(int)CustomizableType.Mustache, i] == unlockedCustomizables[CustomizableType.Mustache].Count)
                selectedMustaches[i] = "None";
            else
                selectedMustaches[i] = ((DatabaseClass.MustacheData)unlockedCustomizables[CustomizableType.Mustache][selectedCustomizables[(int)CustomizableType.Mustache, i]]).model;

            if (selectedCustomizables[(int)CustomizableType.Hat, i] == unlockedCustomizables[CustomizableType.Hat].Count)
                selectedHats[i] = "None";
            else
                selectedHats[i] = ((DatabaseClass.HatData)unlockedCustomizables[CustomizableType.Hat][selectedCustomizables[(int)CustomizableType.Hat, i]]).model;
        }
        dataContainer.SaveData(nbPlayers, sc, sf, selectedMustaches, selectedHats, selectedColorFades, selectedRabbits, selectedMode == 1);
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
