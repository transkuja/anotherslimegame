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

    Vector3 originScale;
    float scaleAmp = 0.05f;

	void Start () {
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();
        outlineColor = outline.effectColor;
        originScale = transform.localScale;
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

        if (text.transform.localScale.x > originScale.x + scaleAmp) scaleIncreasing = false;
        else if (text.transform.localScale.x < originScale.x - scaleAmp) scaleIncreasing = true;
    }

    private void OnDestroy()
    {
        transform.localScale = originScale;
    }

}
