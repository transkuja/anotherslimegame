using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public enum ViewMode {
    thirdPerson3d, // Camera + deplacement comme dans  hub
    sideView3d, // Camera avec orientation quasie fixe sur coté + deplacement 3d  
    sideView2d// Camera avec orientation quasie fixe + deplacement 2d 
} 

public class GameModeData
{

    public GameModeData(GameMode mode)
    {

    }
}


abstract public class GameMode : MonoBehaviour
{
    [SerializeField] protected int nbPlayersMin;
    [SerializeField] protected int nbPlayersMax;
    // Use to remove damage on points based on gamemode when players collide. Players will still be expulsed
    [SerializeField] private bool takesDamageFromPlayer = true;
    // Use to remove damage on points based on gamemode when players collide with a trap. Players will still be expulsed
    [SerializeField] private bool takesDamageFromTraps = true;
    [SerializeField] private ViewMode viewMode = ViewMode.thirdPerson3d;

    protected MinigameRules rules;

    GamePadState prevState;
    GamePadState curState;

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
#endregion

    public void Awake()
    {
        GameManager.Instance.CurrentGameMode = this;
    }
    public virtual bool IsMiniGame()
    {
        return !(this is HubMode);
    }

    public virtual void StartGame(List<GameObject> playerReferences)
    {
        if (IsMiniGame())
            GameManager.ChangeState(GameState.ForcedPauseMGRules);
    }

    public virtual void OpenRuleScreen()
    {
        if (!IsMiniGame())
            return;

        Transform ruleScreenRef = GameManager.UiReference.RuleScreen;

        ruleScreenRef.GetComponentInChildren<Text>().text = rules.title;
        ruleScreenRef.GetChild(1).GetComponent<Text>().text = rules.howToPlay;

        if (rules.controls.Count > 0)
        {
            GameObject controlDetailsPage = new GameObject("ControlDetailsPage");
            controlDetailsPage.transform.SetParent(ruleScreenRef);
            controlDetailsPage.transform.localPosition = Vector3.zero;
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
        }

        if (rules.possiblePickups.Count > 0)
        {
            GameObject possiblePickupsPage = new GameObject("PossiblePickupsPagePage");
            possiblePickupsPage.transform.SetParent(ruleScreenRef);
            possiblePickupsPage.transform.localPosition = Vector3.zero;
            possiblePickupsPage.SetActive(false);

            int i = 0;
            foreach (PossiblePickup pickup in rules.possiblePickups)
            {
                GameObject entry = Instantiate(ResourceUtils.Instance.feedbacksManager.ruleScreenShortPrefab, possiblePickupsPage.transform);
                entry.transform.localPosition = new Vector2(0, 100 * (1 - i));

                GameObject pickupPreview = Instantiate(ResourceUtils.Instance.feedbacksManager.GetPickupPreview(pickup.pickupType), entry.GetComponentInChildren<Image>().transform);
                pickupPreview.transform.localPosition = Vector3.zero;
                pickupPreview.transform.localScale *= 25.0f;
                entry.GetComponentInChildren<Image>().enabled = false;

                entry.GetComponentInChildren<Text>().text = pickup.description;
                i++;
            }
        }
    }

    private void Update()
    {
        if (GameManager.CurrentState == GameState.ForcedPauseMGRules)
        {
            prevState = curState;
            curState = GamePad.GetState(0);

            if (prevState.Buttons.A == ButtonState.Released && curState.Buttons.A == ButtonState.Pressed)
            {
                // Next page
            }
            else if (prevState.Buttons.B == ButtonState.Released && curState.Buttons.B == ButtonState.Pressed)
            {
                // Previous page
            }
        }
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
            cameraReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1.0f);
        }
        // By default, cameraP3 and cameraP4 are set for 4-Player mode, so we only update cameraP1 and cameraP2
        else if (activePlayersAtStart > 2)
        {
            cameraReferences[0].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0, 0.5f, 0.5f, 0.5f);
            cameraReferences[1].transform.GetChild(0).GetComponent<Camera>().rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }
    public virtual void PlayerHasFinished(Player player)
    {
        throw new NotImplementedException();
    }
}

// TODO: @Anthony, move these elsewhere
public class ControlDetails
{
    public ControlType button;
    public string description;

    public ControlDetails(ControlType _button,  string _description)
    {
        button = _button;
        description = _description;
    }

    public ControlDetails(ControlType _button)
    {
        if (_button == ControlType.Movement)
            description = "Use this to move";

        else if (_button == ControlType.Jump)
            description = "Press this to jump";

        button = _button;
    }
}

public class PossiblePickup
{
    // TODO: @Anthony, ColorFloorPickup should be generic pickup
    public PickUpType pickupType;
    public string description;

    public PossiblePickup(PickUpType _pickupType, string _description)
    {
        pickupType = _pickupType;
        description = _description;
    }
}

public class MinigameRules
{
    public string title;
    public string howToPlay;
    public List<ControlDetails> controls = new List<ControlDetails>();
    public List<PossiblePickup> possiblePickups = new List<PossiblePickup>();

    public MinigameRules(GameMode _curGameMode)
    {
        title = MinigameDataUtils.GetTitle(_curGameMode);
        howToPlay = MinigameDataUtils.GetDescription(_curGameMode);
        controls = MinigameDataUtils.GetControls(_curGameMode);
        possiblePickups = MinigameDataUtils.GetPossiblePickups(_curGameMode);
    }

    public MinigameRules(string _title, string _howToPlay)
    {
        title = _title;
        howToPlay = _howToPlay;
    }

    public MinigameRules(string _title, string _howToPlay, List<ControlDetails> _controls, List<PossiblePickup> _possiblePickups)
    {
        title = _title;
        howToPlay = _howToPlay;
        controls = _controls;
        possiblePickups = _possiblePickups;
    }

    public void AddControl(ControlType _newButton, string _effect)
    {
        controls.Add(new ControlDetails(_newButton, _effect));
    }

    public void AddPossiblePickup(PickUpType _newPossiblePickupType, string _effect)
    {
        possiblePickups.Add(new PossiblePickup(_newPossiblePickupType, _effect));
    }
}

public enum ControlType { Movement, Jump, Action, RightTrigger }

/*
 * Handles gamemodes with an internal database in code
 */
//public class GameModeManager
//{
//    //GameMode escapeMode = new GameMode(GameModeType.Escape, EvolutionMode.GrabEvolution, 1, 4);

//    //GameMode arenaMode1 = new GameMode(GameModeType.Arena, EvolutionMode.GrabCollectableAndAutoEvolve, 1, 4);
//    //GameMode arenaMode2 = new GameMode(GameModeType.Arena, EvolutionMode.GrabCollectableAndActivate, 1, 4);


//    //public GameMode GetGameModeByName(GameModeType _name, EvolutionMode _evolutionMode = EvolutionMode.GrabEvolution)
//    //{
//    //    //switch (_name)
//    //    //{
//    //    //    case GameModeType.Escape:
//    //    //        return escapeMode;
//    //    //    case GameModeType.Arena:
//    //    //        if (_evolutionMode == EvolutionMode.GrabCollectableAndAutoEvolve)
//    //    //            return arenaMode1;
//    //    //        else
//    //    //            return arenaMode2;
//    //    //    default:
//    //    //        Debug.LogWarning("The gamemode name specified is unknown:" + _name);
//    //    //        return null;
//    //    //}
//    //}
//}