using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningFeedback : MonoBehaviour {

    Image feedback;
    Color color;
    float currentTimer = 0.0f;
    float maxTimer = 2.0f;
    bool alphaIncreasing = true;

    private void Start()
    {
        feedback = GetComponentInChildren<Image>();
        color = feedback.color;
        color.a = 0.5f;
        feedback.color = color;
    }

    void OnEnable() {
        color = feedback.color;
        color.a = 0.5f;
        feedback.color = color;
        currentTimer = 0.0f;
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;
        if (alphaIncreasing)
            color.a += Time.deltaTime*2;
        else
            color.a -= Time.deltaTime*2;

        feedback.color = color;

        if (currentTimer > 1.0f)
        {
            transform.parent.gameObject.SetActive(false);

            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorPickUps, 1).GetItem(
                transform.GetComponentInParent<OnColoredFloorTrigger>().transform,
                Vector3.up * 1.5f,
                Quaternion.identity,
                true
            );
        }

        if (color.a > 0.99f) alphaIncreasing = false;
        else if (color.a < 0.45f) alphaIncreasing = true;
    }
}
