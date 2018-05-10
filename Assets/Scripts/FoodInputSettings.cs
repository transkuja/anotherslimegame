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

    public PossibleInputs CurrentInput
    {
        get
        {
            return currentInput;
        }

        set
        {
            if (currentInput == PossibleInputs.BadOne)
                transform.GetChild((int)currentInput).GetChild(0).gameObject.SetActive(true);

            currentInput = value;
            transform.GetChild((int)currentInput).GetComponent<Image>().enabled = true;
            transform.GetChild((int)currentInput).GetChild(0).GetComponent<Image>().color = Color.white;

            currentTime = 0.0f;
            if (currentInput != PossibleInputs.BadOne)
                timeBeforeNextInput = Random.Range(((FoodGameMode)GameManager.Instance.CurrentGameMode).miniTail, ((FoodGameMode)GameManager.Instance.CurrentGameMode).maxTail);
            else
            {
                timeBeforeNextInput = 0.0f;
                transform.GetChild((int)currentInput).GetChild(0).gameObject.SetActive(false);
            }

            nextInput = GetRandomInput();
        }
    }

    public PossibleInputs GetRandomInput(bool _firstOne = false)
    {
        PossibleInputs result;
        if (!_firstOne)
        {
            result = (PossibleInputs)Random.Range(0, maxRandom);
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
            transform.GetChild((int)PossibleInputs.BadOne).gameObject.SetActive(false);
        }

        CurrentInput = GetRandomInput(true);

        isInitialized = true;
    }

    void SwitchInput()
    {
        ActiveButton(currentInput);
        CurrentInput = nextInput;
        nextInputCalled = false;
        reverseLerpIncomingAnim = false;
        lerpParamIncomingAnim = 0.0f;
        newColorIncomingAnim = new Color(1, 1, 1, 0.5f);
    }

    bool reverseLerpIncomingAnim = false;
    float lerpParamIncomingAnim = 0.0f;
    Color newColorIncomingAnim = new Color(1, 1, 1, 0.5f);
    float currentScale = 1.0f;

    void Update () {
        if (GameManager.CurrentState != GameState.Normal)
            return;

        if (!nextInputCalled && currentTime >= timeBeforeNextInput)
        {
            Invoke("SwitchInput", reactionTime);
            nextInputCalled = true;
        }
        else
            currentTime += Time.deltaTime;

        if (nextInputCalled)
        {
            // 0.5 -> 0.9 en reactionTime/3 * 2 
            // 0.9 -> 0.5 en reactionTime/3 * 2
            if (lerpParamIncomingAnim >= 1.0f || lerpParamIncomingAnim <= 0.0f)
                reverseLerpIncomingAnim = !reverseLerpIncomingAnim;

            lerpParamIncomingAnim = Mathf.Clamp(lerpParamIncomingAnim + ((reverseLerpIncomingAnim) ? -Time.deltaTime : Time.deltaTime) * (6 / reactionTime), 0, 1.0f);
            newColorIncomingAnim = new Color(1, 1, 1, Mathf.Lerp(0.25f, 0.9f, lerpParamIncomingAnim));
            transform.GetChild((int)nextInput).GetChild(0).GetComponent<Image>().color = newColorIncomingAnim;
        }

        if (isInitialized)
        {
            // scale oscille entre 0.75 et 0.95
            if (currentScale > 0.95f && scaleAscending)
                scaleAscending = false;
            if (currentScale < 0.75f)
                scaleAscending = true;
            transform.GetChild((int)currentInput).localScale += ((scaleAscending) ? 0.5f : -0.5f) * Time.deltaTime * Vector3.one;
            currentScale = transform.GetChild((int)currentInput).localScale.x;
        }
    }

    public void ActiveButton(PossibleInputs _toActivate)
    {
        transform.GetChild((int)_toActivate).GetComponent<Image>().enabled = true;
        transform.GetChild((int)_toActivate).GetChild(0).GetComponent<Image>().color = Color.white;

        transform.GetChild((int)CurrentInput).GetComponent<Image>().enabled = false;
        transform.GetChild((int)CurrentInput).GetChild(0).GetComponent<Image>().color = new Color(1,1,1,0.5f);
        transform.GetChild((int)CurrentInput).localScale = Vector3.one;
    }
}
