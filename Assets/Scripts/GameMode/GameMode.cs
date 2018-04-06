﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public enum ViewMode {
    thirdPerson3d, // Camera + deplacement comme dans  hub
    sideView3d, // Camera avec orientation quasie fixe sur coté + deplacement 3d  
    sideView2d// Camera avec orientation quasie fixe + deplacement 2d 
} 

public enum RuneObjective
{
    None,
    Points,
    Time,
    PointsAndTime    
}

public class GameModeData
{

    public GameModeData(GameMode mode)
    {

    }
}


abstract public class GameMode : MonoBehaviour
{
    [SerializeField] public String title = "";
    [SerializeField] protected int nbPlayersMin;
    [SerializeField] protected int nbPlayersMax;
    // Use to remove damage on points based on gamemode when players collide. Players will still be expulsed
    [SerializeField] private bool takesDamageFromPlayer = true;
    // Use to remove damage on points based on gamemode when players collide with a trap. Players will still be expulsed
    [SerializeField] private bool takesDamageFromTraps = true;
    [SerializeField] private ViewMode viewMode = ViewMode.thirdPerson3d;
    [SerializeField] private string runeToUnlockId;

    public int curNbPlayers;

    public MinigameRules rules;
    public delegate bool CheckRuneObjective();

    public CheckRuneObjective checkRuneObjective;

    public RuneObjective runeObjective;
    public int necessaryPointsForRune;
    public float necessaryTimeForRune;
    protected int currentScore = 0;

    #region getterSetters
    public bool TakesDamageFromPlayer
    {
        get
        {
            return takesDamageFromPlayer;
        }
    }

    public bool TakesDamageFromTraps
    {
        get
        {
            return takesDamageFromTraps;
        }
    }

    public ViewMode ViewMode{get{return viewMode;}}

    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
    }
    #endregion

    public void Awake()
    {
        GameManager.Instance.CurrentGameMode = this;
    }
    public virtual bool IsMiniGame()
    {
        return !(this is HubMode);
    }

    public virtual void StartGame(List<GameObject> playerReferences) // before Screen Rules
    {
        curNbPlayers = playerReferences.Count;
        if (IsMiniGame())
            GameManager.ChangeState(GameState.ForcedPauseMGRules);
        else
            GameManager.ChangeState(GameState.Normal);

    }
    public virtual void OnReadySetGoBegin()
    {
        if (FindObjectOfType<CursorPlayerId>()) FindObjectOfType<CursorPlayerId>().Init();
    }

    public virtual void OpenRuleScreen()
    {
        if (!IsMiniGame())
            return;

        // Rune can't be obtained from minigame selection screen
        if (GameManager.Instance.DataContainer != null)
        {
            if (GameManager.Instance.DataContainer.launchedFromMinigameScreen)
                runeObjective = RuneObjective.None;
        }

        Transform ruleScreenRef = GameManager.UiReference.RuleScreen;
        
        ruleScreenRef.GetComponentInChildren<Text>().text = rules.title;
        ruleScreenRef.GetChild(1).GetComponent<Text>().text = rules.howToPlay + ((runeObjective != RuneObjective.None) ? "\n\nRune objective:\n" + rules.runeObtention : "");
        ruleScreenRef.GetChild(1).gameObject.SetActive(true);

        GameObject controlDetailsPage = new GameObject("ControlDetailsPage");
        controlDetailsPage.transform.SetParent(ruleScreenRef);
        controlDetailsPage.transform.localPosition = Vector3.zero;
        controlDetailsPage.transform.localRotation = Quaternion.identity;
        controlDetailsPage.transform.localScale = Vector3.one;
        controlDetailsPage.SetActive(false);

        int i = 0;
        foreach (ControlDetails control in rules.controls)
        {
            GameObject entry = Instantiate(ResourceUtils.Instance.feedbacksManager.ruleScreenShortPrefab, controlDetailsPage.transform);
            entry.transform.localPosition = new Vector2(0, 100 * (1 - i));
            entry.GetComponentInChildren<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetControlSprite(control.button);
            entry.GetComponentInChildren<Text>().text = control.description;
            i++;
        }
        

        GameObject possiblePickupsPage = new GameObject("PossiblePickupsPagePage");
        possiblePickupsPage.transform.SetParent(ruleScreenRef);
        possiblePickupsPage.transform.localPosition = Vector3.zero;
        possiblePickupsPage.transform.localRotation = Quaternion.identity;
        possiblePickupsPage.transform.localScale = Vector3.one;
        possiblePickupsPage.SetActive(false);

        i = 0;
        foreach (PossiblePickup pickup in rules.possiblePickups)
        {
            GameObject entry = Instantiate(ResourceUtils.Instance.feedbacksManager.ruleScreenShortPrefab, possiblePickupsPage.transform);
            entry.transform.localPosition = new Vector2(0, 100 * (1 - i));

            GameObject pickupPreview = Instantiate(ResourceUtils.Instance.feedbacksManager.GetPickupPreview(pickup.pickupType), entry.GetComponentInChildren<Image>().transform);
            pickupPreview.transform.localPosition = Vector3.right * 15 + Vector3.forward * -50;
            pickupPreview.transform.localScale *= 25.0f;
            entry.GetComponentInChildren<Image>().enabled = false;

            entry.GetComponentInChildren<Text>().text = pickup.description;
            i++;
        }

        ruleScreenRef.gameObject.SetActive(true);
    }

    protected virtual void Update()
    {
        
    }

    public virtual void AttributeCamera(uint activePlayersAtStart, GameObject[] cameraReferences, List<GameObject> playersReference)
    {
        if (cameraReferences.Length == 0)
        {
            return;
        }

        // By default, cameraP2 is set for 2-Player mode, so we only update cameraP1
        if (activePlayersAtStart == 2)
        {
            cameraReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0, 0.499f, 1.0f);
            cameraReferences[1].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.502f, 0, 0.499f, 1.0f);
        }
        // By default, cameraP3 and cameraP4 are set for 4-Player mode, so we only update cameraP1 and cameraP2
        else if (activePlayersAtStart > 2)
        {
            cameraReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0.5f, 0.49f, 0.5f);
            cameraReferences[1].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.52f, 0.5f, 0.49f, 0.5f);
        }
        for(int i = 0; i < cameraReferences.Length; i++)
        {
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().cameraXAdjuster = 0.3f / activePlayersAtStart;
            cameraReferences[i].transform.GetChild(1).GetComponent<DynamicJoystickCameraController>().cameraYAdjuster = 0.05f / activePlayersAtStart;
        }


        GameObject cameraForBlackBackground = new GameObject("CameraForBlackBackground");
        Camera theCamera = cameraForBlackBackground.AddComponent<Camera>();
        theCamera.backgroundColor = Color.black;
        theCamera.cullingMask = 0;
        theCamera.clearFlags = CameraClearFlags.SolidColor;

        theCamera.Render();

        Destroy(cameraForBlackBackground, 0.05f);
    }

    /* 
     * This method should only be overriden if players can finish the minigame separately. 
     * Else, end game is already called by GameManager final countdown (tududuuuu duuuu     tududu du duuuu).
     */
    public virtual void PlayerHasFinished(Player player)
    {
        throw new NotImplementedException();
    }

    public bool IsRuneUnlocked()
    {
        return DatabaseManager.Db.IsUnlock<DatabaseClass.RuneData>(runeToUnlockId);
    }

    public void UnlockRune()
    {
        DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(runeToUnlockId, true);
    }
}