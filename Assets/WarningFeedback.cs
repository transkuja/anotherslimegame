using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningFeedback : MonoBehaviour {

    Image feedback;
    Color color;
    float sinValue;
    float maxTimer = 2.0f;

    private void Start()
    {
        feedback = GetComponentInChildren<Image>();
        color = feedback.color;
        color.a = 0.0f;
        feedback.color = color;
    }

    void OnEnable() {
        color = feedback.color;
        color.a = 0.0f;
        feedback.color = color;
        sinValue = 0.0f;
    }

    private void Update()
    {
        sinValue += Time.deltaTime;
        color.a = (Mathf.Sin(sinValue) + 1) / 2;
        feedback.color = color;

        if (sinValue > 2.0f)
            transform.parent.gameObject.SetActive(false);

    }
}
