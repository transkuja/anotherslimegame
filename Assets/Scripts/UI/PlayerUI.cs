using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUI : MonoBehaviour {

    public enum PlayerUiChildren { Keys, Points };

    public GameObject[] prefabPlayerUI = new GameObject[4];
    public Dictionary<Player, GameObject> linkPlayerKeyToItsUi = new Dictionary<Player, GameObject>();
    public Dictionary<Player, GameObject> linkPlayerPointsToItsUi = new Dictionary<Player, GameObject>();

    void Awake () {
        GameManager.Instance.RegisterPlayerUI(this);
	}

    public void Init()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameObject playerUi = Instantiate(prefabPlayerUI[i], transform);
            // Generate UI key
            for (int j = 0; j < Utils.GetMaxValueForCollectable(CollectableType.Key); j++)
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
                    break;
                case 1: // Player 2
                    // TODO : need to be twerk, currently width of points -10
                    //pointSprite.transform.localPosition = new Vector3(-170, 0);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointSpriteRight, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
                    break;
                case 2: // Player 3
                    // Do nothing default value are ok
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointSpriteLeft, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
                    break;
                case 3: // Player 4
                        // TODO : need to be twerk, currently width of points -10
                        //pointSprite.transform.localPosition = new Vector3(-170, 0);
                    Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabPointSpriteRight, playerUi.transform.GetChild((int)PlayerUiChildren.Points).transform);
                    break;
                default :
                    Debug.Log("bug Point UI.....");
                    break;
            }

            //pointSprite.SetActive(true);
            linkPlayerPointsToItsUi.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerUi.transform.GetChild((int)PlayerUiChildren.Points).gameObject);

            // Initialize points
            RefreshPointsPlayerUi(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), 0);
            playerUi.SetActive(true);
        }
    }

    public void RefreshKeysPlayerUi(Player player, int _newValue)
    {
        if (!linkPlayerKeyToItsUi.ContainsKey(player))
            return;

        Transform toRefresh = linkPlayerKeyToItsUi[player].transform;
        for (int i = 0; i < _newValue; i++)
        {
            toRefresh.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = _newValue; i < Utils.GetMaxValueForCollectable(CollectableType.Key); i++)
        {
            toRefresh.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void RefreshPointsPlayerUi(Player player, int _newValue)
    {
        if (!linkPlayerPointsToItsUi.ContainsKey(player))
            return;

        Transform toRefresh = linkPlayerPointsToItsUi[player].transform;
        toRefresh.GetComponentInChildren<Text>().text = " X " + _newValue;
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
