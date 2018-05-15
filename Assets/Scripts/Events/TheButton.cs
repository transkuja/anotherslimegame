using System.Collections;
using System.Collections.Generic;
using UWPAndXInput;
using UnityEngine;


public enum WaterState { Clear, WaterIsMovingTop, WaterIsMovingBottom}

public class TheButton : MonoBehaviour {


    public GameObject water;

    [Range(0.01f, 0.5f)]
    public float speed = 0.01f;

    [Header("height + water.position")]
    public float heightToReach = 30;

    private Vector3 positionToReach;
    private Vector3 lerpStartValue;
    private float lerpValue;
    private float lastValue;
   
    public float timerResetMax = 20.0f;
    private float timerReset = 0.0f;

    public void Start()
    {
        positionToReach = water.transform.position + heightToReach * Vector3.up;
        lerpValue = 0.0f;
        lastValue = 0.0f;
        lerpStartValue = water.transform.position;
    }

    public void Update()
    {
        switch (HUBManager.instance.WaterState)
        {
            case WaterState.WaterIsMovingTop:
                MoveWater(lerpStartValue, positionToReach);
                if (lerpValue >= 1.0f)
                {
                    timerReset += Time.deltaTime;
                    if (timerReset >= timerResetMax)
                    {
                        timerReset = 0.0f;
                        lerpValue = 0.0f;
                        HUBManager.instance.WaterState = WaterState.WaterIsMovingBottom;
                    }
                }
                break;
            case WaterState.WaterIsMovingBottom:
                MoveWater(positionToReach, lerpStartValue);
                if (lerpValue >= 1.0f)
                {
                    HUBManager.instance.WaterState = WaterState.Clear;
                }
                break;
            case WaterState.Clear:

                break;
        }

    }

    public void OnDisable()
    {
        for (int i = 0; i < 4; i++)
            GamePad.SetVibration((PlayerIndex)i, 0, 0);
    }

    public void MoveWater(Vector3 positionOrigin, Vector3 positionToReach)
    {
        lerpValue += speed * Time.deltaTime;
        water.transform.position = Vector3.Lerp(positionOrigin, positionToReach, lerpValue);

        if (lerpValue >= 1.0f)
        {
            for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
                GamePad.SubVibration((PlayerIndex)i, lastValue, lastValue);
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
            {
                GamePad.SubVibration((PlayerIndex)i, lastValue, lastValue);
                lastValue = Mathf.Lerp(0, 0.5f, lerpValue);
                GamePad.AddVibration((PlayerIndex)i, lastValue, lastValue);
            }
        }
    }
}
