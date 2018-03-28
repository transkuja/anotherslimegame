//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Runner3D
//{
//    public class RunnerLevelManager : MonoBehaviour {

//        #region Data
//        class LevelItem
//        {
//            public GameObject[] wallsInGame;
//            public GameObject clouds;
//        }
//        LevelItem level; // Contient toutes les éléments qui doivent bouger dans le Level.
//        enum State
//        {
//            Loading,
//            InGame,
//        }
//        State state;

//        List<GameObject> playerRef;

//        int firstPlayerZRow = -1; // a quelle distance du début est le premier joueur en LevelUnit
//        int lastPlayerZRow; // // a quelle distance du début est le dernier  joueur en LevelUnit

//        Runner3DGameMode runnerMode;
//        #region getterSetters
//        public Runner3DGameMode RunnerMode
//        {
//            get
//            {
//                if (runnerMode == null)
//                    runnerMode = GameManager.Instance.CurrentGameMode as Runner3DGameMode;
//                return runnerMode;
//            }
//            set
//            {
//                runnerMode = value;
//            }
//        }
//        #endregion
//        #endregion


//        #region Level Movment

//        /// <summary>
//        /// Sert à décaler les mur en avant de façon discrete
//        /// </summary>
//        public void UpdateWallPos()
//        {
//            if (level.wallsInGame[0].transform.position.z < firstPlayerZRow *RunnerLevelGenerator.defaultBlockSize.z)
//            {
//                        // Calcule de combien le mur peut avancer sans que le joueur puisse le voir
//                Material wallMat = level.wallsInGame[0].GetComponent<MeshRenderer>().sharedMaterial;
//                float tiling = wallMat.mainTextureScale.x;
//                float moveStep = level.wallsInGame[0].transform.localScale.z / tiling;

//                        // décaler les mur en avant.
//                for (int i = 0; i < level.wallsInGame.Length; i++)
//                    level.wallsInGame[i].transform.position += Vector3.forward * moveStep;
//            }
//        }
//        public void UpdateCloudsPos()
//        {
//            Vector3 newCloudsPos = level.clouds.transform.position;
//            newCloudsPos.z = firstPlayerZRow * RunnerLevelGenerator.defaultBlockSize.z;
//            level.clouds.transform.position = newCloudsPos;
//        }

//        #endregion






//        public void UpdatePlayerPos()
//        {
//            Vector3[] playerNewPos = new Vector3[playerRef.Count];
//            int farthestZ = 0;
//            for (int i = 0; i < playerRef.Count; i++)
//                farthestZ = Mathf.Max(Mathf.RoundToInt(playerRef[i].transform.position.z), farthestZ);
//            int playerZBlockPos = Mathf.FloorToInt((farthestZ) /RunnerLevelGenerator.defaultBlockSize.z);
//            if (playerZBlockPos != firstPlayerZRow && playerZBlockPos + nbRowUpInFrontFirst < levelUnit.z)
//            {
//                // si le joueur est preque arrivé à la fin du niveau on génère de nouveaux blocs
//                if ((playerZBlockPos + nbRowUpInFrontFirst + 1) * defaultBlockSize.z > nextZChunkOffset)
//                    Generate2DChunk();
//                MoveCursor(playerZBlockPos);
//            }
//            firstPlayerZRow = playerZBlockPos;

//        }
//        void InfiniteModeUpdate()
//        {
//            Vector3[] playerNewPos = new Vector3[playerRef.Count];
//            int farthestZ = 0;

//            // compute where first player is and pop platform if necessary
//            for (int i = 0; i < playerRef.Count; i++)
//                farthestZ = Mathf.Max(Mathf.RoundToInt(playerRef[i].transform.position.z), farthestZ);
//            int playerZBlockPos = Mathf.FloorToInt((farthestZ) / defaultBlockSize.z);
//            if (playerZBlockPos != firstPlayerZRow)
//            {
//                // si le joueur est preque arrivé à la fin du niveau on génère de nouveaux blocs
//                if ((playerZBlockPos + nbRowUpInFrontFirst + 1) * defaultBlockSize.z > nextZChunkOffset)
//                    Generate2DChunk();
//                MoveCursor(playerZBlockPos);
//            }
//            firstPlayerZRow = playerZBlockPos;

//            // compute last player pos : 
//            for (int i = 0; i < playerRef.Count; i++)
//                lastPlayerZRow = Mathf.Min(Mathf.RoundToInt(playerRef[i].transform.position.z), farthestZ);
//        }

//        public void Start()
//        {
//            playerRef = GameManager.Instance.PlayerStart.PlayersReference; // safe ? 
//        }

//        public void Update()
//        {
//            UpdateCloudsPos();
//            UpdateWallPos();
//            switch (state)
//            {
//                case State.Loading:
//                    break;
//                case State.InGame:
//                    switch (RunnerMode.Mode)
//                    {
//                        case Runner3DGameMode.EMode.SoloInfinite:
//                            InfiniteModeUpdate();
//                            break;
//                        case Runner3DGameMode.EMode.LastRemaining:
//                            InfiniteModeUpdate();
//                            break;
//                        case Runner3DGameMode.EMode.Finite:
//                            UpdatePlayerPos();
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//}
