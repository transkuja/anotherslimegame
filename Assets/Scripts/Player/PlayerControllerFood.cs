using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UWPAndXInput;

public class PlayerControllerFood : PlayerController {
    private bool areInputsUnlocked = true;

    public float currentCombo = 1;
    public GameObject comboUI;
    Text comboUIText;
    [SerializeField]
    Image comboUIFillAreaImage;
    public Animator parentAnim;

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
            }

            // update combo
            comboUIText.text = currentCombo.ToString("0.0");
            comboUIFillAreaImage.fillAmount = ((currentCombo - 1) * 20) / 80.0f;
            if (comboUIFillAreaImage.fillAmount > 0.75f)
            {
                ActivateSkull();
            }
            else
                DeactivateSkull();
        }
    }

    void ActivateSkull()
    {
        comboUI.transform.GetChild(2).GetComponent<Image>().color = Color.red;
        if (comboUI.transform.GetChild(2).GetComponent<AnimButton>() != null)
            comboUI.transform.GetChild(2).GetComponent<AnimButton>().enabled = true;
    }

    void DeactivateSkull()
    {
        comboUI.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        if (comboUI.transform.GetChild(2).GetComponent<AnimButton>())
            comboUI.transform.GetChild(2).GetComponent<AnimButton>().enabled = false;
        comboUI.transform.GetChild(2).localScale = Vector3.one * 0.8f;
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
        }
    }

    private void Start()
    {
        CurrentCombo = 1.0f;
        parentAnim = GetComponentInParent<Animator>();
        foodInputSettings = GetComponent<FoodInputSettings>();
    }

    public override void Update () {

        if (GameManager.CurrentState == GameState.Normal)
        {
            if (AreInputsUnlocked)
            {
                CompareInput();
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

            if (foodInputSettings.CurrentInput == PossibleInputs.BadOne
                && foodInputSettings.CurrentTime > 0.2f)
            {
                GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                        = FaceEmotion.Loser;
                ((FoodGameMode)GameManager.Instance.CurrentGameMode).BadInput(this);

                Invoke("ResetStateAfterEatingSmthgBad", 0.5f);
            }
            else
            {
                StartCoroutine(ResetStateAfterWrongInput(_pressed));
                AreInputsUnlocked = false;
                GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                        = FaceEmotion.Hit;

                //((FoodGameMode)GameManager.Instance.CurrentGameMode).BadInput(this);
            }
        }
    }

    IEnumerator ResetStateAfterWrongInput(PossibleInputs _wrongInput)
    {
        yield return new WaitForSeconds(0.3f);

        AreInputsUnlocked = true;
        GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                = FaceEmotion.Neutral;
    }

    void ResetStateAfterEatingSmthgBad()
    {
        GameManager.Instance.PlayerStart.PlayersReference[(int)playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion
                = FaceEmotion.Neutral;
    }
}
