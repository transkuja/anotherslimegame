using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableForBreakingMinigame : Breakable {

    bool gameModeCalled = false;

    [SerializeField]
    GameObject rabbit;

    BreakingGameMode gameModeRef;

    private void OnEnable()
    {
        gameModeCalled = false;
    }

    public override bool OverrideDestruction(PlayerCharacterHub _playerCharacterHub)
    {
        if (_playerCharacterHub.GetComponent<EnnemyController>())
            return true;
        return false;
    }

    public override void PostCollisionProcess(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub)
    {
        if (!gameModeCalled)
        {
            gameModeRef = ((BreakingGameMode)GameManager.Instance.CurrentGameMode);
            gameModeRef.ActivePots--;

            gameModeCalled = true;
            _playerCharacterHub.GetComponent<Player>().NbPoints++;
            if (gameModeRef.withTrappedPots && Random.Range(0, 100) < gameModeRef.boardReference.GetComponent<BreakingGameSpawner>().trapFrequency)
                Invoke("DelayedSummonRabbit", 0.3f);
        }
    }

    void DelayedSummonRabbit()
    {
        Instantiate(rabbit, transform.position + Vector3.up, Quaternion.identity);
    }
}
