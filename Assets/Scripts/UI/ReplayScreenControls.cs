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

    [SerializeField]
    GameObject menuCursor;

    private void Start()
    {
        UpdateCursor(0);
    }

    void UpdateCursor(int _cursorIndex)
    {
        menuCursor.transform.SetParent(transform.GetChild(_cursorIndex + 1));
        menuCursor.transform.localPosition = Vector3.zero;
        menuCursor.transform.localScale = Vector3.one;
        menuCursor.transform.localRotation = Quaternion.identity;
        Rect textBox = transform.GetChild(_cursorIndex + 1).GetComponentInChildren<Text>().GetComponent<RectTransform>().rect;

        menuCursor.transform.GetChild(0).localPosition = new Vector3(textBox.xMin, -1.0f, 0.0f);
        menuCursor.transform.GetChild(1).localPosition = new Vector3(textBox.xMax, -1.0f, 0.0f);
    }

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
            UpdateCursor(cursor);
        }

        if (prevControllerState.Buttons.A == ButtonState.Released && controllerState.Buttons.A == ButtonState.Pressed)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.buttonValidationFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.buttonValidationFx);

            if (cursor == 0)
            {
                // Reload scene
                GameManager.Instance.previousScene = SceneManager.GetActiveScene().name;
                LevelLoader.LoadLevelWithFadeOut(SceneManager.GetActiveScene().name);
            }
            else
            {
                GameManager.Instance.previousScene = "";
                if (GameManager.Instance.DataContainer.launchedFromMinigameScreen)
                {
                    LevelLoader.LoadLevelWithFadeOut("Menu");
                }
                else
                {
                    LevelLoader.LoadLevelWithLoadingScreen("Hub"); // ugly?
                }
            }
        }
    }
}
