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
    public enum MenuState { Common, TitleScreen, ConfirmationScreen, ModeSelection, NumberOfPlayers, CustomisationScreen, MinigameSelection, Credits }
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

    bool creditsShown = false;
    [SerializeField]
    Sprite difficultyStar;
    [SerializeField]
    Sprite difficultyEmptyStar;

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
        Controls.nbPlayersSelectedInMenu = nbPlayers;
    }

    void LoadFromDatabase()
    {
        customizables.Clear();
        minigames.Clear();
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
    }

    private void Start()
    {
        Cursor.visible = false;
        minigameTypeSelected = false;

        // Deactivate debug tools in menu
        if (ResourceUtils.Instance != null && ResourceUtils.Instance.debugTools != null)
            ResourceUtils.Instance.debugTools.ActivateDebugMode();

        if (AudioManager.Instance != null)
            AudioManager.Instance.Fade(AudioManager.Instance.musicMenu);

        LoadFromDatabase();

        // Load data from container if players come from a minigame. Menu initialized on minigame selection screen.
        if (DataContainer.launchedFromMinigameScreen || DataContainer.isInTheShop)
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

            if (DataContainer.isInTheShop)
            {
                selectedMode = 0;
                nbPlayers = SlimeDataContainer.instance.nbPlayers;
                SetState(MenuState.CustomisationScreen);
            }
            else
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
        if (Input.GetKeyDown(KeyCode.G))
        {
            DatabaseManager.Db.ProgressGold();
            LoadFromDatabase();
        }

        // Save all players input
        for (int i = 0; i < 4; i++)
        {
            prevControllerStates[i] = controllerStates[i];
            controllerStates[i] = GamePad.GetState((PlayerIndex)i);
        }

        // Player 1 has the lead and can rewind the menu state by pressing B
        if (Controls.MenuCancellation(prevControllerStates[0], controllerStates[0], 0))
        {
            if (creditsShown)
            {
                creditsShown = false;
                transform.GetChild((int)MenuState.Credits).gameObject.SetActive(false);
            }

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
                    UpdateSelectionVisual(4, 0);
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

            if (Controls.MenuValidation(prevControllerStates[0], controllerStates[0], 0))
            {
                if (creditsShown)
                {
                    creditsShown = false;
                    transform.GetChild((int)MenuState.Credits).gameObject.SetActive(false);
                }

                if (CurrentlySelectedButton != null)
                {
                    CurrentlySelectedButton.onClick.Invoke();
                    if (!creditsShown)
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
                            transform.GetChild((int)MenuState.MinigameSelection).GetChild(4).gameObject.SetActive(false);
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
        HideDifficulty();

        for (int i = 0; i < transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).childCount; i++)
            Destroy(transform.GetChild((int)MenuState.MinigameSelection).GetChild(2).GetChild(i).gameObject);

        transform.GetChild((int)MenuState.MinigameSelection).GetChild(1).GetChild((int)selectedMinigameType).GetComponent<MinigameSelectionAnim>().ReduceYourUI();
        transform.GetChild((int)MenuState.MinigameSelection).GetChild(4).gameObject.SetActive(true);
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
        if (Controls.MenuDefaultMoveDown(controllerStates[0], prevControllerStates[0], 0))
        {
            buttonNeedUpdate = true;
            currentCursor++;
        }
        else if (Controls.MenuDefaultMoveUp(controllerStates[0], prevControllerStates[0], 0))
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
            if (Controls.MenuCancellation(prevControllerStates[i], controllerStates[i], i))
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

            // Press start when you're ready to go
            if (Controls.MenuIsReady(prevControllerStates[i], controllerStates[i], i))
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
                else
                {
                    playerCustomScreens[i].GetComponentInChildren<PlayerCosmetics>().GetComponent<Animator>().SetTrigger("APressed");
                }
            }

            playerCustomScreens[i].transform.GetChild(4).Rotate(Vector3.up, Controls.MenuRotateCharacter(controllerStates[i], i) * 150.0f * Time.deltaTime);

            if (Controls.MenuRandomize(prevControllerStates[i], controllerStates[i], i))
            {
                if (!areReady[i])
                {
                    PlayerCosmetics cosmetics = playerCustomScreens[i].GetComponentInChildren<PlayerCosmetics>();
                    cosmetics.RandomSelectionUnlocked();

                    // Update selection after random
                    for (int j = 0; j < customizables[CustomizableType.Color].Count; j++)
                    {
                        if (cosmetics.BodyColor == ((DatabaseClass.ColorData)customizables[CustomizableType.Color][j]).color)
                        {
                            selectedCustomizables[(int)CustomizableType.Color, i] = j;
                            break;
                        }
                    }

                    selectedCustomizables[(int)CustomizableType.Face, i] = cosmetics.FaceType;

                    if (cosmetics.Mustache == "None" || cosmetics.Mustache == string.Empty)
                        selectedCustomizables[(int)CustomizableType.Mustache, i] = customizables[CustomizableType.Mustache].Count;
                    else
                        selectedCustomizables[(int)CustomizableType.Mustache, i] = customizables[CustomizableType.Mustache].FindIndex(x => ((DatabaseClass.MustacheData)x).Id == cosmetics.Mustache);

                    if (cosmetics.Hat == "None" || cosmetics.Hat == string.Empty)
                        selectedCustomizables[(int)CustomizableType.Hat, i] = customizables[CustomizableType.Hat].Count;
                    else
                        selectedCustomizables[(int)CustomizableType.Hat, i] = customizables[CustomizableType.Hat].FindIndex(x => ((DatabaseClass.HatData)x).Id == cosmetics.Hat);

                    if (cosmetics.Accessory == "None" || cosmetics.Accessory == string.Empty)
                        selectedCustomizables[(int)CustomizableType.Accessory, i] = customizables[CustomizableType.Accessory].Count;
                    else
                        selectedCustomizables[(int)CustomizableType.Accessory, i] = customizables[CustomizableType.Accessory].FindIndex(x => ((DatabaseClass.AccessoryData)x).Id == cosmetics.Accessory);

                    if (cosmetics.Ears == "None" || cosmetics.Ears == string.Empty)
                        selectedCustomizables[(int)CustomizableType.Ears, i] = customizables[CustomizableType.Ears].Count;
                    else
                        selectedCustomizables[(int)CustomizableType.Ears, i] = customizables[CustomizableType.Ears].FindIndex(x => ((DatabaseClass.EarsData)x).Id == cosmetics.Ears);

                    if (cosmetics.Chin == "None" || cosmetics.Chin == string.Empty)
                        selectedCustomizables[(int)CustomizableType.Chin, i] = customizables[CustomizableType.Chin].Count;
                    else
                        selectedCustomizables[(int)CustomizableType.Chin, i] = customizables[CustomizableType.Chin].FindIndex(x => ((DatabaseClass.ChinData)x).Id == cosmetics.Chin);

                    if (cosmetics.Skin == "None" || cosmetics.Skin == string.Empty)
                        selectedCustomizables[(int)CustomizableType.Skin, i] = customizables[CustomizableType.Skin].Count;
                    else
                        selectedCustomizables[(int)CustomizableType.Skin, i] = customizables[CustomizableType.Skin].FindIndex(x => ((DatabaseClass.SkinData)x).Id == cosmetics.Skin);

                    if (cosmetics.Forehead == "None" || cosmetics.Forehead == string.Empty)
                        selectedCustomizables[(int)CustomizableType.Forehead, i] = customizables[CustomizableType.Forehead].Count;
                    else
                        selectedCustomizables[(int)CustomizableType.Forehead, i] = customizables[CustomizableType.Forehead].FindIndex(x => ((DatabaseClass.ForeheadData)x).Id == cosmetics.Forehead);
                }
                else
                {
                    FaceEmotion randomEmotion = (FaceEmotion)(UnityEngine.Random.Range(1, 5));
                    playerCustomScreens[i].GetComponentInChildren<PlayerCosmetics>().FaceEmotion = randomEmotion;
                    StartCoroutine(ResetFace(playerCustomScreens[i].GetComponentInChildren<PlayerCosmetics>(), randomEmotion));
                }
            }

            if (Controls.MenuBuy(prevControllerStates[i], controllerStates[i], i))
            {
                if (areReady[i])
                    playerCustomScreens[i].GetComponentInChildren<PlayerCosmetics>().GetComponent<Animator>().SetTrigger("YPressed");
            }

            // If the player i is ready, block LB/RB and left stick
            if (areReady[i])
                continue;

            // Y axis controls the settings selection
            if (Controls.MenuNextCustomizableType(prevControllerStates[i], controllerStates[i], i))
            {
                currentlySelectedOption[i]++;
                currentlySelectedOption[i] = currentlySelectedOption[i] % (int)CustomizableType.Size;
                UpdatePreview(i);
            }
            else if (Controls.MenuPreviousCustomizableType(prevControllerStates[i], controllerStates[i], i))
            {
                currentlySelectedOption[i]--;
                if (currentlySelectedOption[i] < 0)
                    currentlySelectedOption[i] = (int)CustomizableType.Size - 1;
                else
                    currentlySelectedOption[i] = currentlySelectedOption[i] % (int)CustomizableType.Size;
                UpdatePreview(i);
            }
            // X axis controls the settings values
            else if (Controls.MenuNextCustomizableValue(prevControllerStates[i], controllerStates[i], i))
            {
                selectedCustomizables[currentlySelectedOption[i], i]++;
                UpdatePreview(i);
            }
            else if (Controls.MenuPreviousCustomizableValue(prevControllerStates[i], controllerStates[i], i))
            {
                selectedCustomizables[currentlySelectedOption[i], i]--;
                UpdatePreview(i);
            }
        }
       
        // Buy controls
        if (Controls.MenuBuy(prevControllerStates[0], controllerStates[0], 0))
        {
            if (playerCustomScreens[0].GetComponentInChildren<UnlockableContainer>() != null) // if inactive, will be null
            {
                DatabaseClass.Unlockable unlockableData = playerCustomScreens[0].GetComponentInChildren<UnlockableContainer>().data;

                if (unlockableData.costToUnlock != -1)
                {
                    if (DatabaseManager.Db.Money >= unlockableData.costToUnlock)
                    {
                        DatabaseManager.Db.SetUnlockByCustomType(playerCustomScreens[0].GetComponentInChildren<UnlockableContainer>().type,
                            playerCustomScreens[0].GetComponentInChildren<UnlockableContainer>().data.Id, true);
                        DatabaseManager.Db.Money -= unlockableData.costToUnlock;
                        transform.GetComponentInChildren<HandleMoneyUIMenu>().GetComponent<Text>().text = DatabaseManager.Db.Money.ToString();

                        if (AudioManager.Instance != null && AudioManager.Instance.buySoundFx != null)
                            AudioManager.Instance.Play(AudioManager.Instance.buySoundFx);

                        playerCustomScreens[0].GetComponentInChildren<UnlockableContainer>().gameObject.SetActive(false);
                        UpdatePreview(0);
                    }
                }
            }
        }
    }

    IEnumerator ResetFace(PlayerCosmetics _cosmetics, FaceEmotion _emotionToCompare)
    {
        yield return new WaitForSeconds(1.0f);
        if (_emotionToCompare == _cosmetics.FaceEmotion)
            _cosmetics.FaceEmotion = FaceEmotion.Neutral;
    }

    private void MinigameSelectionCursorControls()
    {
        if (Controls.MenuNextCustomizableValue(prevControllerStates[0], controllerStates[0], 0))
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
        else if (Controls.MenuPreviousCustomizableValue(prevControllerStates[0], controllerStates[0], 0))
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

        if (Controls.MenuPreviousMinigameTypeY(prevControllerStates[0], controllerStates[0], 0))
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
        else if (Controls.MenuNextMinigameTypeY(prevControllerStates[0], controllerStates[0], 0))
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

        if (!minigameTypeSelected && canChangeSelection &&
            Controls.MenuRandomize(prevControllerStates[0], controllerStates[0], 0))
        {
            RandomizeMinigameSelection();
        }
    }

    void RandomizeMinigameSelection()
    {
        List<DatabaseClass.MinigameData> minigameTypeRand = DatabaseManager.Db.GetUnlockedMinigamesOfType((MinigameType)UnityEngine.Random.Range(0, (int)MinigameType.Size));
        while (minigameTypeRand.Count == 0)
            minigameTypeRand = DatabaseManager.Db.GetUnlockedMinigamesOfType((MinigameType)UnityEngine.Random.Range(0, (int)MinigameType.Size));

        transform.GetChild((int)MenuState.MinigameSelection).GetChild(4).gameObject.SetActive(false);

        DatabaseClass.MinigameData selectedMinigame = minigameTypeRand[UnityEngine.Random.Range(0, minigameTypeRand.Count)];
        minigameTypeSelected = true;
        minigameCurrentCursor = (int)selectedMinigame.type;
        selectedMinigameType = selectedMinigame.type;
        UpdateMinigameSelection(0);
        ShowMinigamesOfType(selectedMinigameType);

        List<DatabaseClass.MinigameData> allMinigamesOfType = DatabaseManager.Db.GetAllMinigamesOfType(selectedMinigame.type);
        for (int i = 0; i < allMinigamesOfType.Count; i++)
        {
            if (allMinigamesOfType[i].Id == selectedMinigame.Id && allMinigamesOfType[i].version == selectedMinigame.version)
            {
                minigameCurrentVerticalCursor = i;
                break;
            }
        }
        UpdateMinigameVersionSelection(0);
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

        UpdateDifficulty();
    }

    void UpdateDifficulty()
    {
        int difficulty = minigames[(int)selectedMinigameType][minigameCurrentVerticalCursor].difficulty;

        if (!minigames[(int)selectedMinigameType][minigameCurrentVerticalCursor].isUnlocked)
        {
            HideDifficulty();
        }
        else
        {
            transform.GetChild((int)MenuState.MinigameSelection).GetChild(3).gameObject.SetActive(true);
            Image[] difficultyDots = transform.GetChild((int)MenuState.MinigameSelection).GetChild(3).GetComponentsInChildren<Image>();

            for (int i = 1; i <= difficulty; i++)
                difficultyDots[i].sprite = difficultyStar;

            for (int i = difficulty + 1; i <= 5; i++)
                difficultyDots[i].sprite = difficultyEmptyStar;

            if (selectedMinigameType == MinigameType.Floor || selectedMinigameType == MinigameType.Shape)
            {
                transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).gameObject.SetActive(true);
                int tmpVersion = minigames[(int)selectedMinigameType][minigameCurrentVerticalCursor].version;

                if (tmpVersion >= 4)
                {
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(3).gameObject.SetActive(true);
                    tmpVersion -= 4;
                }
                else
                {
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(3).gameObject.SetActive(false);
                }

                if (tmpVersion >= 2)
                {
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(0).gameObject.SetActive(true);
                    tmpVersion -= 2;
                }
                else
                {
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(0).gameObject.SetActive(false);
                }

                if (tmpVersion == 1)
                {
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(1).gameObject.SetActive(true);
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(1).gameObject.SetActive(false);
                    transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).GetChild(2).gameObject.SetActive(true);
                }
            }
        }
    }

    void HideDifficulty()
    {
        transform.GetChild((int)MenuState.MinigameSelection).GetChild(3).gameObject.SetActive(false);
        transform.GetChild((int)MenuState.MinigameSelection).GetChild(5).gameObject.SetActive(false);
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

        if (currentCustomType != 0)
            playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(0).GetChild((int)currentCustomType - 1).GetComponent<Image>().color = new Color(54, 54, 54, 114) / 255.0f;
        else
            playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(0).GetChild((int)CustomizableType.Size - 1).GetComponent<Image>().color = new Color(54, 54, 54, 114) / 255.0f;

        playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(0).GetChild((int)currentCustomType).GetComponent<Image>().color = Color.yellow;
        playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(0).GetChild((int)(currentCustomType + 1)%(int)CustomizableType.Size).GetComponent<Image>().color = new Color(54, 54, 54, 114) / 255.0f;

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

            if (_playerIndex == 0)
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
            _parent.GetComponentInChildren<Text>().text = (canBeBoughtWithGold) ? _unlockableData.costToUnlock.ToString() : "???";

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
                        playerCustomScreens[i].transform.localPosition = new Vector3(-180 + (2 * i * 180), -65.0f, 0.0f);
                    if (nbPlayers >= 3)
                        playerCustomScreens[i].transform.localPosition = new Vector3(-(180) * Mathf.Pow(-1, i), (i < 2) ? 25.0f : -165.0f, 0.0f);

                    ShowControlsOnCustomScreen(i);
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

            for (int i = 0; i < playerCustomScreens.Count; i++)
            {
                playerCustomScreens[i].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = SlimeDataContainer.instance.playerColorsMenu[i];
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

    void ShowControlsOnCustomScreen(int _playerIndex)
    {
        bool showKeyboardControls = (_playerIndex == Controls.keyboardIndex);
        playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(2).GetComponent<Image>().enabled = !showKeyboardControls;
        playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(3).GetComponent<Image>().enabled = !showKeyboardControls;
        playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(4).GetComponent<Image>().enabled = showKeyboardControls;
        playerCustomScreens[_playerIndex].transform.GetChild(2).GetChild(5).GetComponent<Image>().enabled = showKeyboardControls;

        playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(0).GetComponent<Image>().enabled = !showKeyboardControls;
        playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(1).GetComponent<Image>().enabled = !showKeyboardControls;
        playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(3).GetComponent<Image>().enabled = showKeyboardControls;
        playerCustomScreens[_playerIndex].transform.GetChild(3).GetChild(4).GetComponent<Image>().enabled = showKeyboardControls;
    }

    GameObject CreatePlayerCustomScreen(int _newPlayerIndex)
    {
        GameObject go = Instantiate(playerCustomScreenPrefab, transform.GetChild((int)MenuState.CustomisationScreen));
        go.GetComponentInChildren<Text>().text = "Player " + (_newPlayerIndex + 1);

        if (nbPlayers == 1)
            go.transform.localPosition = new Vector3(0.0f, -65.0f, 0.0f);
        if (nbPlayers == 2)
            go.transform.localPosition = new Vector3(-180 + (2 * _newPlayerIndex * 180), -65.0f, 0.0f);
        if (nbPlayers >= 3)
            go.transform.localPosition = new Vector3(-(180) * Mathf.Pow(-1, _newPlayerIndex), (_newPlayerIndex < 2) ? 25.0f : -165.0f , 0.0f);

        go.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().color = Color.yellow;
        go.transform.GetChild(2).GetComponent<Text>().text = ((CustomizableType)0).ToString();
        go.transform.GetChild(3).GetComponent<Text>().text = customizables[0][0].Id;
        go.transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().SetUniqueColor(((DatabaseClass.ColorData)customizables[CustomizableType.Color][0]).color);
        go.transform.GetChild(4).GetComponentInChildren<PlayerCosmetics>().FaceType = ((DatabaseClass.FaceData)customizables[CustomizableType.Face][0]).indiceForShader;

        playerCustomScreens.Add(go);
        ShowControlsOnCustomScreen(_newPlayerIndex);

        return go;
    }

    void GoToNextState()
    {
        // Go to next state if not story + customisation or minigames and minigame selection
        if (selectedMode == 0 && currentState == MenuState.CustomisationScreen)
            return;

        // Exit game has been pressed
        if (currentState == MenuState.TitleScreen && currentCursor >= 2)
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
        if (SlimeDataContainer.instance.isInTheShop)
            return;

        if (currentState == MenuState.TitleScreen)
            return;

        if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

        if (currentState == MenuState.MinigameSelection)
        {
            if (!canChangeSelection)
                return;

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
            SendDataToContainer(MinigameType.Size);
            SlimeDataContainer.instance.isInTheShop = false;
            LevelLoader.LoadLevelWithLoadingScreen("Hub");

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



        SendDataToContainer(minigameContainer.GetMinigameType(), minigameContainer.GetMinigameVersion());
		LevelLoader.LoadLevelWithFadeOut(minigameContainer.GetMinigameId());
    }


    void SendDataToContainer(MinigameType _minigameType, int _minigameVersion = 0)
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

        if (dataContainer.isInTheShop)
            dataContainer.SaveData(dataContainer.nbPlayers, sc, sf, selectedMustaches, selectedHats, selectedEars, selectedForeheads, selectedChins,
                selectedSkins, selectedAccessories, _minigameType, _minigameVersion, selectedColorFades, false);
        else
            dataContainer.SaveData(nbPlayers, sc, sf, selectedMustaches, selectedHats, selectedEars, selectedForeheads, selectedChins,
                    selectedSkins, selectedAccessories, _minigameType, _minigameVersion, selectedColorFades, selectedMode == 1);
    }

    private void OnDestroy()
    {
        CurrentlySelectedButton = null;
    }

    public void ExitGame()
    {
        Debug.Log("Exiting this exciting game.");
        DatabaseManager.instance.SaveData();
        Application.Quit();
    }

    public void NewGamePressed()
    {
        // Reset -> Unlock all -> cowboy / candy / sneakyprogress = 0
        DatabaseManager.Db.NewGameSettings();
        LoadFromDatabase();
    }

    public void ShowCredits()
    {
        creditsShown = true;
        transform.GetChild((int)MenuState.Credits).gameObject.SetActive(true);
    }
    
}
