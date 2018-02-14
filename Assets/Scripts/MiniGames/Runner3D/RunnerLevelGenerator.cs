using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner3D
{

    enum State
    {
        Loading,
        LevelPresentation,
        InGame,
    }
    public class RunnerLevelGenerator : MonoBehaviour
    {
        public static readonly Vector3 defaultBlockSize = Vector3.one * 20; // Taille du plus petit bloc possible
        [SerializeField] GameObject arrivalPrefab;

        //  ref vers la pool. 
        //TODO: A transformer en dynamique pour contrôler la fréquence des blocs
        PoolLeader runnerBlocPool;
        List <GameObject> playerRef;


        [SerializeField] Vector3 levelUnit; // taille d'un niveau en nb de blocs
        [SerializeField] Vector3 leveFinalSize;  // taille réelle du niveau
        Runner3DGameMode runnerMode;

        [SerializeField]int firstPlayerZRow = -1;
        int nbRowUpInFrontFirst = 2;
        float timeBeforeFalling = 12;

        int nbBlockLineDestroyed;
        List<RunnerBlocs[]> blockLineList;


        [SerializeField] State state;
        bool[,] levelMask; // Pas nécessaire de l'enregistrer.

        // infnite generation : 
        //Vector3 curChunkOffset;
        int chunkLine = 0;
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

        public void OnValidate()
        {
            levelUnit.x = Mathf.Round(levelUnit.x);
            levelUnit.y = Mathf.Round(levelUnit.y);
            levelUnit.z = Mathf.Round(levelUnit.z);

            leveFinalSize.x = levelUnit.x * defaultBlockSize.x;
            leveFinalSize.y = levelUnit.y * defaultBlockSize.y;
            leveFinalSize.z = levelUnit.z * defaultBlockSize.z;
        }
        #region LevelGeneration

        #region blocsCreation
        // Fill the level according to the mask.
        public void GenerateLevelBlock(bool[,] mask,Vector3 startPos)
        {
            int extentsX = Mathf.FloorToInt((levelUnit.x + 1) * 0.5f);

            
            for (int z = 0; z < levelUnit.z; z++)
            {
                RunnerBlocs[] runnerblocsLine = new RunnerBlocs[(int)levelUnit.x];
                for (int x = 0; x < levelUnit.x; x++)
                {
                    if (mask[z, x])
                    {
                        Vector3 position = startPos + new Vector3(x * defaultBlockSize.x, 0, z * defaultBlockSize.z);
                        position.x -= extentsX;
                        GameObject bloc = runnerBlocPool.GetItem(transform, position, Quaternion.identity);
                        bloc.SetActive(true);
                        runnerblocsLine[x] = bloc.GetComponent<RunnerBlocs>();
                        runnerblocsLine[x].SaveStartPos();
                    }
                }
                blockLineList.Add(runnerblocsLine);
            }
            nextZChunkOffset += (int)levelUnit.z * (int)defaultBlockSize.z;

                // Si finite on fait apparraitre l'arrivée. 
            if (RunnerMode.Mode == Runner3DGameMode.EMode.Finite)
                Instantiate(arrivalPrefab, Vector3.forward * (defaultBlockSize.z*.5f + levelUnit.z * defaultBlockSize.z), Quaternion.identity,transform);
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
                posInLine =  Random.Range(0, (int)levelUnit.x);

            for (int z = 0; z < levelUnit.z; z++)
            {
                posInLine += Random.Range(-1, 2);
                posInLine = posInLine < 0 ?  0 : posInLine;
                posInLine = posInLine >= levelUnit.x ? (int)levelUnit.x - 1 : posInLine;
                mask[z, posInLine] = true;
            }
        }
        public bool[,] GenerateLevelMask()
        {
                // if first itération of bloc creation : 
            if (lastChunkLineBlocPos.Count == 0)
            {
                levelMask = new bool[(int)levelUnit.z, (int)levelUnit.x];
                for (int i = 0; i < 2; i++)
                    WritePathIntoLevelMask(levelMask);
            }
            else
            {
                levelMask = new bool[(int)levelUnit.z, (int)levelUnit.x];
                for (int i = 0; i < 2; i++)
                    WritePathIntoLevelMask(levelMask, lastChunkLineBlocPos[i % lastChunkLineBlocPos.Count]);
            }

                // save last line bloc pos for next génération. 
            for( int i = 0; i< levelUnit.x;i++)
            {
                if (levelMask[(int)levelUnit.z-1,i])
                    lastChunkLineBlocPos.Add(i);
            }
            return levelMask;
        }
        public void Generate2DChunk()
        {
            GenerateLevelMask();
            Vector3 centerStartPos = Vector3.forward * defaultBlockSize.z * 0.5f
                                    - Vector3.right * defaultBlockSize.x * 0.5f
                                    - Vector3.right * levelUnit.x * defaultBlockSize.x * 0.25f;
            GenerateLevelBlock(levelMask, nextZChunkOffset*Vector3.forward + centerStartPos);
        }


        public void MoveCursor(int newCursorValue)
        {
            int variation = newCursorValue - firstPlayerZRow;
            if (variation > 0)
            {
                for (int i = firstPlayerZRow + 1; i <= firstPlayerZRow + variation; i++)
                {
                    LerpMessage(i + nbRowUpInFrontFirst, DirLerpState.Up);
                    LerpMessage(i + nbRowUpInFrontFirst, DirLerpState.Down, timeBeforeFalling);
                }
            }
            firstPlayerZRow = newCursorValue;
            
        }
        #endregion

        #endregion

        #region LevelMovement
        public void LerpMessage(int row,DirLerpState dir,float waitTime = 0)
        {
         
            Debug.Log("Lauch " + row + "at " + dir);
            if (row < 0 || row >= blockLineList.Count)
                return;
            for (int x = 0; x < levelUnit.x; x++)
            {
                if (blockLineList[row][x]!=null)
                    blockLineList[row][x].LauchLerp(dir, waitTime);
            }
        }

        public void UpdatePlayerPos()
        {
            Vector3[] playerNewPos = new Vector3[playerRef.Count];
            int farthestZ = 0;
            for (int i = 0; i < playerRef.Count; i++)
                farthestZ = Mathf.Max(Mathf.RoundToInt(playerRef[i].transform.position.z), farthestZ);
            int playerZBlockPos = Mathf.FloorToInt((farthestZ) / defaultBlockSize.z);
            if (playerZBlockPos != firstPlayerZRow)
            {
                // si le joueur est preque arrivé à la fin du niveau on génère de nouveaux blocs
                if ((playerZBlockPos + nbRowUpInFrontFirst + 1) * defaultBlockSize.z > nextZChunkOffset)
                    Generate2DChunk();

                MoveCursor(playerZBlockPos);
            }
            firstPlayerZRow = playerZBlockPos;

        }
        #endregion
        public void Awake()
        {
            lastChunkLineBlocPos = new List<int>();
            blockLineList = new List<RunnerBlocs[]>();
            
        }
        public void Start()
        {
            playerRef = GameManager.Instance.PlayerStart.PlayersReference;
            state = State.Loading;
            runnerBlocPool = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.RunnerBloc);
            Generate2DChunk();
            LevelBegin();
        }
        public void LevelBegin()
        {
            MoveCursor(-nbRowUpInFrontFirst - 1);
            state = State.InGame;
            chunkLine = 0;
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                for (int z = 0; z < transform.childCount; z++)
                {
                    Destroy(transform.GetChild(z).gameObject);
                }
                Generate2DChunk();
            }
            switch (state)
            {
                case State.Loading:
                    break;
                case State.LevelPresentation:
                    break;
                case State.InGame:
                    UpdatePlayerPos();
                    break;
                default:
                    break;
            }

        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(Vector3.left * levelUnit.x*0.5f + Vector3.forward * levelUnit.z * 0.5f* defaultBlockSize.z, Vector3.Scale(levelUnit, defaultBlockSize));
        }
    }
}