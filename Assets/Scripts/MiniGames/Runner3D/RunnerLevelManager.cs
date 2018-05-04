using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Runner3D
{
    public class RunnerLevelManager : MonoBehaviour
    {
        #region Data
        LevelItems level; // Contient toutes les éléments qui doivent bouger dans le Level.
        enum State
        {
            Loading,
            InGame,
        }
        State state;

        List<GameObject> playerRef;

            // Info on player position to create/destroy blocs.
        int firstPlayerZRow = -1; // a quelle distance du début est le premier joueur en LevelUnit
        int nbRowUpInFrontFirst = 2; // marge de pop des platforme par rapport au premier joueur.
        float timeBeforeFalling = 12; // temps que met une platforme à disparaître après avoir spawn

            // components : 
        Runner3DGameMode runnerMode;
        RunnerLevelGenerator levelGenerator;

        #region getterSetters
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

        public RunnerLevelGenerator LevelGenerator
        {
            get
            {
                if (!levelGenerator)
                    levelGenerator = GetComponent<RunnerLevelGenerator>();
                return levelGenerator;
            }

            set
            {
                levelGenerator = value;
            }
        }

        public LevelItems Level
        {
            get
            {
                if (level == null)
                    level = LevelGenerator.level;
                return level;
            }

            set
            {
                level = value;
            }
        }
        #endregion
        #endregion


        #region Level Movement

        /// <summary>
        /// Sert à décaler les mur en avant de façon discrete
        /// </summary>
        public void UpdateWallPos()
        {
            //if (level.wallsInGame[0].transform.position.z < firstPlayerZRow * RunnerLevelGenerator.defaultBlockSize.z)
            //{
            //    // Calcule de combien le mur peut avancer sans que le joueur puisse le voir
            //    Material wallMat = level.wallsInGame[0].GetComponent<MeshRenderer>().sharedMaterial;
            //    float tiling = wallMat.mainTextureScale.x;
            //    float moveStep = level.wallsInGame[0].transform.localScale.z / tiling;

            //    // décaler les mur en avant.
            //    for (int i = 0; i < level.wallsInGame.Length; i++)
            //        level.wallsInGame[i].transform.position += Vector3.forward * moveStep;
            //}
        }
        public void UpdateCloudsPos()
        {
            Vector3 newCloudsPos = Level.clouds.transform.position;
            newCloudsPos.z = firstPlayerZRow * RunnerLevelGenerator.defaultBlockSize.z;
            Level.clouds.transform.position = newCloudsPos;
        }

        #endregion


        /// <summary>
        /// Move imaginary cursor at the first player Z BLOCK pos.
        /// if the player is in a new line : 
        ///-->  Order to  blocks to go Up / go down. 
        /// </summary>
        /// <param name="newCursorValue"></param>
        public void MoveCursor(int newCursorValue)
        {
            int variation = newCursorValue - firstPlayerZRow;
            if (variation > 0)
            {
                for (int i = firstPlayerZRow + 1; i <= firstPlayerZRow + variation; i++)
                {
                    LevelGenerator.OrderLerpRow(i + nbRowUpInFrontFirst, DirLerpState.Up);
                    LevelGenerator.OrderLerpRow(i + nbRowUpInFrontFirst, DirLerpState.Down, timeBeforeFalling);
                }
            }
            firstPlayerZRow = newCursorValue;
        }
        /// <summary>
        ///  args : 
        ///  Mathf.min to get last player block pos
        ///  Mathf.max to get first player block pos
        /// </summary>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public int PlayerBestZBlocPos(Comparison<int> comparer)
        {
            int farthestZ = 0;
            for (int i = 0; i < playerRef.Count; i++)
                farthestZ = comparer(Mathf.RoundToInt(playerRef[i].transform.position.z), farthestZ);
            int playerZBlockPos = Mathf.FloorToInt((farthestZ) / RunnerLevelGenerator.defaultBlockSize.z);
            return playerZBlockPos;
        }
       
        void InfiniteModeUpdate()
        {
            // compute where first player is and pop platform if necessary
            int playerZBlockPos = PlayerBestZBlocPos(Mathf.Max);
            if (playerZBlockPos != firstPlayerZRow)
            {
                        // si le joueur est preque arrivé à la fin du niveau on génère de nouveaux blocs
                if(LevelGenerator.IsLevelFinishedAt((playerZBlockPos + nbRowUpInFrontFirst + 1) * RunnerLevelGenerator.defaultBlockSize.z))
                    LevelGenerator.Generate2DChunk();
                MoveCursor(playerZBlockPos);
            }
            firstPlayerZRow = playerZBlockPos;
        }
        public void Awake()
        {
            state = State.Loading;
        }
        public void Start()
        {
            LevelGenerator = GetComponent<RunnerLevelGenerator>();
            Level = LevelGenerator.level;
        }
        public void OnLevelBegin()
        {
            playerRef = GameManager.Instance.PlayerStart.PlayersReference; // safe ? 
            MoveCursor(-nbRowUpInFrontFirst - 1);
            state = State.InGame;
            Level.beginAreaPrefab.GetComponent<RunnerBlocs>().SaveStartPos();
            Level.beginAreaPrefab.GetComponent<RunnerBlocs>().LauchLerp(DirLerpState.Down, timeBeforeFalling);
        }

        public void Update()
        {
            UpdateCloudsPos();
            UpdateWallPos();
            switch (state)
            {
                case State.Loading:
                    break;
                case State.InGame:
                    InfiniteModeUpdate();
                    break;
                default:
                    break;
            }
        }
    }
}
