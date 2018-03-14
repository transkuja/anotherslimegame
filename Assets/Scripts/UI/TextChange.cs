using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextChange : MonoBehaviour {

    public PlayerUIStat type;

    public int[] associatedPlayers;

    public string prefix = "";
    public string suffix = "";

    public void Init(int _playerIndex)
    {
        associatedPlayers[0] = _playerIndex; // Ugly, is the foreach necessary @Rémi?
        foreach (int i in associatedPlayers)
        {
            {
                if(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>()!= null)
                {
                    if (GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().OnValuesChange != null)
                        GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().OnValuesChange[(int)type] += OnValueChange;
                }
            }
        }

    }

    public void OnValueChange(int _newValue)
    {
        GameManager.Instance.SpecificPlayerUI.OnValueChange(this);
        this.GetComponentInChildren<Text>().text = prefix + _newValue + suffix;
    }
}
