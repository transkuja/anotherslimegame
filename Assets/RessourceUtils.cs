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
            DontDestroyOnLoad(Instance);
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

    public void Start()
    {
        refPrefabMonster = GetComponentInChildren<PrefabMonster>();
        refPrefabLoot = GetComponentInChildren<PrefabLoot>();
    }
}
