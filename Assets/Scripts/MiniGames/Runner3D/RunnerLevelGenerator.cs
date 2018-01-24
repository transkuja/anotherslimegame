using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerLevelGenerator : MonoBehaviour {

    //List<RunnerBlocs> runnerBlocs;

    Pool runnerBlocPool;

    [SerializeField]Vector3 levelSize;
    public void OnValidate()
    {
        //levelSize.x = levelSize.x / RunnerBlocs.DefaultSize.x;
        //levelSize.y = levelSize.y / RunnerBlocs.DefaultSize.y;
        //levelSize.z = levelSize.z / RunnerBlocs.DefaultSize.z;
    }
    //public RunnerBlocs GetRandomBloc()
    //{
    //    //runnerBlocs.prefabs.Count
    //   //int rand = Random.Range(0, runnerBlocs.Count);
    //    //return runnerBlocs[rand];
    //}

    public void Generate2D()
    {
        Vector3 blocLen; // nb de blocs par rangées
        blocLen.x = levelSize.x/ RunnerBlocs.DefaultSize.x;
        blocLen.z = levelSize.z/ RunnerBlocs.DefaultSize.z;

        for (int x = 0; x < blocLen.x; x++)
        {
            for (int z = 0; z < blocLen.z; z++)
            {
                Vector3 position = new Vector3(x * RunnerBlocs.DefaultSize.x, 0, z * RunnerBlocs.DefaultSize.z);
                runnerBlocPool.GetItem(transform, position, Quaternion.identity).SetActive(true);
            }
        }
    }

    public void Start()
    {
        runnerBlocPool = ResourceUtils.Instance.poolManager.runnerBlocPool;
        Invoke("Generate2D", 0.4f);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Generate2D();
    }
}
