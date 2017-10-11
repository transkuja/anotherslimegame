using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnType { Item, Monster }
public class SpawnComponent : MonoBehaviour {

    [SerializeField]
    public SpawnType mySpawnType;

    public MonsterType myMonsterType;
    public ItemType myItemType;


    private int mySpawnId;

    [Tooltip("Spawn a spawnable in registry")]
    public bool needSpawn = true;

    [Tooltip("Force spawn despite reaching the max spawn unit at the same time")]
    public bool forceSpawn = false;

	// Use this for initialization
	void Start () {
        switch (mySpawnType) {
            case SpawnType.Item:
                mySpawnId = SpawnManager.Instance.RegisterSpawnItemLocation(this.transform, myItemType, needSpawn, forceSpawn);
                break;
            case SpawnType.Monster:
                mySpawnId = SpawnManager.Instance.RegisterSpawnMonsterLocation(this.transform, myMonsterType, needSpawn, forceSpawn);
                break;
            default:
                Debug.Log("Unknowned Spawn Type");
                break;
        }
    }

    private void OnDestroy()
    {
        switch (mySpawnType)
        {
            case SpawnType.Item:
                SpawnManager.Instance.UnregisterSpawnItemLocation(mySpawnId);
                break;
            case SpawnType.Monster:
                SpawnManager.Instance.UnregisterSpawnMonsterLocation(mySpawnId);
                break;
            default:
                Debug.Log("Unknowned Spawn Type");
                break;
        }
    }

}
