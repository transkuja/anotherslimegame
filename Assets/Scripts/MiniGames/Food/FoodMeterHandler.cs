using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the food meter behavior
/// </summary>
public class FoodMeterHandler : MonoBehaviour {

    // Food meter settings
    public int foodMeterStep = 10;

    public float decreaseSpeed; // good feeling with 60
    public float decreaseSpeedWhenFullMultiplier; // with 60, 1.5 is quite good

    [SerializeField]
    Slider[] sliders;
    Image[] fillImages;
    PlayerControllerFood[] controllers;

	void Start () {
        fillImages = new Image[sliders.Length];
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].value = 0;
            fillImages[i] = sliders[i].transform.GetChild(1).GetComponentInChildren<Image>();
        }

        controllers = new PlayerControllerFood[(int)GameManager.Instance.PlayerStart.ActivePlayersAtStart];
        for (int i = 0; i < controllers.Length; i++)
            controllers[i] = GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<PlayerControllerFood>();
    }
	
	void Update () {
        IsSliderValueNull();
    }

    // Keep sliders for DEBUG, but slime will inflate instead
    void IsSliderValueNull()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            if (sliders[i].value <= 0.0f)
            {
                sliders[i].value = 0;
                fillImages[i].enabled = false;

                if (!controllers[i].areInputsUnlocked)
                {
                    decreaseSpeed /= decreaseSpeedWhenFullMultiplier;
                    controllers[i].areInputsUnlocked = true;
                }
            }
            else
            {
                if (!fillImages[i].enabled) fillImages[i].enabled = true;
                sliders[i].value -= Time.deltaTime * decreaseSpeed;
            }
        }
    }

    public void FoodMeterIncrease(int _playerIndex)
    {
        sliders[_playerIndex].value += foodMeterStep;
        if (sliders[_playerIndex].value >= sliders[_playerIndex].maxValue)
        {
            decreaseSpeed *= decreaseSpeedWhenFullMultiplier;
            GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponent<PlayerControllerFood>().areInputsUnlocked = false;
        }
    }
}
