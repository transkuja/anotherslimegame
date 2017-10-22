using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    public GameObject[] prefabPlayerUI = new GameObject[4];
    public Dictionary<Player, GameObject> linkPlayerToItsUi = new Dictionary<Player, GameObject>();

    void Awake () {
        GameManager.Instance.RegisterPlayerUI(this);
	}

    public void Init()
    {
        for (int i = 0; i < GameManager.Instance.PlayerStart.PlayersReference.Count; i++)
        {
            GameObject playerUi = Instantiate(prefabPlayerUI[i], transform);
            linkPlayerToItsUi.Add(GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>(), playerUi);

            for (int j = 0; j < Utils.GetMaxValueForCollectable(CollectableType.Key); j++)
            {
                GameObject keySprite = Instantiate(ResourceUtils.Instance.refPrefabLoot.prefabKeySprite, playerUi.transform);
                keySprite.SetActive(false);
            }
        }       
    }

    public void RefreshPlayerUi(Player player, int _newValue)
    {
        Transform toRefresh = linkPlayerToItsUi[player].transform;
        for (int i = 0; i < _newValue; i++)
        {
            toRefresh.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = _newValue; i < Utils.GetMaxValueForCollectable(CollectableType.Key); i++)
        {
            toRefresh.GetChild(i).gameObject.SetActive(false);
        }
    }
}
