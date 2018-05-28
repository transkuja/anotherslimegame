using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorPlayerId : MonoBehaviour {

    public void Init()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; ++i)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
        }
    }

    public void DisableCursors()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; ++i)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Disable(Player p)
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; ++i)
        {
            if(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>() == p)
                transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
    }

}
