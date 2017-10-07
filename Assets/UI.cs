using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    Text evolutionText;

    private void Start()
    {
        GameManager.UiReference = this;
        evolutionText = GetComponentInChildren<Text>();
    }

    public void NeedUpdate(string value)
    {
        evolutionText.text = value;
    }
}
