using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceUtils : MonoBehaviour {

    private static RessourceUtils instance = null;
 
    public void Awake()
    {

        if (instance == null)
        {
            instance = this;

            refPrefabMonster = GetComponentInChildren<PrefabMonster>();
            refPrefabLoot = GetComponentInChildren<PrefabLoot>();

            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    [HideInInspector]
    public PrefabMonster refPrefabMonster;

    [HideInInspector]
    public PrefabLoot refPrefabLoot;

    public static RessourceUtils Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

}
