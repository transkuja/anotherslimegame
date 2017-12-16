using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePushManager : MonoBehaviour {

    CostArea costArea;
     // fake singleton
    static MiniGamePushManager singleton;

    public static MiniGamePushManager Singleton
    {
        get
        {
            return singleton;
        }
        set
        {
            singleton = value;
        }
    }
    public void Awake()
    {
        singleton = this;
    }
    public void Start()
    {
        costArea = GetComponent<CostArea>();
    }
    void Update () {
		
	}
    public void StartGame(List<GameObject> playerReferences)
    {
        Utils.PopTutoTextForAll("Be the last one standing!");

        Player player;
        for (int i = 0; i < playerReferences.Count; i++)
        {
            player = playerReferences[i].GetComponent<Player>();
            player.UpdateCollectableValue(CollectableType.StrengthEvolution1, 1);
            player.NbLife = 10;
            GameManager.Instance.PlayerUI.ShowLife(true);
            GameManager.Instance.PlayerUI.ShowPoints(false);
            GameManager.Instance.PlayerUI.RefreshLifePlayerUi(player, player.NbLife, i);
        }
    }

    public void CheckVictory()
    {
        // parcourir les joueur et verifier combien de kills ils ont
        //costArea.end
        //costArea->HasFinishedProcess;
    }
    public void ResetPlayer(Player p)
    {
        p.NbLife-=1;
        if (p.NbLife>0)
        {
            Respawner.RespawnProcess(p);
            GameManager.Instance.PlayerUI.RefreshLifePlayerUi(p, p.NbLife, p.cameraReference.transform.GetSiblingIndex());
        }
        else
        {
            // le joueur a perdu il ne réapparait pas; 
            /// ecran noir ?
        }
    }
}
