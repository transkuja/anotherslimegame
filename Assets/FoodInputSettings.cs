using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodInputSettings : MonoBehaviour {
    float inputSpeed;
    float targetSlotPosition;
    public float targetPositionError;
    PlayerControllerFood target;
    Slider tail;
    float tailInternValue = 0.0f;

    // 3 behaviours for "tail"
    // -- Attached && tail length < window => reduce tail length over time
    // -- Attached && tail length > window => store tail length, decrease the value but not the visual until it reaches the window size
    // -- Detached => increase tail length until it reaches its max value or the socket
    public PossibleInputs associatedInput;
    bool isAttached = false;
    bool nextInputCalled = false;

	void Start () {
        inputSpeed = ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputSpeed;
        associatedInput = (PossibleInputs)Random.Range(0, (int)PossibleInputs.Size);
        transform.GetChild(1).GetComponent<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetSpriteFromInput(associatedInput);
        target = transform.parent.parent.GetComponentInChildren<PlayerControllerFood>();
        targetSlotPosition = target.transform.position.x;
        tail = GetComponentInChildren<Slider>();
        tail.value = 0.0f;
        tailInternValue = 0.0f;
        tail.maxValue = Random.Range(((FoodGameMode)GameManager.Instance.CurrentGameMode).miniTail, ((FoodGameMode)GameManager.Instance.CurrentGameMode).maxTail);
        tail.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tail.maxValue);
    }
	
	void Update () {
        if (!isAttached)
        {
            transform.position += Time.deltaTime * inputSpeed * Vector3.right;
            if (tailInternValue < 10.0f)
            {
                tailInternValue += Time.deltaTime * (inputSpeed + 5);
            }
            else
            {
                tail.value += Time.deltaTime * (inputSpeed + 5);
                tailInternValue = tail.value + 10.0f;
            }
            if (tail.value >= tail.maxValue && !nextInputCalled)
            {
                ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputTracksHandler.SendNextInput((int)target.playerIndex);
                nextInputCalled = true;
            }

            if (transform.position.x >= targetSlotPosition - targetPositionError)
            {
                transform.SetParent(target.transform);
                transform.localPosition = Vector3.zero;
                isAttached = true;
                // TODO: feedback YOUCANPRESSTHEBUTTONNOW
                target.currentInput = associatedInput;
            }

        }
        else
        {
            if (tailInternValue >= tail.maxValue)
            {
                if (!nextInputCalled)
                {
                    ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputTracksHandler.SendNextInput((int)target.playerIndex);
                    nextInputCalled = true;
                }
                tail.value -= Time.deltaTime * inputSpeed;
                if (tail.value <= 0.0f)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                tailInternValue += Time.deltaTime * (inputSpeed);
            }
        }
    }
}
