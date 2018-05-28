using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodInputSettings : MonoBehaviour {
    float currentTime;
    float timeBeforeNextInput;

    private PossibleInputs currentInput;
    public PossibleInputs nextInput;

    bool nextInputCalled = false;
    int maxRandom;
    bool areBadInputsEnabled = false;
    bool scaleAscending = true;

    bool isInitialized = false;
    float reactionTime;

    [SerializeField]
    Sprite redFeedback;
    [SerializeField]
    Sprite yellowFeedback;

    public float badInputChanceMin = 10.0f;
    float badInputChance;

    public PossibleInputs CurrentInput
    {
        get
        {
            return currentInput;
        }

        set
        {
            currentInput = value;

            currentTime = 0.0f;
            if (currentInput != PossibleInputs.BadOne)
            {
                transform.GetChild((int)currentInput).GetComponent<Image>().enabled = true;
                transform.GetChild((int)currentInput).GetChild(0).GetComponent<Image>().color = Color.white;
                timeBeforeNextInput = Random.Range(((FoodGameMode)GameManager.Instance.CurrentGameMode).miniTail, ((FoodGameMode)GameManager.Instance.CurrentGameMode).maxTail);
            }
            else
            {
                timeBeforeNextInput = Random.Range(0.5f, 0.9f) + 0.2f;
            }

            nextInput = GetRandomInput();
        }
    }

    public float CurrentTime
    {
        get
        {
            return currentTime;
        }
    }

    public PossibleInputs GetRandomInput()
    {
        PossibleInputs result;

        if (areBadInputsEnabled)
        {
            if (Random.Range(0, 100) < badInputChance)
            {
                result = PossibleInputs.BadOne;
                badInputChance = badInputChanceMin;
            }
            else
            {
                if (badInputChance < badInputChanceMin)
                    badInputChance = badInputChanceMin;

                badInputChance += 5.0f;
                result = (PossibleInputs)Random.Range(0, 4);
            }

        }       
        else
        {
            result = (PossibleInputs)Random.Range(0, 4);
        }

        if (result == currentInput)
            return (PossibleInputs)((int)(result + 1) % maxRandom);
        else
            return result;
    }

    public void Init()
    {
        areBadInputsEnabled = ((FoodGameMode)GameManager.Instance.CurrentGameMode).enableBadInputs;
        reactionTime = ((FoodGameMode)GameManager.Instance.CurrentGameMode).reactionTime;
        maxRandom = (int)PossibleInputs.Size;
        if (!areBadInputsEnabled)
        {
            maxRandom--;
        }

        badInputChance = 0.0f;
        CurrentInput = GetRandomInput();

        isInitialized = true;
    }

    void SwitchInput()
    {
        ActiveButton(nextInput);
        CurrentInput = nextInput;
        newColorIncomingAnim = new Color(1, 1, 1, 0.5f);
    }

    bool reverseLerpIncomingAnim = false;
    float lerpParamIncomingAnim = 0.0f;
    Color newColorIncomingAnim = new Color(1, 1, 1, 0.5f);
    float currentScale = 1.0f;

    void Update () {
        if (GameManager.CurrentState != GameState.Normal)
            return;

        if (currentTime >= timeBeforeNextInput)
        {
            SwitchInput();
        }
        else
            currentTime += Time.deltaTime;

        if (isInitialized)
        {
            // scale oscille entre 0.75 et 0.95
            if (currentScale > 0.95f && scaleAscending)
                scaleAscending = false;
            if (currentScale < 0.75f)
                scaleAscending = true;
            if (currentInput != PossibleInputs.BadOne)
            {
                transform.GetChild((int)currentInput).localScale += ((scaleAscending) ? 0.5f : -0.5f) * Time.deltaTime * Vector3.one;
                currentScale = transform.GetChild((int)currentInput).localScale.x;
            }
        }
    }

    public void ActiveButton(PossibleInputs _toActivate)
    {
        if (_toActivate != PossibleInputs.BadOne)
        {
            transform.GetChild((int)_toActivate).GetComponent<Image>().enabled = true;
            transform.GetChild((int)_toActivate).GetComponent<Image>().color = Color.white;
            transform.GetChild((int)_toActivate).GetChild(0).GetComponent<Image>().color = Color.white;
        }
        else
        {
            for (int i = 0; i < (int)PossibleInputs.Size - 1; i++)
            {
                transform.GetChild(i).GetComponent<Image>().enabled = true;
                transform.GetChild(i).GetComponent<Image>().sprite = redFeedback;
            }
        }

        if (CurrentInput != PossibleInputs.BadOne)
        {
            if (_toActivate != PossibleInputs.BadOne)
                transform.GetChild((int)CurrentInput).GetComponent<Image>().enabled = false;
            transform.GetChild((int)CurrentInput).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            transform.GetChild((int)CurrentInput).localScale = Vector3.one;
        }
        else
        {
            for (int i = 0; i < (int)PossibleInputs.Size - 1; i++)
            {
                transform.GetChild(i).GetComponent<Image>().enabled = false;
                transform.GetChild(i).GetComponent<Image>().sprite = yellowFeedback;
            }
        }
    }
}
