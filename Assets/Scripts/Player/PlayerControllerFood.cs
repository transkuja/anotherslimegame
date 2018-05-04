using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public class PlayerControllerFood : PlayerController {
    public PossibleInputs currentInput;
    private bool areInputsUnlocked = true;
    private bool hasEatenSmthgBad = false;

    public int currentCombo = 0;
    public GameObject comboUI;
    Animator parentAnim;

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

    public bool AreInputsUnlocked
    {
        get
        {
            return areInputsUnlocked;
        }

        set
        {
            areInputsUnlocked = value;
            if (!areInputsUnlocked)
                parentAnim.SetBool("wrong", true);
        }
    }

    public bool HasEatenSmthgBad
    {
        get
        {
            return hasEatenSmthgBad;
        }

        set
        {
            hasEatenSmthgBad = value;
            if (hasEatenSmthgBad)
                parentAnim.SetBool("wrong", true);
        }
    }

    private void Start()
    {
        CurrentCombo = 0;
        parentAnim = GetComponentInParent<Animator>();
    }

    public override void Update () {
        base.Update();

        if (GameManager.CurrentState == GameState.Normal)
        {
            if (AreInputsUnlocked && !HasEatenSmthgBad)
            {
                parentAnim.SetBool("wrong", false);
                CompareInput();
            }
            else
            {
                parentAnim.SetBool("wrong", true);

            }
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
        {
            ((FoodGameMode)GameManager.Instance.CurrentGameMode).GoodInput(this);
            if (AudioManager.Instance != null && AudioManager.Instance.positiveSoundFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.incorrectFx);
        }
        else
        {
            CurrentCombo = 0;
            if (AudioManager.Instance != null && AudioManager.Instance.incorrectFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.incorrectFx);

            if (currentInput == PossibleInputs.BadOne)
            {
                HasEatenSmthgBad = true;
                GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                        = FaceEmotion.Loser;
                Invoke("ResetStateAfterEatingSmthgBad", 1.5f);
            }
        }
    }

    void ResetStateAfterEatingSmthgBad()
    {
        HasEatenSmthgBad = false;
        GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                = FaceEmotion.Neutral;
    }

    public void UpdateCurrentInput(PossibleInputs _newInput)
    {
        currentInput = _newInput;
    }
}
