using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
public class HubMinigameHandler : MonoBehaviour {

    // gamePad
    protected GamePadState state;
    protected GamePadState prevState;


    float indexStored;
    // PNJ Character 
    public void MessageTest(float indexPlayer)
    {
        GameManager.ChangeState(GameState.ForcedPauseMGRules);
        GameObject go = Instantiate(ResourceUtils.Instance.feedbacksManager.prefabReplayScreenHub, GameManager.UiReference.transform);
        //go.transform.GetChild(1).GetComponentInChildren<Text>().text = "BIIIITE";
        indexStored = indexPlayer;
    }

    public void Update()
    {
        if (GameManager.CurrentState == GameState.ForcedPauseMGRules)
        {
            prevState = state;
            state = GamePad.GetState((PlayerIndex)indexStored);

            if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed)
            {
                GameManager.ChangeState(GameState.Normal);

            }
        }
    }
}
