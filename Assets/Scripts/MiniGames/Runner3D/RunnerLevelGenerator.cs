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

        bool[,] mask = new bool[(int)blocLen.z, (int)blocLen.x];

        int posInLine = Random.Range(0, (int)blocLen.x);
        mask[0, posInLine] = true;

        for (int z = 1; z < blocLen.z; z++)
        {
            posInLine += Random.Range(-1, 2);
            posInLine = posInLine < 0 ? posInLine + 1 : posInLine;
            posInLine = posInLine >= blocLen.x ?(int)blocLen.x-1 : posInLine;
            mask[z, posInLine] = true;
        }




        for (int x = 0; x < blocLen.x; x++)
        {
            for (int z = 0; z < blocLen.z; z++)
            {
                if (mask[z,x])
                {
                    Vector3 position = new Vector3(x * RunnerBlocs.DefaultSize.x, 0, z * RunnerBlocs.DefaultSize.z);
                    runnerBlocPool.GetItem(transform, position, Quaternion.identity).SetActive(true);
                }
               
            }
        }
    }

    public void Start()
    {
        runnerBlocPool = ResourceUtils.Instance.poolManager.runnerBlocPool;
        //Invoke("Generate2D", 0.4f);

    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int z = 0; z < transform.childCount; z++)
            {
                Destroy(transform.GetChild(z).gameObject);
            }
            Generate2D();
        }
    }
}
