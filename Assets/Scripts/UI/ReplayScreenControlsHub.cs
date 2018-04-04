using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReplayScreenControlsHub : MonoBehaviour {

    int cursor = 0;
    GamePadState prevControllerState;
    GamePadState controllerState;

    public HubMinigameHandler refMinigameHandler;
    public int index = -1;

    private void Update()
    {
        if (!refMinigameHandler || index ==-1)
            return; 

        prevControllerState = controllerState;
        controllerState = GamePad.GetState((PlayerIndex)index);

        if ((controllerState.ThumbSticks.Left.X > 0.5f && prevControllerState.ThumbSticks.Left.X < 0.5f)
            || (controllerState.ThumbSticks.Left.Y < -0.75f && prevControllerState.ThumbSticks.Left.Y > -0.75f)
            || (controllerState.ThumbSticks.Left.X < -0.5f && prevControllerState.ThumbSticks.Left.X > -0.5f)
            || (controllerState.ThumbSticks.Left.Y > 0.75f && prevControllerState.ThumbSticks.Left.Y < 0.75f))
        {
            if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

            cursor = (cursor+1)%2;
            transform.GetChild(cursor + 1).GetComponent<AnimButton>().enabled = true;
            transform.GetChild((cursor == 0) ? 2 : 1).GetComponent<AnimButton>().enabled = false;
        }

        if (prevControllerState.Buttons.A == ButtonState.Released && controllerState.Buttons.A == ButtonState.Pressed)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

            if (cursor == 0)
            {
                refMinigameHandler.LunchMinigameHub();
            }
            else
            {
                refMinigameHandler.CleanMinigameHub();
            }
        }
    }
}
