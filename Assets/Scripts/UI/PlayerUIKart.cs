using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIKart : APlayerUI
{

    public override void Init()
    {
        base.Init();
        UpdateSpriteColorBasedOnSelection();
    }


    public override void OnValueChange(TextChange text)
    {
        text.suffix = "/" + ((KartGameMode)GameManager.Instance.CurrentGameMode).NumberOfLaps;  
    }

    private void UpdateSpriteColorBasedOnSelection()
    {
        if (SlimeDataContainer.instance != null)
        {
            //UIref.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = (SlimeDataContainer.instance.colorFadeSelected[i] ? SlimeDataContainer.instance.selectedColors[i] : Color.white);

            // TMP: Should use color from slimedatacontainer instead
            switch (GameManager.Instance.PlayerStart.ActivePlayersAtStart)
            {
                case 1:
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
                    break;
                case 2:
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
                    UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
                    break;
                case 3:
                    UIrefLeft.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
                    UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
                    break;
                default:
                    UIrefLeft.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
                    UIrefLeft.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
                    UIrefRight.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.white;
                    UIrefRight.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = Color.white;
                    break;
            }
        }
    }
}
