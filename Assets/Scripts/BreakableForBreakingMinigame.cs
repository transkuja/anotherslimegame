using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableForBreakingMinigame : Breakable {

    bool gameModeCalled = false;

    private void OnEnable()
    {
        gameModeCalled = false;
    }

    public override void PostCollisionProcess(PlayerCharacterHub _playerCharacterHub, PlayerControllerHub _playerControllerHub)
    {
        if (!gameModeCalled)
        {
            ((BreakingGameMode)GameManager.Instance.CurrentGameMode).activePots--;
            gameModeCalled = true;
            _playerCharacterHub.GetComponent<Player>().NbPoints++;
        }
    }
}
