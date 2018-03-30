﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner3D
{

    public class LevelItems
    {
        public GameObject[] wallsInGame;
        public GameObject clouds;
        public GameObject beginAreaPrefab;
    }

    public class RunnerLevelGenerator : MonoBehaviour
    {
        public static readonly Vector3 defaultBlockSize = Vector3.one * 20; // Taille des blocs de generation.
        [SerializeField] GameObject arrivalPrefab;
        [SerializeField] GameObject cloudsPrefabModel;
        [SerializeField] GameObject wallModel;
        [SerializeField] GameObject beginAreaModel;

        public LevelItems level;

        [SerializeField] DiffcultySO difficultyDB;
        [SerializeField] int  difficulty_ID;

        PoolLeader runnerBlocPool;

        [SerializeField] Vector3 levelUnit; // taille d'un niveau en nb de blocs
        [SerializeField] Vector3 levelFinalSize;  // taille réelle du niveau
        Runner3DGameMode runnerMode;

        int nbRowUpInFrontFirst = 2; // marge de pop des platforme par rapport au premier joueur.
        float timeBeforeFalling = 12; // temps que met une platforme à disparaître après avoir spawn

        List<RunnerBlocs[]> blockLineList; // Contains all rows of blocks since the beginning.  => [0] = [B(1,0), B(2,0), B(3,0), (...)]
                                           //                               => [1] = [B(1,1), B(1,2), B(1,3), (...)]

        bool[,] levelMask; // Pas nécessaire de l'enregistrer.

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

                // Si finite on fait apparraitre l'arrivée. 
            if (RunnerMode.Mode == Runner3DGameMode.EMode.Finite)
                Instantiate(arrivalPrefab, Vector3.forward * (defaultBlockSize.z*.5f + LevelUnit.z * defaultBlockSize.z), Quaternion.identity,transform);
        }

        public bool IsLevelFinishedAt(float zPos)
        {
            return (zPos >= nextZChunkOffset);
        }

        public void InitWalls()
        {
            int wallRows = 2;
            level.wallsInGame = new GameObject[wallRows];
            Vector3 wallPosition = Vector3.zero;
            // Init left walls :
            wallPosition.x = levelFinalSize.x;
            level.wallsInGame[0] = Instantiate(wallModel,wallPosition,Quaternion.identity);

            // init right Walls :
            wallPosition.x = -levelFinalSize.x;
            level.wallsInGame[1] = Instantiate(wallModel, wallPosition, Quaternion.identity);
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
        public void LerpMessage(int row,DirLerpState dir,float waitTime = 0)
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
            InitWalls();
            InitClouds();
            InitBeginArea();
        }
        public void Start()
        {
            Runner3DGameMode runnerMode = (GameManager.Instance.CurrentGameMode as Runner3DGameMode);
            runnerMode.levelGenerator = this;

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