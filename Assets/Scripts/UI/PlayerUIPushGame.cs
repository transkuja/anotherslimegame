using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPushGame : PlayerUI {

    public override void Init()
    {
        //base.Init();
        PlayerStart playerStart = FindObjectOfType<PlayerStart>();

        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameObject playerUi = Instantiate(prefabPlayerUI[i], transform);
            Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointPushGame, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
            linkPlayerPointsToItsUi.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerUi.transform.GetChild((int)PlayerUiChildren.Points).gameObject);
            playerUi.transform.Find("Life").gameObject.SetActive(false);
            linkPlayerLifesToItsUi.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerUi.transform.Find("Life").gameObject);
            RefreshPointsPlayerUi(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), 0, i);
            playerUi.SetActive(true);
            Image imagePlayer = playerUi.transform.Find("Points").GetComponentInChildren<Image>();
            if (i>0)
            {
                //imagePlayer.color = 
                Color col = playerStart.colorPlayer[i-1];
                col.a = 1;
                imagePlayer.color = col;
            }
        }



        // Initialize points
       

    }

    public override void RefreshPointsPlayerUi(Player player, int _newValue, int index)
    {
        //base.RefreshPointsPlayerUi(player, _newValue, index);
        Transform toRefresh = linkPlayerPointsToItsUi[player].transform;
        toRefresh.GetComponentInChildren<Text>().text = " X " + _newValue;
    }
}
