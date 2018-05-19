using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingGameSpawner : MonoBehaviour {

    public bool withTrappedPots = false;

    [Tooltip("% of chance each pot will be trapped.")]
    [Range(10, 100)]
    [SerializeField]
    public float trapFrequency = 10.0f;

    int lineCount;
    int mapSize;
    BreakingGameMode gameMode;

    IEnumerator Start()
    {
        lineCount = transform.childCount;
        mapSize = FindObjectsOfType<BoardFloor>().Length;

        yield return new WaitUntil(() => GameManager.CurrentState != GameState.ForcedPauseMGRules);
        gameMode = (BreakingGameMode)GameManager.Instance.CurrentGameMode;
        
        while (true)
        {
            yield return new WaitUntil(() => gameMode.activePots == 0);

            int randomPatt = Random.Range(0, (int)BoardSpawner.Pattern.Size);

            int[] randIndex = BoardSpawner.GetPattern((BoardSpawner.Pattern)Random.Range(0, (int)BoardSpawner.Pattern.Size));
            gameMode.activePots = randIndex.Length;
            Spawn(randIndex);
            Debug.Log(gameMode.activePots);
        }
    }

    void Spawn(int[] _index)
    {
        for (int i = 0; i < _index.Length; i++)
        {
            Transform currentFloor = transform.GetChild(_index[i] / lineCount).GetChild(_index[i] % lineCount);
            // TODO: check what happened if player is on the floor
            //if (currentFloor.GetComponent<BoardFloor>().HasAnItem)
            //    currentFloor.GetComponentInChildren<PoolChild>().ReturnToPool();

            transform.GetChild(_index[i] / lineCount).GetChild(_index[i] % lineCount).GetComponent<BoardFloor>().WarnPlayerSmthgBadIsComing();
        }
    }

}
