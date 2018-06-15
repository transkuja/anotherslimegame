using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomButtonUI : MonoBehaviour {

    void OnEnable()
    {
        if (Controls.nbPlayersSelectedInMenu == 1)
        {
            transform.GetChild(0).gameObject.SetActive(Controls.IsKeyboardUsed());
            transform.GetChild(1).gameObject.SetActive(!Controls.IsKeyboardUsed());
            transform.GetChild(0).GetComponentInChildren<Image>().sprite = ResourceUtils.Instance.spriteUtils.keyboardR;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(!Controls.IsKeyboardUsed());
            transform.GetChild(0).GetComponentInChildren<Image>().sprite = ResourceUtils.Instance.spriteUtils.XButtonSprite;
            transform.GetChild(1).gameObject.SetActive(Controls.IsKeyboardUsed());
        }
    }
}
