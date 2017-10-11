using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType { Monster1 }
public class PrefabMonster : MonoBehaviour {

    [SerializeField]
    public GameObject prefabMonster1;

    public GameObject SpawnMonsterInstance(Vector3 where, Quaternion direction, Transform parent, MonsterType myMonsterType )
    {
        switch (myMonsterType)
        {
            case MonsterType.Monster1:
                return Instantiate(prefabMonster1, where, direction, parent);
            default:
                Debug.Log("Unknown Monster type");
                return null;
        }


    }
}
