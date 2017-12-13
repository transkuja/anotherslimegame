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
            Respawner.RespawnProcess(p);
        else
        {
            // le joueur a perdu il ne réapparait pas; 
        }
    }
}
