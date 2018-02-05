using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUBManager : MonoBehaviour
{
    public static HUBManager instance;

    public WaterState WaterState = WaterState.Clear;

    public void Awake()
    {
        instance = this;
    }


    // HUB events
    public void StartIncreasing()
    {
        WaterState = WaterState.WaterIsMovingTop;
    }

}
