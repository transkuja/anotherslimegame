using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPushGame : APlayerUI {

    public override void Init()
    {
        base.Init();        
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().CallOnValueChange(PlayerUIStat.Points, 0);
        }
    }

 
}
