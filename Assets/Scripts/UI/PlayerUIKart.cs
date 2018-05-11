using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIKart : APlayerUI
{

    public override void Init()
    {
        base.Init();
    }


    public override void OnValueChange(TextChange text)
    {
        text.suffix = "/" + ((KartGameMode)GameManager.Instance.CurrentGameMode).NumberOfLaps;  
    }
}
