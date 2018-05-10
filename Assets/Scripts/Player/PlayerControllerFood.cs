using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public class PlayerControllerFood : PlayerController {
    private bool areInputsUnlocked = true;
    private bool hasEatenSmthgBad = false;

    public float currentCombo = 1;
    public GameObject comboUI;
    Text comboUIText;
    Slider comboUISlider;
    Image comboUIFillAreaImage;
    Animator parentAnim;

    public FoodInputSettings foodInputSettings;

    public float CurrentCombo
    {
        get
        {
            return currentCombo;
        }

        set
        {
            currentCombo = value;

            if (comboUIText == null)
            {
                comboUIText = comboUI.GetComponentInChildren<Text>();
                comboUISlider = comboUI.GetComponentInChildren<Slider>();
                comboUIFillAreaImage = comboUISlider.fillRect.transform.GetComponent<Image>();
            }

            // update combo
            comboUIText.text = "x " + currentCombo.ToString("0.0");
            comboUISlider.value = (currentCombo - 1) * 20;
            comboUIFillAreaImage.color = (currentCombo > 4.0f) ? Color.red : (currentCombo > 2.5f) ? (new Color(255, 174, 0) / 255) : Color.green;
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
        CurrentCombo = 1.0f;
        parentAnim = GetComponentInParent<Animator>();
        foodInputSettings = GetComponent<FoodInputSettings>();
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
        if (_pressed == foodInputSettings.CurrentInput)
        {
            ((FoodGameMode)GameManager.Instance.CurrentGameMode).GoodInput(this);
            if (AudioManager.Instance != null && AudioManager.Instance.positiveSoundFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.incorrectFx);
        }
        else
        {

            if (AudioManager.Instance != null && AudioManager.Instance.incorrectFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.incorrectFx);

            if (foodInputSettings.CurrentInput == PossibleInputs.BadOne)
            {
                HasEatenSmthgBad = true;
                GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                        = FaceEmotion.Loser;
                Invoke("ResetStateAfterEatingSmthgBad", 1.5f);
            }
            else
            {
                Invoke("ResetStateAfterWrongInput", 0.5f);
                AreInputsUnlocked = false;
                GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                        = FaceEmotion.Hit;
            }
        }
    }

    void ResetStateAfterWrongInput()
    {
        AreInputsUnlocked = true;
        GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                = FaceEmotion.Neutral;
    }

    void ResetStateAfterEatingSmthgBad()
    {
        HasEatenSmthgBad = false;
        GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                = FaceEmotion.Neutral;
    }
}
