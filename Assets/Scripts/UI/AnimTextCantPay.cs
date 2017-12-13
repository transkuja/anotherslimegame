using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimTextCantPay : MonoBehaviour {
    Text text;
    Outline outline;
    Color outlineColor;
    bool scaleIncreasing = true;
    bool alphaIncreasing = true;

    [SerializeField]
    bool scaleOnly = false;

	void Start () {
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();
        outlineColor = outline.effectColor;
    }
	
	void Update () {
        if (!scaleOnly)
        {
            outlineColor.a += ((alphaIncreasing) ? Time.deltaTime : -Time.deltaTime);       
            outline.effectColor = outlineColor;

            if (outlineColor.a > 0.75f) alphaIncreasing = false;
            else if (outlineColor.a < 0.35f) alphaIncreasing = true;
        }

        text.transform.localScale = new Vector3(text.transform.localScale.x + ((scaleIncreasing) ? Time.deltaTime : -Time.deltaTime) / 4,
                                                    text.transform.localScale.y + ((scaleIncreasing) ? Time.deltaTime : -Time.deltaTime) / 4,
                                                    1);
        if (text.transform.localScale.x > 1.05f) scaleIncreasing = false;
        else if (text.transform.localScale.x < 0.95f) scaleIncreasing = true;
    }

}
