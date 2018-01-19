using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour {

    public enum PlayerUiChildren { Keys, Points };

    public GameObject[] prefabPlayerUI = new GameObject[4];
    public Dictionary<Player, GameObject> linkPlayerKeyToItsUi = new Dictionary<Player, GameObject>();
    public Dictionary<Player, GameObject> linkPlayerPointsToItsUi = new Dictionary<Player, GameObject>();
    public Dictionary<Player, GameObject> linkPlayerLifesToItsUi = new Dictionary<Player, GameObject>();

    void Awake () {
        GameManager.Instance.RegisterPlayerUI(this);
	}

    public virtual void Init()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameObject playerUi = Instantiate(prefabPlayerUI[i], transform);
            // Generate UI key
            for (int j = 0; j < Utils.GetMaxValueForCollectable(CollectableType.Rune); j++)
            {
                GameObject keySprite = Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabKeySprite, playerUi.transform.GetChild((int)PlayerUiChildren.Keys).transform);
                keySprite.SetActive(false);
            }
            linkPlayerKeyToItsUi.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerUi.transform.GetChild((int)PlayerUiChildren.Keys).gameObject);

            // Generate UI points
            switch (i){
                case 0: // Player 1
                        // Do nothing default value are ok
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointSpriteLeft, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabLifeSpriteLeft, playerUi.transform.Find("Life"));
                    break;
                case 1: // Player 2
                    // TODO : need to be twerk, currently width of points -10
                    //pointSprite.transform.localPosition = new Vector3(-170, 0);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointSpriteRight, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabLifeSpriteRight, playerUi.transform.Find("Life"));
                    break;
                case 2: // Player 3
                    // Do nothing default value are ok
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointSpriteLeft, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabLifeSpriteLeft, playerUi.transform.Find("Life"));
                    break;
                case 3: // Player 4
                        // TODO : need to be twerk, currently width of points -10
                        //pointSprite.transform.localPosition = new Vector3(-170, 0);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointSpriteRight, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabLifeSpriteRight, playerUi.transform.Find("Life"));
                    break;
                default :
                    Debug.Log("bug Point UI.....");
                    break;
            }

            //pointSprite.SetActive(true);
            linkPlayerPointsToItsUi.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerUi.transform.GetChild((int)PlayerUiChildren.Points).gameObject);

            playerUi.transform.Find("Life").gameObject.SetActive(false);
            linkPlayerLifesToItsUi.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerUi.transform.Find("Life").gameObject);

            // Initialize points
            RefreshPointsPlayerUi(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), 0, i);
            playerUi.SetActive(true);
        }
    }
    public void ShowLife(bool isShown)
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count;i++)
        {
            Player player = GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>();
            linkPlayerLifesToItsUi[player].SetActive(isShown);
        }
    }
    public void ShowPoints(bool isShown)
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            Player player = GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>();
            linkPlayerPointsToItsUi[player].SetActive(isShown);
        }
    }

    // TODO: Remi
    public void RefreshKeysPlayerUi(Player player, int _newValue)
    {
        // TODO: should only refresh the runes value        
    }

    public virtual void RefreshPointsPlayerUi(Player player, int _newValue, int index)
    {
        if (!linkPlayerPointsToItsUi.ContainsKey(player))
            return;

        Transform toRefresh = linkPlayerPointsToItsUi[player].transform;
        if (index % 2 == 0)
            toRefresh.GetComponentInChildren<Text>().text = " X " + _newValue;
        else
            toRefresh.GetComponentInChildren<Text>().text = _newValue + " X ";
    }
    public void RefreshLifePlayerUi(Player player, int _newValue, int index)
    {
        if (!linkPlayerLifesToItsUi.ContainsKey(player))
            return;

        Transform toRefresh = linkPlayerLifesToItsUi[player].transform;
        if (index % 2 == 0)
            toRefresh.GetComponentInChildren<Text>().text = " X " + _newValue;
        else
            toRefresh.GetComponentInChildren<Text>().text = _newValue + " X ";
    }
    public void HandleFeedbackNotEnoughPoints(Player player, bool _activate)
    {
        if (!linkPlayerPointsToItsUi.ContainsKey(player))
            return;

        Transform ptsText = linkPlayerPointsToItsUi[player].transform.GetComponentInChildren<Text>().transform;


        if (_activate)
        {
            // Feedback already running
            if (ptsText.GetComponent<AnimTextCantPay>())
                return;

            ptsText.GetComponent<Outline>().effectColor = Color.red;
            ptsText.GetComponent<Text>().fontSize += 20;
            ptsText.gameObject.AddComponent<AnimTextCantPay>();
            player.FeedbackCantPayActive = true;
        }
        else
        {
            ptsText.GetComponent<Outline>().effectColor = Color.black;
            ptsText.GetComponent<Text>().fontSize -= 20;
            Destroy(ptsText.GetComponent<AnimTextCantPay>());
            ptsText.localScale = Vector3.one;          
        }
    }
}
