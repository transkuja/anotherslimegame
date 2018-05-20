using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingGameSpawner : MonoBehaviour {

    public bool withTrappedPots = false;

    [Tooltip("% of chance each pot will be trapped.")]
    [Range(5, 100)]
    [SerializeField]
    public float trapFrequency = 5.0f;

    int lineCount;
    int mapSize;
    BreakingGameMode gameMode;

    IEnumerator Start()
    {
        lineCount = transform.childCount;
        mapSize = FindObjectsOfType<BoardFloor>().Length;

        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => GameManager.CurrentState != GameState.ForcedPauseMGRules);
        gameMode = (BreakingGameMode)GameManager.Instance.CurrentGameMode;

        if (gameMode.curNbPlayers > 2)
            trapFrequency *= 2;
        else if (gameMode.curNbPlayers == 2)
            trapFrequency *= 1.5f;

        while (true)
        {
            yield return new WaitUntil(() => gameMode.activePots == 0);
            yield return new WaitForSeconds(0.25f);

            int[] randIndex = BoardSpawner.GetPattern((BoardSpawner.Pattern)Random.Range(0, (int)BoardSpawner.Pattern.Size));
            gameMode.activePots = randIndex.Length;
            Spawn(randIndex);
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

            if (gameMode.minigameVersion == 4)
                transform.GetChild(_index[i] / lineCount).GetChild(_index[i] % lineCount).GetComponent<BoardFloor>().WarnPlayerSmthgBadIsComing();
            else
            {
                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.BreakingPots).GetItem(
                    transform.GetChild(_index[i] / lineCount).GetChild(_index[i] % lineCount),
                    Vector3.up,
                    Quaternion.identity,
                    true
                );
            }
        }
    }

}
