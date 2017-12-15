using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUBManager : MonoBehaviour {

    public GameObject CostAreaMiniGamePush;

    public void Start()
    {
        UpdateHUBWithData(GameManager.Instance.unlockedMinigames);
    }

    public void UpdateHUBWithData(bool[] _activateMinigames)
    {
        if (_activateMinigames[(int)MiniGame.KickThemAll]) CostAreaMiniGamePush.GetComponent<CostArea>().UnlockAssociatedMinigame();
    }
}
