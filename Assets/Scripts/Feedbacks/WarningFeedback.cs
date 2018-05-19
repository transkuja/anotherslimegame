using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningFeedback : MonoBehaviour {

    Image feedback;
    Color color;
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
        if (feedback == null)
            feedback = GetComponentInChildren<Image>();

        color = feedback.color;
        color.a = 0.5f;
        feedback.color = color;
        StartCoroutine(DisableFeedback());
    }

    private void Update()
    {
        if (alphaIncreasing)
            color.a += Time.deltaTime*2;
        else
            color.a -= Time.deltaTime*2;

        feedback.color = color;

        if (color.a > 0.99f) alphaIncreasing = false;
        else if (color.a < 0.45f) alphaIncreasing = true;
    }

    IEnumerator DisableFeedback()
    {
        yield return new WaitForSeconds(1.0f);
        transform.parent.gameObject.SetActive(false);

        bool skipTrapSetup = false;
        for (int i = 0; i < GameManager.Instance.CurrentGameMode.curNbPlayers; i++)
        {
            if (((ColorFloorGameMode)GameManager.Instance.CurrentGameMode).freeMovement)
            {
                if (transform.GetComponentInParent<OnColoredFloorTrigger>().GetFloorIndex() == ColorFloorHandler.playerCurrentPositionsFreeMovement[i])
                {
                    ColorFloorHandler.LosePoints(i);
                    skipTrapSetup = true;
                }
            }
            else
            {
                if (transform.GetComponentInParent<OnColoredFloorTrigger>().GetFloorIndex()
                    == ColorFloorHandler.restrainedGP.playerCurrentPositions[i].GetComponentInChildren<OnColoredFloorTrigger>().GetFloorIndex())
                {
                    ColorFloorHandler.LosePoints(i);
                    skipTrapSetup = true;
                }

            }
        }

        if (!skipTrapSetup)
        {
            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.ColorFloorPickUps, 1).GetItem(
                transform.GetComponentInParent<OnColoredFloorTrigger>().transform,
                Vector3.up * 1.5f,
                Quaternion.identity,
                true
            );
        }

    }
}
