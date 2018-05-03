using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the food meter behavior
/// </summary>
public class FoodMeterHandler : MonoBehaviour {

    // base scale 1
    // max scale
    float maxScale;
    // food meter % avec 0 => scale 1, 90 ==> max scale, start clignotement

    // Food meter settings
    public int foodMeterStep = 30;

    public float decreaseSpeed; // good feeling with 60
    public float decreaseSpeedWhenFullMultiplier; // with 60, 1.5 is quite good

    float[] foodMeters = new float[4];
    [SerializeField]
    PlayerControllerFood[] controllers;

	void Start () {
        maxScale = ((FoodGameMode)GameManager.Instance.CurrentGameMode).maxScale;
    }
	
	void Update () {
        IsSliderValueNull();
    }

    // Keep sliders for DEBUG, but slime will inflate instead
    void IsSliderValueNull()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            if (foodMeters[i] <= 1.0f)
            {
                if (!controllers[i].areInputsUnlocked)
                {
                    decreaseSpeed /= decreaseSpeedWhenFullMultiplier;
                    controllers[i].areInputsUnlocked = true;
                }
            }
            else
            {
                foodMeters[i] -= Time.deltaTime * decreaseSpeed;
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.localScale 
                    = Vector3.one * Mathf.Max(foodMeters[i] * maxScale * 0.01f, 1.0f);
            }
        }
    }

    public void FoodMeterIncrease(int _playerIndex)
    {
        foodMeters[_playerIndex] += foodMeterStep;
        GameObject currentPlayer = GameManager.Instance.PlayerStart.PlayersReference[_playerIndex];
        currentPlayer.GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Winner; // Should be "Eating"

        if (foodMeters[_playerIndex] >= 100)
        {
            decreaseSpeed *= decreaseSpeedWhenFullMultiplier;
            controllers[_playerIndex].areInputsUnlocked = false;
            StartCoroutine(ResetFaceTo(FaceEmotion.Loser, _playerIndex)); // Ate too much
        }
        else if (foodMeters[_playerIndex] >= 100 - (foodMeterStep * 2))
        {
            // clignote
            StartCoroutine(ResetFaceTo(FaceEmotion.Hit, _playerIndex)); // Well fed
        }
        else
        {
            StartCoroutine(ResetFaceTo(FaceEmotion.Neutral, _playerIndex));
        }

    }

    IEnumerator ResetFaceTo(FaceEmotion _target, int _playerIndex)
    {
        yield return new WaitForSeconds(0.33f);
        if (GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion == FaceEmotion.Winner)
            GameManager.Instance.PlayerStart.PlayersReference[_playerIndex].GetComponentInChildren<PlayerCosmetics>().FaceEmotion = _target;
    }
}
