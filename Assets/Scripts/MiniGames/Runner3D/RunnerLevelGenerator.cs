using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner3D
{
        // Used to communicate with RunnerLevelManager 
    public class LevelItems
    {
        public GameObject[] wallsInGame;
        public GameObject clouds;
        public GameObject beginAreaPrefab;
    }

    public class RunnerLevelGenerator : MonoBehaviour
    {
        public static readonly Vector3 defaultBlockSize = Vector3.one * 20; // Taille des blocs de generation.

            // prefab to populate level : 
        [SerializeField] GameObject arrivalPrefab;
        [SerializeField] GameObject cloudsPrefabModel;
        [SerializeField] GameObject beginAreaModel;
        PoolLeader runnerBlocPool;
            
        public LevelItems level;
            // difficulty settings : 
        [SerializeField] DiffcultySO difficultyDB;
        [SerializeField] int  difficulty_ID;

            // Level Size Parameters : 
        [SerializeField] Vector3 levelUnit; // taille d'un chunk en nb de blocs
        [SerializeField] Vector3 levelFinalSize;  // taille réelle du chunk

            // Refs : 
        Runner3DGameMode runnerMode;

            // level Data : 
        List<RunnerBlocs[]> blockLineList; // Contains all rows of blocks since the beginning.  => [0] = [B(1,0), B(2,0), B(3,0), (...)]
                                           //                                                   => [1] = [B(1,1), B(1,2), B(1,3), (...)]
        bool[,] levelMask; // Pas nécessaire de l'enregistrer.
            // level data to create chunk of blocs : 
        List<int> lastChunkLineBlocPos;
        int nextZChunkOffset;



        public Runner3DGameMode RunnerMode
        {
            get
            {
                if (runnerMode == null)
                    runnerMode = GameManager.Instance.CurrentGameMode as Runner3DGameMode;
                return runnerMode;
            }

            set
            {
                runnerMode = value;
            }
        }

        public Vector3 LevelUnit
        {
            get
            {
                return levelUnit;
            }

            private set
            {
                levelUnit = value;
            }
        }

        public void OnValidate()
        {
            levelUnit.x = Mathf.Round(LevelUnit.x);
            levelUnit.y = Mathf.Round(LevelUnit.y);
            levelUnit.z = Mathf.Round(LevelUnit.z);

            levelFinalSize.x = LevelUnit.x * defaultBlockSize.x;
            levelFinalSize.y = LevelUnit.y * defaultBlockSize.y;
            levelFinalSize.z = LevelUnit.z * defaultBlockSize.z;
        }
        #region LevelGeneration
        #region PrefabInstantiation
        // Fill the level according to the mask.
        public void GenerateLevelBlock(bool[,] mask,Vector3 startPos)
        {
            int extentsX = Mathf.FloorToInt((LevelUnit.x + 1) * 0.5f);
            for (int z = 0; z < LevelUnit.z; z++)
            {
                RunnerBlocs[] runnerblocsLine = new RunnerBlocs[(int)LevelUnit.x];
                for (int x = 0; x < LevelUnit.x; x++)
                {
                    if (mask[z, x])
                    {
                        Vector3 position = startPos + new Vector3(x * defaultBlockSize.x, 0, z * defaultBlockSize.z);
                        position.x -= extentsX;
                        float curZPos = startPos.z + z * defaultBlockSize.z;
                        int poolRow = difficultyDB.difficultyParameters[difficulty_ID].GetTableRandAt(curZPos);
                        runnerBlocPool = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.RunnerBloc,poolRow);
                        GameObject bloc = runnerBlocPool.GetItem(transform, position, Quaternion.identity);
                        if (bloc == null)
                        {
                            Debug.LogWarning("Pool item is null");
                            return;
                        }
                        bloc.SetActive(true);
                        runnerblocsLine[x] = bloc.GetComponent<RunnerBlocs>();
                        runnerblocsLine[x].SaveStartPos();
                        runnerblocsLine[x].TpDown();
                    }
                }
                blockLineList.Add(runnerblocsLine);
            }
            nextZChunkOffset += (int)LevelUnit.z * (int)defaultBlockSize.z;
        }

        public bool IsLevelFinishedAt(float zPos)
        {
            return (zPos >= nextZChunkOffset);
        }

        public void InitClouds()
        {
            level.clouds = Instantiate(cloudsPrefabModel, transform);
        }
        public void InitBeginArea()
        {
            level.beginAreaPrefab = Instantiate(beginAreaModel);
            level.beginAreaPrefab.SetActive(true);
        }
        #endregion

        #region MaskCreation

        /// random walker algorithme to fill the level mask
        public void WritePathIntoLevelMask(bool[,] mask,int? lastPosInLine = null)
        {
            int posInLine;
            if (lastPosInLine.HasValue)
                posInLine = lastPosInLine.Value;
            else
                posInLine = Mathf.FloorToInt(LevelUnit.x*0.5f)   ;

            for (int z = 0; z < LevelUnit.z; z++)
            {
                posInLine += Random.Range(-1, 2);
                posInLine = posInLine < 0 ?  0 : posInLine;
                posInLine = posInLine >= LevelUnit.x ? (int)LevelUnit.x - 1 : posInLine;
                mask[z, posInLine] = true;
            }
        }
        public bool[,] GenerateLevelMask()
        {
                // if first itération of bloc creation : 
            if (lastChunkLineBlocPos.Count == 0)
            {
                levelMask = new bool[(int)LevelUnit.z, (int)LevelUnit.x];
                for (int i = 0; i < 2; i++)
                    WritePathIntoLevelMask(levelMask);
            }
            else
            {
                levelMask = new bool[(int)LevelUnit.z, (int)LevelUnit.x];
                for (int i = 0; i < 2; i++)
                    WritePathIntoLevelMask(levelMask, lastChunkLineBlocPos[i % lastChunkLineBlocPos.Count]);
            }

                // save last line bloc pos for next génération. 
            for( int i = 0; i< LevelUnit.x;i++)
            {
                if (levelMask[(int)LevelUnit.z-1,i])
                    lastChunkLineBlocPos.Add(i);
            }
            return levelMask;
        }
        public void Generate2DChunk()
        {
            GenerateLevelMask();
            Vector3 centerStartPos = Vector3.forward * defaultBlockSize.z * 0.5f
                                    - Vector3.right * defaultBlockSize.x * 0.5f
                                    - Vector3.right * levelFinalSize.x * 0.25f;
            GenerateLevelBlock(levelMask, nextZChunkOffset*Vector3.forward + centerStartPos);
        }
        #endregion

        #endregion

        #region LevelMovement
        public void OrderLerpRow(int row,DirLerpState dir,float waitTime = 0)
        {
         
            //Debug.Log("Lauch " + row + "at " + dir);
            if (row < 0 || row >= blockLineList.Count)
                return;
            for (int x = 0; x < LevelUnit.x; x++)
            {
                if (blockLineList[row][x]!=null)
                    blockLineList[row][x].LauchLerp(dir, waitTime);
            }
        }

        #endregion
        public void Awake()
        {
            level = new LevelItems();
            lastChunkLineBlocPos = new List<int>();
            blockLineList = new List<RunnerBlocs[]>();
            InitClouds();
            InitBeginArea();
        }
        public void Start()
        {
            Runner3DGameMode runnerMode = (GameManager.Instance.CurrentGameMode as Runner3DGameMode);
            //runnerMode.levelGenerator = this;
            runnerBlocPool = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.RunnerBloc);
        }
        public void LevelBegin()
        {
            Generate2DChunk();
            RunnerLevelManager levelManager = GetComponent<RunnerLevelManager>();
            levelManager.OnLevelBegin();
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(transform.position + Vector3.left * LevelUnit.x*0.5f + Vector3.forward * levelFinalSize.z * 0.5f,
                Vector3.Scale(LevelUnit, defaultBlockSize));
        }
    }
}