using UnityEngine;
using UnityEngine.UI;

public class SwitchUIBasedOnId : MonoBehaviour {

    [SerializeField]
    KeyboardControlType keyboardInput;

    [SerializeField]
    int playerIndex;

    private void OnEnable()
    {
        if (playerIndex == Controls.keyboardIndex)
            transform.GetComponent<Image>().sprite = ResourceUtils.Instance.spriteUtils.GetKeyboardControlSprite(keyboardInput);
    }
}
