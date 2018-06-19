using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchableUIControllerOrBoth : MonoBehaviour {

    void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(!Controls.IsKeyboardUsed());
        transform.GetChild(1).gameObject.SetActive(Controls.IsKeyboardUsed());
    }

}
