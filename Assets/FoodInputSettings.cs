using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodInputSettings : MonoBehaviour {
    float inputSpeed;
    float targetSlotPosition;
    public float targetPositionError;
    PlayerControllerFood target;

    // 2 states, Attached/NotAttached
    // 3 behaviours for "tail"
    // -- Attached && tail length < window => reduce tail length over time
    // -- Attached && tail length > window => store tail length, decrease the value but not the visual until it reaches the window size
    // -- Detached => increase tail length until it reaches its max value or the socket
    public PossibleInputs associatedInput;
    bool isAttached = false;

	void Start () {
        inputSpeed = ((FoodGameMode)GameManager.Instance.CurrentGameMode).inputSpeed;
        associatedInput = (PossibleInputs)Random.Range(0, (int)PossibleInputs.Size);
        transform.GetChild(1).GetComponent<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetSpriteFromInput(associatedInput);
        target = transform.parent.parent.GetComponentInChildren<PlayerControllerFood>();
        targetSlotPosition = target.transform.position.x;
    }
	
	void Update () {
        if (!isAttached)
        {
            transform.position += Time.deltaTime * inputSpeed * Vector3.right;
            if (transform.position.x >= targetSlotPosition - targetPositionError)
            {
                transform.position += (targetSlotPosition - transform.position.x) * Vector3.right;
                isAttached = true;
                // TODO: feedback YOUCANPRESSTHEBUTTONNOW
                target.currentInput = associatedInput;
            }

        }
    }
}
