using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodInputSettings : MonoBehaviour {

    // 2 states, Attached/NotAttached
    // 3 behaviours for "tail"
    // -- Attached && tail length < window => reduce tail length over time
    // -- Attached && tail length > window => store tail length, decrease the value but not the visual until it reaches the window size
    // -- Detached => increase tail length until it reaches its max value or the socket
    public PossibleInputs associatedInput;

    public void Init(PossibleInputs _associatedInput)
    {

    }

	void Start () {
        associatedInput = (PossibleInputs)Random.Range(0, (int)PossibleInputs.Size);
        transform.GetChild(1).GetComponent<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetSpriteFromInput(associatedInput);
    }
	
	void Update () {
		
	}
}
