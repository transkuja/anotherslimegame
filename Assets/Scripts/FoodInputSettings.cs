using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodInputSettings : MonoBehaviour {
    float inputSpeed;
    float targetSlotPosition;
    public float targetPositionError;
    PlayerControllerFood target;
    float currentTime;
    float timeBeforeNextInput;

    public PossibleInputs associatedInput;
    bool isAttached = false;
    bool nextInputCalled = false;
    int maxRandom;
    bool areBadInputsEnabled = false;
    bool scaleAscending = true;

    public void Init (bool _firstOne = false) {
        areBadInputsEnabled = ((FoodGameMode)GameManager.Instance.CurrentGameMode).enableBadInputs;

        maxRandom = (int)PossibleInputs.Size;
        if (!areBadInputsEnabled)
            maxRandom--;

        inputSpeed = ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputSpeed;
        if (!_firstOne)
            associatedInput = (PossibleInputs)Random.Range(0, maxRandom);
        else
            associatedInput = (PossibleInputs)Random.Range(0, 4);

        transform.GetComponentInChildren<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetSpriteFromInput(associatedInput);
        target = transform.parent.parent.GetComponentInChildren<PlayerControllerFood>();
        targetSlotPosition = target.transform.position.x;
        currentTime = 0.0f;
        if (associatedInput != PossibleInputs.BadOne)
            timeBeforeNextInput = Random.Range(((FoodGameMode)GameManager.Instance.CurrentGameMode).miniTail, ((FoodGameMode)GameManager.Instance.CurrentGameMode).maxTail);
        else
        {
            timeBeforeNextInput = 0.5f;
            transform.GetComponentInChildren<Image>().color = Color.red;
        }
    }

    void Update () {
        if (GameManager.CurrentState != GameState.Normal)
            return;

        if (!nextInputCalled && currentTime >= timeBeforeNextInput)
        {
            ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputTracksHandler.SendNextInput((int)target.playerIndex);
            nextInputCalled = true;
        }
        else
            currentTime += Time.deltaTime;

        if (!isAttached)
        {
            transform.position += Time.deltaTime * inputSpeed * Vector3.right;
            // scale oscille entre 0.75 et 0.95
            if (transform.localScale.x > 0.95f && scaleAscending)
                scaleAscending = false;
            if (transform.localScale.x < 0.75f)
                scaleAscending = true;
            transform.localScale += ((scaleAscending) ? 0.5f : -0.5f) * Time.deltaTime * Vector3.one;

            if (transform.position.x >= targetSlotPosition - targetPositionError)
            {
                if (target.transform.childCount > 0)
                    Destroy(target.transform.GetChild(0).gameObject);

                transform.SetParent(target.transform);
                transform.localPosition = Vector3.zero;
                isAttached = true;
                transform.localScale = Vector3.one;
                // TODO: feedback YOUCANPRESSTHEBUTTONNOW
                target.currentInput = associatedInput;
                transform.parent.parent.GetChild(0).GetComponentInChildren<Image>().color = GetColorFromInput();

            }
        }
    }

    Color GetColorFromInput()
    {
        switch (associatedInput)
        {
            case PossibleInputs.A:
                return Color.green;
            case PossibleInputs.B:
                return Color.red;
            case PossibleInputs.Y:
                return Color.yellow;
            case PossibleInputs.X:
                return new Color(0, 128, 255, 255);
            case PossibleInputs.BadOne:
                return Color.grey;
        }
        return Color.white;
    }

    public void StartGame()
    {
        Init(true);
        transform.SetParent(target.transform);
        transform.localPosition = Vector3.zero;
        isAttached = true;
        // TODO: feedback YOUCANPRESSTHEBUTTONNOW
        target.currentInput = associatedInput;
        transform.parent.parent.GetChild(0).GetComponentInChildren<Image>().color = GetColorFromInput();
    }
}
