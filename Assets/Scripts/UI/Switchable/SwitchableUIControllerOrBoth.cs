using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchableUIControllerOrBoth : MonoBehaviour {
    bool hasBeenForced = false;
    void OnEnable()
    {
        if (hasBeenForced)
            return;

        transform.GetChild(0).gameObject.SetActive(!Controls.IsKeyboardUsed());
        transform.GetChild(1).gameObject.SetActive(Controls.IsKeyboardUsed());
    }

    public void ForceOneImageOnly()
    {
        hasBeenForced = true;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

}
