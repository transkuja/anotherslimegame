using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TextChange : MonoBehaviour {

    public PlayerUIStat type;

    public int[] associatedPlayers;

    private TextChange instance;
    public GameObject originalState;

    public void Start()
    {
        instance = this;
        originalState = gameObject;
    }

    public void Init()
    {
        foreach (int i in associatedPlayers)
        {
            if (GameManager.Instance.PlayerStart.PlayersReference[i] != null)
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
        GameManager.Instance.PlayerUI.OnValueChange(this);
        this.GetComponentInChildren<Text>().text = "" + _newValue;
    }
}
