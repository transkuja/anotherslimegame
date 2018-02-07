﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleScreenHandler : MonoBehaviour {
    enum RuleScreenState { FirstPage, ControlsPage, PickupPage }
    RuleScreenState curState = RuleScreenState.FirstPage;

    private RuleScreenState CurState {
        set
        {
            for (int i = 1; i < transform.childCount; i++)
            {
                if (i - 1 == (int)value)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                else
                    transform.GetChild(i).gameObject.SetActive(false);
            }
            
            curState = value;
        }
    }

    void Start () {
        CurState = RuleScreenState.FirstPage;
    }

    public void ChangeState(bool _stateForward)
    {
        if (_stateForward)
        {
            if (curState != RuleScreenState.PickupPage)
                CurState = (RuleScreenState)(int)curState + 1;
            else
            {
                CleanUpAndStart();
                GameObject readySetGo = Instantiate(ResourceUtils.Instance.spriteUtils.spawnableSpriteUI, GameManager.UiReference.transform);
                readySetGo.AddComponent<ReadySetGo>();
            }
        }

        if (!_stateForward && curState != RuleScreenState.FirstPage)
            CurState = (RuleScreenState)(int)curState - 1;

    }

    // WARNING, should only be called from the outside in player start for debug purpose
    public void CleanUpAndStart()
    {
        // TODO: may consider destroying on creation instead, so we can show it back later through pause menu
        Destroy(transform.GetChild(2).gameObject);
        Destroy(transform.GetChild(3).gameObject);
        gameObject.SetActive(false);
    }
}

public enum ControlType { Movement, Jump, Action, RightTrigger }
public class ControlDetails
{
    public ControlType button;
    public string description;

    public ControlDetails(ControlType _button, string _description)
    {
        button = _button;
        description = _description;
    }

    public ControlDetails(ControlType _button)
    {
        if (_button == ControlType.Movement)
            description = "Move with L stick";

        else if (_button == ControlType.Jump)
            description = "Jump with A";

        button = _button;
    }
}

public class PossiblePickup
{
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
    public string runeObtention;
    public List<ControlDetails> controls = new List<ControlDetails>();
    public List<PossiblePickup> possiblePickups = new List<PossiblePickup>();

    public MinigameRules(GameMode _curGameMode)
    {
        title = MinigameDataUtils.GetTitle(_curGameMode);
        howToPlay = MinigameDataUtils.GetDescription(_curGameMode);
        controls = MinigameDataUtils.GetControls(_curGameMode);
        possiblePickups = MinigameDataUtils.GetPossiblePickups(_curGameMode);
        runeObtention = MinigameDataUtils.GetRuneInformation(_curGameMode);
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
