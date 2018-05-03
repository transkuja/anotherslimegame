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

    // 3 behaviours for "tail"
    // -- Attached && tail length < window => reduce tail length over time
    // -- Attached && tail length > window => store tail length, decrease the value but not the visual until it reaches the window size
    // -- Detached => increase tail length until it reaches its max value or the socket
    public PossibleInputs associatedInput;
    bool isAttached = false;
    bool nextInputCalled = false;

	public void Init () {
        inputSpeed = ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputSpeed;
        associatedInput = (PossibleInputs)Random.Range(0, (int)PossibleInputs.Size);
        transform.GetComponentInChildren<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetSpriteFromInput(associatedInput);
        target = transform.parent.parent.GetComponentInChildren<PlayerControllerFood>();
        targetSlotPosition = target.transform.position.x;
        currentTime = 0.0f;
        timeBeforeNextInput = Random.Range(((FoodGameMode)GameManager.Instance.CurrentGameMode).miniTail, ((FoodGameMode)GameManager.Instance.CurrentGameMode).maxTail);
    }
	
	void Update () {
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

            if (transform.position.x >= targetSlotPosition - targetPositionError)
            {
                if (target.transform.childCount > 0)
                    Destroy(target.transform.GetChild(0).gameObject);

                transform.SetParent(target.transform);
                transform.localPosition = Vector3.zero;
                isAttached = true;
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
        }
        return Color.white;
    }

    public void StartGame()
    {
        Init();
        transform.SetParent(target.transform);
        transform.localPosition = Vector3.zero;
        isAttached = true;
        // TODO: feedback YOUCANPRESSTHEBUTTONNOW
        target.currentInput = associatedInput;
        transform.parent.parent.GetChild(0).GetComponentInChildren<Image>().color = GetColorFromInput();
    }
}
