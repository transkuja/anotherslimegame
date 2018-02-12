using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPushGame : APlayerUI {

    public override void Init()
    {
        base.Init();        
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            if (SlimeDataContainer.instance != null)
                //UIref.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = (SlimeDataContainer.instance.colorFadeSelected[i] ? SlimeDataContainer.instance.selectedColors[i] : Color.white);
                // TMP
                UIref.transform.GetChild(i).GetChild(0).GetComponent<Image>().color = Color.white;

            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().CallOnValueChange(PlayerUIStat.Points, 0);
        }

    }
}
