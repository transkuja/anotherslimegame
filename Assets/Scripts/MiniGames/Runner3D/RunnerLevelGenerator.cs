using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerLevelGenerator : MonoBehaviour {

        //  ref vers la pool. 
        //TODO: A transformer en tableau pour contrôler la fréquence des blocs
    Pool runnerBlocPool;
        
    public static readonly Vector3 defaultBlockSize = Vector3.one * 20; // Taille du plus petit bloc possible
    [SerializeField]Vector3 levelUnit; // taille d'un niveau en nb de blocs
    [SerializeField]Vector3 leveFinalSize;  // taille réelle du niveau
    public void OnValidate()
    {
        levelUnit.x = Mathf.Round(levelUnit.x);
        levelUnit.y = Mathf.Round(levelUnit.y);
        levelUnit.z = Mathf.Round(levelUnit.z);

        leveFinalSize.x = levelUnit.x * defaultBlockSize.x;
        leveFinalSize.y = levelUnit.y * defaultBlockSize.y;
        leveFinalSize.z = levelUnit.z * defaultBlockSize.z;
    }
  
        // Fill the level according to the mask.
    public void GenerateLevelBlock(bool[,] mask)
    {
        for (int x = 0; x < levelUnit.x; x++)
        {
            for (int z = 0; z < levelUnit.z; z++)
            {
                if (mask[z, x])
                {
                    Vector3 position = new Vector3(x * defaultBlockSize.x, 0, z * defaultBlockSize.z);
                    runnerBlocPool.GetItem(transform, position, Quaternion.identity).SetActive(true);
                }

            }
        }
    }
    public bool[,] GenerateLevelMask()
    {
        bool[,] mask = new bool[(int)levelUnit.z, (int)levelUnit.x];

        // random walker algorithme to fill the level mask
        int posInLine = Random.Range(0, (int)levelUnit.x);
        mask[0, posInLine] = true;

        for (int z = 1; z < levelUnit.z; z++)
        {
            posInLine += Random.Range(-1, 2);
            posInLine = posInLine < 0 ? posInLine + 1 : posInLine;
            posInLine = posInLine >= levelUnit.x ? (int)levelUnit.x - 1 : posInLine;
            mask[z, posInLine] = true;
        }
        return mask;
    }

    public void Generate2D()
    {
        bool[,] mask = GenerateLevelMask();
        GenerateLevelBlock(mask);
    }

    public void Start()
    {
        runnerBlocPool = ResourceUtils.Instance.poolManager.runnerBlocPool;
        Invoke("Generate2D", 0.4f);
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
