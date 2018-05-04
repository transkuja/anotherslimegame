﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public class PlayerControllerFood : PlayerController {
    public PossibleInputs currentInput;
    public bool areInputsUnlocked = true;
    public bool hasEatenSmthgBad = false;

    public int currentCombo = 0;
    public GameObject comboUI;

    public int CurrentCombo
    {
        get
        {
            return currentCombo;
        }

        set
        {
            currentCombo = value;

            // update combo
            comboUI.GetComponent<Text>().text = "X " + currentCombo;
        }
    }

    private void Start()
    {
        CurrentCombo = 0;
    }

    public override void Update () {
        base.Update();

        if (GameManager.CurrentState == GameState.Normal)
        {
            if (areInputsUnlocked && !hasEatenSmthgBad)
                CompareInput();
        }
    }

    void CompareInput()
    {
        if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
        {
            CheckInput(PossibleInputs.A);
        }

        if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed)
        {
            CheckInput(PossibleInputs.B);
        }

        if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed)
        {
            CheckInput(PossibleInputs.X);
        }

        if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
        {
            CheckInput(PossibleInputs.Y);
        }

    }

    void CheckInput(PossibleInputs _pressed)
    {
        if (_pressed == currentInput)
            ((FoodGameMode)GameManager.Instance.CurrentGameMode).GoodInput(this);
        else
        {
            CurrentCombo = 0;
            if (currentInput == PossibleInputs.BadOne)
            {
                hasEatenSmthgBad = true;
                GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                        = FaceEmotion.Loser;
                Invoke("ResetStateAfterEatingSmthgBad", 1.5f);
            }
        }
    }

    void ResetStateAfterEatingSmthgBad()
    {
        hasEatenSmthgBad = false;
        GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                = FaceEmotion.Neutral;
    }

    public void UpdateCurrentInput(PossibleInputs _newInput)
    {
        currentInput = _newInput;
    }
}
