using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReplayScreenControls : MonoBehaviour {

    int cursor = 0;
    GamePadState prevControllerState;
    GamePadState controllerState;

    private void Update()
    {
        prevControllerState = controllerState;
        controllerState = GamePad.GetState(0);

        if ((controllerState.ThumbSticks.Left.X > 0.5f && prevControllerState.ThumbSticks.Left.X < 0.5f)
            || (controllerState.ThumbSticks.Left.Y < -0.75f && prevControllerState.ThumbSticks.Left.Y > -0.75f)
            || (controllerState.ThumbSticks.Left.X < -0.5f && prevControllerState.ThumbSticks.Left.X > -0.5f)
            || (controllerState.ThumbSticks.Left.Y > 0.75f && prevControllerState.ThumbSticks.Left.Y < 0.75f))
        {
            if (AudioManager.Instance != null && AudioManager.Instance.changeOptionFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.changeOptionFx);

            cursor = (cursor+1)%2;
            // Show currently selected text
            //Color newColor = transform.GetChild(cursor + 1).GetComponent<Text>().color;
            //newColor.a = 1.0f;
            //transform.GetChild(cursor + 1).GetComponent<Text>().color = newColor;
            transform.GetChild(cursor + 1).GetComponent<AnimButton>().enabled = true;
            transform.GetChild((cursor == 0) ? 2 : 1).GetComponent<AnimButton>().enabled = false;

            // Unshow last selection
            //newColor = transform.GetChild((cursor == 0) ? 2 : 1).GetComponent<Text>().color;
            //newColor.a = 0.4f;
            //transform.GetChild((cursor == 0) ? 2 : 1).GetComponent<Text>().color = newColor;
        }

        if (prevControllerState.Buttons.A == ButtonState.Released && controllerState.Buttons.A == ButtonState.Pressed)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

            if (cursor == 0)
            {
                // Reload scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                if (GameManager.Instance.DataContainer.launchedFromMinigameScreen)
                {
                    SceneManager.LoadScene(0);
                }
                else
                {
                    SceneManager.LoadScene(1); // ugly?
                }
            }
        }
    }
}
