using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabMonster : MonoBehaviour {

    private static PrefabMonster instance;

    public PrefabMonster Instance
    {
        get
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            return instance;
        }

        private set { }
    }

    [SerializeField]
    public GameObject prefabMonster1;

    public GameObject spawnMonster2Instance(Vector3 where, Quaternion direction, Transform parent)
    {
        return Instantiate(Instance.prefabMonster1, where, direction, parent);
    }
}
