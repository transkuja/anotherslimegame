using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceUtils : MonoBehaviour
{

    private static ResourceUtils instance = null;

    void FindPoolManager(Scene scene, LoadSceneMode mode)
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;

            refPrefabMonster = GetComponentInChildren<PrefabMonster>();
            refPrefabLoot = GetComponentInChildren<PrefabLoot>();
            refPrefabPlatform = GetComponentInChildren<PrefabPlatform>();
            refPrefabGhost = GetComponentInChildren<PrefabGhost>();
            debugTools = GetComponentInChildren<DebugTools>();
            particleSystemManager = GetComponentInChildren<ParticleSystemManager>();
            spriteUtils = GetComponentInChildren<SpriteUtils>();
            feedbacksManager = GetComponentInChildren<FeedbacksManager>();

            SceneManager.sceneLoaded += FindPoolManager;

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

    // TODO: move it, may have nothing to do here
    [Tooltip("Emissive color for moving platforms")]
    public Color isMovingEmissiveColor;

    [Tooltip("Emissive color for teleporters")]
    public Color teleporterEmissiveColor;

    [HideInInspector]
    public PrefabMonster refPrefabMonster;

    [HideInInspector]
    public PrefabLoot refPrefabLoot;

    [HideInInspector]
    public PrefabPlatform refPrefabPlatform;

    [HideInInspector]
    public PrefabGhost refPrefabGhost;

    [HideInInspector]
    public DebugTools debugTools;

    [HideInInspector]
    public PoolManager poolManager;

    [HideInInspector]
    public ParticleSystemManager particleSystemManager;

    [HideInInspector]
    public SpriteUtils spriteUtils;

    [HideInInspector]
    public FeedbacksManager feedbacksManager;

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
