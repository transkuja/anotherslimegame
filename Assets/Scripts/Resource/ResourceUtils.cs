using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceUtils : MonoBehaviour {

    private static ResourceUtils instance = null;
 
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;

            refPrefabMonster = GetComponentInChildren<PrefabMonster>();
            refPrefabLoot = GetComponentInChildren<PrefabLoot>();
            refPrefabPlatform = GetComponentInChildren<PrefabPlatform>();
            refPrefabGhost = GetComponentInChildren<PrefabGhost>();
            refPrefabIle = GetComponentInChildren<PrefabIle>();
            refMainTowerGameplayManager = GetComponentInChildren<MainTowerGameplayManager>();
            debugTools = GetComponentInChildren<DebugTools>();

            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // TODO: move it, may have nothing to do here
    [Tooltip("Emissive color for bounciness")]
    public Color bounceEmissiveColor;

    [HideInInspector]
    public PrefabMonster refPrefabMonster;

    [HideInInspector]
    public PrefabLoot refPrefabLoot;


    [HideInInspector]
    public PrefabIle refPrefabIle;

    [HideInInspector]
    public PrefabPlatform refPrefabPlatform;

    [HideInInspector]
    public PrefabGhost refPrefabGhost;

    [HideInInspector]
    public MainTowerGameplayManager refMainTowerGameplayManager;

    [HideInInspector]
    public DebugTools debugTools;

    public static ResourceUtils Instance
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
