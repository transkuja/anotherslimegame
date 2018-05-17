using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UWPAndXInput;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Video;

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

    private List<DatabaseClass.MinigameData[]> minigames = new List<DatabaseClass.MinigameData[]>(); 

    // TODO: add in database missing values Ears, Hands, Tail
    private Dictionary<CustomizableType, List<DatabaseClass.Unlockable>> customizables = new Dictionary<CustomizableType, List<DatabaseClass.Unlockable>>();

    GamePadState[] prevControllerStates = new GamePadState[4];
    GamePadState[] controllerStates = new GamePadState[4];

    // Used to know which option is currently selected
    int[] currentlySelectedOption = new int[4];

    // Used for visual and UI "value" update 
    int[,] selectedCustomizables = new int[(int)CustomizableType.Size, 4];

    // LEGACY
    bool[] selectedColorFades = new bool[4];

    [SerializeField]
    SlimeDataContainer dataContainer;

    bool[] areReady;

    [SerializeField]
    GameObject menuCursor;
    GameObject currentCursorVisual;

    // Video Player
    private float timerBeforeStartingVideo = 2.0f;
    private float currentTimerBeforeStartingVideo = 0.0f;
    private bool isPlayingAVideo = false;

    MinigameType selectedMinigameType;
    bool minigameTypeSelected = false;

    [SerializeField]
    GameObject minigameVersionUIPrefab;
    bool canChangeSelection = true;

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
        minigameTypeSelected = false;

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

        foreach (DatabaseClass.AccessoryData accessory in DatabaseManager.Db.accessories)
        {
            customizables[CustomizableType.Accessory].Add(accessory);
        }

        foreach (DatabaseClass.ChinData chin in DatabaseManager.Db.chins)
        {
            customizables[CustomizableType.Chin].Add(chin);
        }

        foreach (DatabaseClass.SkinData skin in DatabaseManager.Db.skins)
        {
            customizables[CustomizableType.Skin].Add(skin);
        }

        foreach (DatabaseClass.ForeheadData forehead in DatabaseManager.Db.foreheads)
        {
            customizables[CustomizableType.Forehead].Add(forehead);
        }

        for (int i = 0; i < (int)MinigameType.Size; i++)
        {
            if (DatabaseManager.Db.GetAllMinigamesOfType((MinigameType)i) != null)
            {
                List<DatabaseClass.MinigameData> allMinigamesOfTypeI = DatabaseManager.Db.GetAllMinigamesOfType((MinigameType)i);
                DatabaseClass.MinigameData[] c = new DatabaseClass.MinigameData[allMinigamesOfTypeI.Count];

                for (int j = 0; j < allMinigamesOfTypeI.Count; j++)
                {
                    c[j] = allMinigamesOfTypeI[j];
                }

                minigames.Add(c);    
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

                if (isNonable[(int)CustomizableType.Ears] && dataContainer.earsSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Ears, i] = customizables[CustomizableType.Ears].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Ears, i] = customizables[CustomizableType.Ears].FindIndex(x => ((DatabaseClass.EarsData)x).Id == dataContainer.earsSelected[i]);

                if (isNonable[(int)CustomizableType.Chin] && dataContainer.chinsSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Chin, i] = customizables[CustomizableType.Chin].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Chin, i] = customizables[CustomizableType.Chin].FindIndex(x => ((DatabaseClass.ChinData)x).Id == dataContainer.chinsSelected[i]);

                if (isNonable[(int)CustomizableType.Skin] && dataContainer.skinsSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Skin, i] = customizables[CustomizableType.Skin].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Skin, i] = customizables[CustomizableType.Skin].FindIndex(x => ((DatabaseClass.SkinData)x).Id == dataContainer.skinsSelected[i]);

                if (isNonable[(int)CustomizableType.Accessory] && dataContainer.accessoriesSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Accessory, i] = customizables[CustomizableType.Accessory].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Accessory, i] = customizables[CustomizableType.Accessory].FindIndex(x => ((DatabaseClass.AccessoryData)x).Id == dataContainer.accessoriesSelected[i]);

                if (isNonable[(int)CustomizableType.Forehead] && dataContainer.foreheadsSelected[i] == "None")
                    selectedCustomizables[(int)CustomizableType.Forehead, i] = customizables[CustomizableType.Forehead].Count;
                else
                    selectedCustomizables[(int)CustomizableType.Forehead, i] = customizables[CustomizableType.Forehead].FindIndex(x => ((DatabaseClass.ForeheadData)x).Id == dataContainer.foreheadsSelected[i]);

                currentlySelectedOption = new int[4];
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
                        if (!minigameTypeSelected)
                        {
                            selectedMinigameType = (MinigameType)minigameCurrentCursor;
                            minigameTypeSelected = true;
                            ShowMinigamesOfType(selectedMinigameType);
                        }
                        else
                        {
                            GoToNextStateFromMinigameSelection();
                            return;
                        }
                    }
                }
            }
        }
        // Customisation screen inputs & behaviours
        else
        {
            CustomScreenControls();
        }

        // Video player
        //if (!isPlayingAVideo && selectedMode == 1 && currentState == MenuState.MinigameSelection && minigameTypeSelected)
        //{
        //    if (minigames[minigameCurrentCursor][minigameCurrentVerticalCursor].isUnlocked && minigames[minigameCurrentCursor][minigameCurrentVerticalCursor].videoPreview != "")
        //    {
        //        currentTimerBeforeStartingVideo += Time.deltaTime;
        //        if (currentTimerBeforeStartingVideo > timerBeforeStartingVideo)
        //        {
        //            VideoPlayer vp = transform.GetChild((int)MenuState.MinigameSelection)
        //                .GetChild(2).GetComponent<MinigameSelectionAnim>().GetComponentInChildren<VideoPlayer>();

        //            transform.GetChild((int)MenuState.MinigameSelection)
        //            .GetChild(2).GetChild(2).GetComponentInChildren<RawImage>().enabled = true;

        //            if (vp)
        //            {
        //                vp.Play();
        //                isPlayingAVideo = true;
        //            }

        //        }
           
        //    }
        //}
    }

    private void ShowMinigamesOfType(MinigameType _type)
    {
        for (int i = 0; i < (int)MinigameType.Size; i++)
        {
            if (i == (int)_type)
                continue;

            transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild(i).GetComponent<MinigameSelectionAnim>().Hide();
        }

        transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild((int)_type).GetComponent<MinigameSelectionAnim>().EnlargeYourUI();

        //for (int i = 0; i < transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).childCount; i++)
        //    DestroyImmediate(transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).GetChild(i).gameObject);

        for (int i = 0; i < minigames[(int)_type].Length; i++)
        {
            GameObject _newButton = Instantiate(minigameVersionUIPrefab, transform.GetChild((int)MenuState.MinigameSelection).GetChild(2));
            _newButton.GetComponent<MinigameSelectionAnim>().SetMinigame(minigames[(int)_type][i]);

            _newButton.transform.localPosition = Vector3.right * 250.0f + Vector3.up * ((minigames[(int)_type].Length * 25 - 25) - (50 * i));
            _newButton.transform.localScale = Vector3.one;
        }

        UpdateMinigameVersionSelection(0);
    }

    void ReturnToMinigameTypeSelection()
    {
        canChangeSelection = false;
        for (int i = 0; i < transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).childCount; i++)
            Destroy(transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).GetChild(i).gameObject);

        transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild((int)selectedMinigameType).GetComponent<MinigameSelectionAnim>().ReduceYourUI();
        Invoke("ShowMinigameTypesOnReturn", 0.4f);
    }

    void ShowMinigameTypesOnReturn()
    {
        for (int i = 0; i < (int)MinigameType.Size; i++)
        {
            if (i == minigameCurrentCursor)
                continue;

            transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild(i).GetComponent<MinigameSelectionAnim>().Show();
        }
        Invoke("CanChangeSelection", 0.2f);
    }

    void CanChangeSelection()
    {
        canChangeSelection = true;
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

    private void CustomScreenControls()
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


            // Buy controls
            if (prevControllerStates[i].Buttons.Y == ButtonState.Released && controllerStates[i].Buttons.Y == ButtonState.Pressed)
            {
                if (playerCustomScreens[i].GetComponentInChildren<UnlockableContainer>() != null) // if inactive, will be null
                {
                    DatabaseClass.Unlockable unlockableData = playerCustomScreens[i].GetComponentInChildren<UnlockableContainer>().data;

                    if (unlockableData.costToUnlock != -1)
                    {
                        if (DatabaseManager.Db.Money >= unlockableData.costToUnlock)
                        {
                            DatabaseManager.Db.SetUnlockByCustomType(playerCustomScreens[i].GetComponentInChildren<UnlockableContainer>().type,
                                playerCustomScreens[i].GetComponentInChildren<UnlockableContainer>().data.Id, true);
                            DatabaseManager.Db.Money -= unlockableData.costToUnlock;

                            if (AudioManager.Instance != null && AudioManager.Instance.buySoundFx != null)
                                AudioManager.Instance.Play(AudioManager.Instance.buySoundFx);

                            playerCustomScreens[i].GetComponentInChildren<UnlockableContainer>().gameObject.SetActive(false);
                            UpdatePreview(i);
                        }
                    }
                }
            }

            playerCustomScreens[i].transform.GetChild(4).Rotate(Vector3.up, controllerStates[i].ThumbSticks.Right.X * 150.0f * Time.deltaTime);
        }
    }

    private void MinigameSelectionCursorControls()
    {
        if ((controllerStates[0].ThumbSticks.Left.X > 0.5f && prevControllerStates[0].ThumbSticks.Left.X < 0.5f)
            // Keyboard input
            || (Input.GetKeyDown(KeyCode.D) || (Input.GetKeyDown(KeyCode.RightArrow)))
            )
        {
            if (!minigameTypeSelected && canChangeSelection)
            {
                if (minigameCurrentCursor < (int)MinigameType.Size - 1)
                {
                    minigameCurrentCursor++;
                    minigameCurrentCursor %= (int)MinigameType.Size;

                    UpdateMinigameSelection(minigameCurrentCursor - 1);
                }
            }
        }
        else if ((controllerStates[0].ThumbSticks.Left.X < -0.5f && prevControllerStates[0].ThumbSticks.Left.X > -0.5f)
            // Keyboard input
            || (Input.GetKeyDown(KeyCode.Q) || (Input.GetKeyDown(KeyCode.LeftArrow)))
            )
        {
            if (!minigameTypeSelected && canChangeSelection)
            {
                minigameCurrentCursor--;

                if (minigameCurrentCursor < 0)
                    minigameCurrentCursor = 0;
                else
                    UpdateMinigameSelection(minigameCurrentCursor + 1);
            }
        }

        if ((controllerStates[0].ThumbSticks.Left.Y > 0.5f && prevControllerStates[0].ThumbSticks.Left.Y < 0.5f)
           // Keyboard input
           || (Input.GetKeyDown(KeyCode.Z) || (Input.GetKeyDown(KeyCode.UpArrow)))
           )
        {
            if (minigameTypeSelected)
            {
                int oldValue = minigameCurrentVerticalCursor;
                minigameCurrentVerticalCursor--;
                if (minigameCurrentVerticalCursor < 0)
                    minigameCurrentVerticalCursor = minigames[(int)selectedMinigameType].Length - 1;
                else
                    minigameCurrentVerticalCursor %= minigames[(int)selectedMinigameType].Length;

                UpdateMinigameVersionSelection(oldValue);
            }
            else
            {
                if (canChangeSelection)
                {
                    if (minigameCurrentCursor >= (int)MinigameType.Size / 2)
                    {
                        minigameCurrentCursor -= (int)MinigameType.Size / 2;
                        UpdateMinigameSelection(minigameCurrentCursor + (int)MinigameType.Size / 2);
                    }
                }
            }
        }
        else if ((controllerStates[0].ThumbSticks.Left.Y < -0.5f && prevControllerStates[0].ThumbSticks.Left.Y > -0.5f)
            // Keyboard input
            || (Input.GetKeyDown(KeyCode.S) || (Input.GetKeyDown(KeyCode.DownArrow)))
            )
        {
            if (minigameTypeSelected)
            {
                int oldValue = minigameCurrentVerticalCursor;
                minigameCurrentVerticalCursor++;
                minigameCurrentVerticalCursor %= minigames[(int)selectedMinigameType].Length;

                UpdateMinigameVersionSelection(oldValue);
            }
            else
            {
                if (canChangeSelection)
                {
                    if (minigameCurrentCursor < (int)MinigameType.Size / 2)
                    {
                        minigameCurrentCursor += (int)MinigameType.Size / 2;
                        UpdateMinigameSelection(minigameCurrentCursor - (int)MinigameType.Size / 2);
                    }
                }
            }
            //UpdateMinigameSelection();
        }
    }

    private void UpdateMinigameSelection(int _oldValue)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

        transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild(_oldValue).GetComponent<MinigameSelectionAnim>().IsSelected(false);

        // Currently selected
        transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild(minigameCurrentCursor).GetComponent<MinigameSelectionAnim>().IsSelected(true);
            //SetMinigame(minigames[minigameCurrentCursor][minigameCurrentVerticalCursor]);

        //// stop previous video player
        //transform.GetChild((int)MenuState.MinigameSelection)
        // .GetChild(2).GetChild(2).GetComponentInChildren<RawImage>().enabled = false;

        //VideoPlayer vp = transform.GetChild((int)MenuState.MinigameSelection)
        //    .GetChild(2).GetComponent<MinigameSelectionAnim>().GetComponentInChildren<VideoPlayer>();

        //if (vp && vp.isPlaying)
        //{
        //    isPlayingAVideo = false;
        //    vp.Stop();
        //}

        // Restart timer
        //if (minigames[minigameCurrentCursor][minigameCurrentVerticalCursor].videoPreview != "")
        //{
        //    currentTimerBeforeStartingVideo = 0.0f;
        //}
    }

    private void UpdateMinigameVersionSelection(int _oldValue)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

        transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).GetChild(_oldValue).GetComponent<MinigameSelectionAnim>().IsSelected(false);

        // Currently selected
        transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).GetChild(minigameCurrentVerticalCursor).GetComponent<MinigameSelectionAnim>().IsSelected(true);

        transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild((int)selectedMinigameType).GetComponent<MinigameSelectionAnim>()
            .SetMinigame(minigames[(int)selectedMinigameType][minigameCurrentVerticalCursor]);
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
            bool isUnlocked = false;
            DatabaseClass.Unlockable unlockableData = null;
            if (noneValue)
            {
                playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = "None";
            }
            else
            {
                unlockableData = customizables[currentCustomType][selectedCustomizables[(int)currentCustomType, _playerIndex]];
                playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().text = unlockableData.Id;
                isUnlocked = unlockableData.isUnlocked;

                if (!isUnlocked)
                {
                    playerCustomScreens[_playerIndex].transform.GetChild(3).GetComponent<Text>().color = Color.red;
                }
            }

            HandleHowToUnlock(playerCustomScreens[_playerIndex].transform.GetChild(6), unlockableData, currentCustomType);
            UpdatePlayerVisual(_playerIndex, currentCustomType, selectedCustomizables[(int)currentCustomType, _playerIndex], noneValue || !isUnlocked);
        }
    }

    void HandleHowToUnlock(Transform _parent, DatabaseClass.Unlockable _unlockableData = null, CustomizableType _type = 0)
    {
        if (_unlockableData == null || _unlockableData.isUnlocked)
        {
            _parent.gameObject.SetActive(false);
        }
        else
        {
            _parent.gameObject.SetActive(true);
            bool canBeBoughtWithGold = _unlockableData.costToUnlock != -1;
            _parent.GetComponentInChildren<Text>().text = (canBeBoughtWithGold) ? DatabaseManager.Db.Money.ToString() + " / " + _unlockableData.costToUnlock.ToString() : "???";

            _parent.GetChild(1).GetComponent<Image>().enabled = canBeBoughtWithGold;
            _parent.GetChild(2).GetComponent<Image>().enabled = (canBeBoughtWithGold && DatabaseManager.Db.Money >= _unlockableData.costToUnlock);
            _parent.GetComponentInChildren<Text>().color = (canBeBoughtWithGold && DatabaseManager.Db.Money < _unlockableData.costToUnlock) ? Color.red : Color.yellow;

            _parent.GetComponent<UnlockableContainer>().data = _unlockableData;
            _parent.GetComponent<UnlockableContainer>().type = _type;
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
                UpdatePlayerPreviewAccessory(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Chin:
                UpdatePlayerPreviewChin(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Forehead:
                UpdatePlayerPreviewForehead(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
            case CustomizableType.Skin:
                UpdatePlayerPreviewSkin(_playerIndex, _unlockedIndex, _isNoneValue);
                break;
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

    void UpdatePlayerPreviewChin(int _playerIndex, int _selection, bool _isNoneValue)
    {
        PlayerCosmetics playerCosmetics = playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>();

        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Chin) - 2);
        if (parent.childCount < 1)
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCosmetics));
        }

        if (!_isNoneValue)
        {
            playerCosmetics.Chin = ((DatabaseClass.ChinData)customizables[CustomizableType.Chin][_selection]).Id;
        }
        else
        {
            playerCosmetics.Chin = String.Empty;
        }
    }

    void UpdatePlayerPreviewSkin(int _playerIndex, int _selection, bool _isNoneValue)
    {
        PlayerCosmetics playerCosmetics = playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>();

        if (!_isNoneValue)
        {
            playerCosmetics.Skin = ((DatabaseClass.SkinData)customizables[CustomizableType.Skin][_selection]).Id;
        }
        else
        {
            playerCosmetics.Skin = String.Empty;
        }
    }

    void UpdatePlayerPreviewAccessory(int _playerIndex, int _selection, bool _isNoneValue)
    {
        PlayerCosmetics playerCosmetics = playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>();

        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Accessory) - 2);
        if (parent.childCount < 1)
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCosmetics));
        }

        if (!_isNoneValue)
        {
            playerCosmetics.Accessory = ((DatabaseClass.AccessoryData)customizables[CustomizableType.Accessory][_selection]).Id;
        }
        else
        {
            playerCosmetics.Accessory = String.Empty;
        }
    }

    void UpdatePlayerPreviewForehead(int _playerIndex, int _selection, bool _isNoneValue)
    {
        PlayerCosmetics playerCosmetics = playerCustomScreens[_playerIndex].GetComponentInChildren<PlayerCosmetics>();

        Transform parent = playerCustomScreens[_playerIndex].transform.GetComponentInChildren<CustomizableSockets>().transform.GetChild((int)(CustomizableType.Forehead) - 2);
        if (parent.childCount < 1)
        {
            if (!_isNoneValue)
                StartCoroutine(Happy(playerCosmetics));
        }

        if (!_isNoneValue)
        {
            playerCosmetics.Forehead = ((DatabaseClass.ForeheadData)customizables[CustomizableType.Forehead][_selection]).Id;
        }
        else
        {
            playerCosmetics.Forehead = String.Empty;
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
                        UpdatePreview(i);
                    }

                }
                CurrentlySelectedButton = null;
            }

        }

        // Minigame screen reset
        if (currentState == MenuState.MinigameSelection)
        {
            CurrentlySelectedButton = null;
            // MINIGAMES TYPE
            int oldValue = minigameCurrentCursor;
            minigameCurrentCursor = 0;
            UpdateMinigameSelection(oldValue);
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

        if (currentState == MenuState.MinigameSelection)
        {
            if (minigameTypeSelected)
            {
                minigameTypeSelected = false;
                minigameCurrentVerticalCursor = 0;
                ReturnToMinigameTypeSelection();
                return;
            }
        }
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
            for (int j = 2; j < (int)CustomizableType.Size; j++)
            {
                if (customizables[(CustomizableType)j].Count != 0 && selectedCustomizables[j, i] != customizables[(CustomizableType)j].Count
                        && !customizables[(CustomizableType)j][selectedCustomizables[j, i]].isUnlocked)
                    selectedCustomizables[j, i] = customizables[(CustomizableType)j].Count;
            }
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
        MinigameSelectionAnim minigameContainer = transform.GetChild((int)MenuState.MinigameSelection)
                    .GetChild(2).GetChild(minigameCurrentVerticalCursor).GetComponent<MinigameSelectionAnim>();

        if (!minigameContainer.IsUnlocked())
        {
            if (AudioManager.Instance != null && AudioManager.Instance.incorrectFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.incorrectFx);
            return;
        }

        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);


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

        string[] selectedChins = new string[nbPlayers];
        string[] selectedSkins = new string[nbPlayers];
        string[] selectedAccessories = new string[nbPlayers];
        string[] selectedForeheads = new string[nbPlayers];

        for (int i = 0; i < nbPlayers; i++)
        {
            //if (selectedColors[i] == unlockedCustomColors.Count)
            //    selectedColorFades[i] = true;
            //else
            {
                selectedColorFades[i] = false; // Line needed in case we come back from minigame selection screen
                sc[i] = ((DatabaseClass.ColorData)customizables[CustomizableType.Color][selectedCustomizables[(int)CustomizableType.Color, i]]).color;
            }


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

            ///////////////////////////////////////////////////////////////////////////////
            if (customizables[CustomizableType.Chin].Count == 0 || selectedCustomizables[(int)CustomizableType.Chin, i] == customizables[CustomizableType.Chin].Count)
                selectedChins[i] = "None";
            else
                selectedChins[i] = customizables[CustomizableType.Chin][selectedCustomizables[(int)CustomizableType.Chin, i]].Id;

            if (customizables[CustomizableType.Skin].Count == 0 || selectedCustomizables[(int)CustomizableType.Skin, i] == customizables[CustomizableType.Skin].Count)
                selectedSkins[i] = "None";
            else
                selectedSkins[i] = customizables[CustomizableType.Skin][selectedCustomizables[(int)CustomizableType.Skin, i]].Id;

            if (customizables[CustomizableType.Forehead].Count == 0 || selectedCustomizables[(int)CustomizableType.Forehead, i] == customizables[CustomizableType.Forehead].Count)
                selectedForeheads[i] = "None";
            else
                selectedForeheads[i] = customizables[CustomizableType.Forehead][selectedCustomizables[(int)CustomizableType.Forehead, i]].Id;

            if (customizables[CustomizableType.Accessory].Count == 0 || selectedCustomizables[(int)CustomizableType.Accessory, i] == customizables[CustomizableType.Accessory].Count)
                selectedAccessories[i] = "None";
            else
                selectedAccessories[i] = customizables[CustomizableType.Accessory][selectedCustomizables[(int)CustomizableType.Accessory, i]].Id;
            ////////////////////////////////////////////////////////////////////////////////
        }
        dataContainer.SaveData(nbPlayers, sc, sf, selectedMustaches, selectedHats, selectedEars, selectedForeheads, selectedChins,
                selectedSkins, selectedAccessories, _minigameVersion, selectedColorFades, selectedMode == 1);
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
