using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimText : MonoBehaviour {
    Text text;
    Outline outline;
    Color outlineColor;
    bool scaleIncreasing = true;
    bool alphaIncreasing = true;

	void Start () {
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();
        outlineColor = outline.effectColor;
    }
	
	void Update () {
        outlineColor.a += ((alphaIncreasing) ? Time.deltaTime : -Time.deltaTime);
        text.transform.localScale = new Vector3(text.transform.localScale.x + ((scaleIncreasing) ? Time.deltaTime : -Time.deltaTime)/4,
                                                text.transform.localScale.y + ((scaleIncreasing) ? Time.deltaTime : -Time.deltaTime)/4,
                                                1);
        outline.effectColor = outlineColor;

        if (outlineColor.a > 0.3f) alphaIncreasing = false;
        else if (outlineColor.a < 0.01f) alphaIncreasing = true;
        if (text.transform.localScale.x > 1.05f) scaleIncreasing = false;
        else if (text.transform.localScale.x < 0.95f) scaleIncreasing = true;
    }
}
