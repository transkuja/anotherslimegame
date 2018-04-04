﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class HubMinigameHandler : MonoBehaviour {

    public GameObject BbuttonShown;
    public GameObject Message;

    // gamePad
    protected GamePadState state;
    protected GamePadState prevState;

    GameObject message;

    public void MessageTest(int indexPlayer)
    {
        BbuttonShown.SetActive(false);
        Message.SetActive(true);
        //GameManager.ChangeState(GameState.ForcedPauseMGRules);
        //message = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        //message.GetComponent<ReplayScreenControlsHub>().index = indexPlayer;
    }


    public void LunchMinigameHub()
    {
        // Fade....
        GameManager.ChangeState(GameState.Normal);
    }

    public void StopMinigameHub()
    {

    }
}
