using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runner3D
{
    public class RunnerLevelGenerator : MonoBehaviour
    {

        //  ref vers la pool. 
        //TODO: A transformer en tableau pour contrôler la fréquence des blocs
        PoolLeader runnerBlocPool;
        Player[] playerRef;

        RunnerBlocs[,] blockMap;
        public static readonly Vector3 defaultBlockSize = Vector3.one * 20; // Taille du plus petit bloc possible
        [SerializeField] Vector3 levelUnit; // taille d'un niveau en nb de blocs
        [SerializeField] Vector3 leveFinalSize;  // taille réelle du niveau
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
        public void GenerateLevelBlock(bool[,] mask,Vector3 startPos)
        {
            blockMap = new RunnerBlocs[(int)levelUnit.x, (int)levelUnit.z];
            int extentsX = Mathf.FloorToInt((levelUnit.x + 1) * 0.5f);
            //int extentsZ = Mathf.FloorToInt((levelUnit.z + 1) * 0.5f);

            for (int x = 0; x < levelUnit.x; x++)
            {
                for (int z = 0; z < levelUnit.z; z++)
                {
                    if (mask[z, x])
                    {
                        Vector3 position = startPos+ new Vector3(x * defaultBlockSize.x, 0, z * defaultBlockSize.z);
                        position.x -= extentsX;
                        GameObject bloc = runnerBlocPool.GetItem(transform, position, Quaternion.identity);
                        bloc.SetActive(true);
                        blockMap[x, z] = bloc.GetComponent<RunnerBlocs>();
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
            GenerateLevelBlock(mask,defaultBlockSize.z*Vector3.forward*0.5f);
            StartCoroutine(LevelPresentation());
        }

        int firstPlayerZPos = 0;
        int nbRowUpBehindFirst = 2;
        int nbRowUpInFrontFirst = 2;
        public void MoveCursor(int newCursorValue)
        {
            int variation = newCursorValue - firstPlayerZPos;
            if (variation > 0)
            {
                for (int i = firstPlayerZPos + 1; i <= firstPlayerZPos + variation; i++)
                {
                    LerpMessage(i + nbRowUpInFrontFirst, Direction.Up);
                    LerpMessage(i - nbRowUpBehindFirst, Direction.Down);
                }
            }
            else if (variation < 0) // Pas encore testé.
                for (int i = firstPlayerZPos; i >= firstPlayerZPos - variation; i--)
                {
                    LerpMessage(firstPlayerZPos + i, Direction.Down);
                }
            else
                Debug.LogWarning("This function shouldn't be called");
            firstPlayerZPos = newCursorValue;
        }
        public void LerpMessage(int row,Direction dir)
        {
            if (row < 0 || row >= levelUnit.z)
                return;
            Debug.Log("Line : " + row + "go " + dir);
            for (int x = 0; x < levelUnit.x; x++)
            {
                if (blockMap[x, row] != null)
                    blockMap[x, row].LauchLerp(dir);
            }
        }

        public void UpdatePlayerPos()
        {
            Vector3[] playerNewPos = new Vector3[playerRef.Length];
            float farthestZ = 0;
            for (int i = 0; i < playerRef.Length; i++)
                farthestZ = Mathf.Max(playerRef[i].transform.position.z, farthestZ);
            if (farthestZ != firstPlayerZPos)
            {
                float test = farthestZ / defaultBlockSize.z;
                int playerZBlockPos = Mathf.RoundToInt(farthestZ / defaultBlockSize.z);
            }
        }

        public void Start()
        {
            runnerBlocPool = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.RunnerBloc);
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
        public IEnumerator LevelPresentation()
        {
            yield return new WaitForSeconds(0.5f);
            firstPlayerZPos = -nbRowUpInFrontFirst - 1;
            for (int i = - nbRowUpInFrontFirst; i < levelUnit.z+ nbRowUpBehindFirst; i++)
            {
                MoveCursor(i);
                yield return new WaitForSeconds(0.5f);
            }
            MoveCursor(-nbRowUpInFrontFirst-1);
            MoveCursor(-1);
            yield return null;
        }
        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(Vector3.left * levelUnit.x*0.5f + Vector3.forward * levelUnit.z * 0.5f* defaultBlockSize.z, Vector3.Scale(levelUnit, defaultBlockSize));
        }
    }
}