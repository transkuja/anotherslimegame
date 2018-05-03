using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the food meter behavior
/// </summary>
public class FoodMeterHandler : MonoBehaviour {

    float maxScale;

    // Food meter settings
    public int foodMeterStep = 30;

    public float decreaseSpeed;
    public float decreaseSpeedWhenFullMultiplier;

    float[] foodMeters = new float[4];
    [SerializeField]
    PlayerControllerFood[] controllers;

    float[] timerResetFace = new float[4];
    bool[] doesFaceNeedReset = new bool[4];
    FaceEmotion[] resetTarget = new FaceEmotion[4];

	void Start () {
        maxScale = ((FoodGameMode)GameManager.Instance.CurrentGameMode).maxScale;
        for (int i = 0; i < 4; i++)
            foodMeters[i] = foodMeterStep;
    }
	
	void Update () {
        IsSliderValueNull();

        for (int i = 0; i < 4; ++i)
        {
            if (!doesFaceNeedReset[i])
                continue;

            timerResetFace[i] -= Time.deltaTime;
            if (timerResetFace[i] <= 0.0f)
            {
                GameManager.Instance.PlayerStart.PlayersReference[i].GetComponentInChildren<PlayerCosmetics>().FaceEmotion = resetTarget[i];
                doesFaceNeedReset[i] = false;
            }
        }
    }

    void IsSliderValueNull()
    {
        for (int i = 0; i < controllers.Length; i++)
        {
            if (foodMeters[i] <= foodMeterStep)
            {
                if (!controllers[i].areInputsUnlocked)
                {
                    decreaseSpeed /= decreaseSpeedWhenFullMultiplier;
                    controllers[i].areInputsUnlocked = true;
                    GameManager.Instance.PlayerStart.PlayersReference[i].GetComponentInChildren<PlayerCosmetics>().FaceEmotion 
                        = FaceEmotion.Neutral;
                }
            }
            else
            {
                foodMeters[i] -= Time.deltaTime * decreaseSpeed;
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.localScale 
                    = Vector3.one * Mathf.Max(foodMeters[i] * maxScale * 0.01f, 1.0f);

                if (foodMeters[i] <= 100 - (foodMeterStep) && 
                        GameManager.Instance.PlayerStart.PlayersReference[i].GetComponentInChildren<PlayerCosmetics>().FaceEmotion == FaceEmotion.Hit)
                    ResetFaceTo(FaceEmotion.Neutral, i);
            }
        }
    }

    public void FoodMeterIncrease(int _playerIndex)
    {
        foodMeters[_playerIndex] += foodMeterStep;
        GameObject currentPlayer = GameManager.Instance.PlayerStart.PlayersReference[_playerIndex];
        currentPlayer.GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Winner; // Should be "Eating"

        if (foodMeters[_playerIndex] >= 100 + foodMeterStep)
        {
            decreaseSpeed *= decreaseSpeedWhenFullMultiplier;
            controllers[_playerIndex].areInputsUnlocked = false;
            currentPlayer.GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Loser; // Ate too much
            doesFaceNeedReset[_playerIndex] = false;
            controllers[_playerIndex].CurrentCombo = 0;
        }
        else if (foodMeters[_playerIndex] >= 100 - (foodMeterStep * 2))
        {
            // clignote
            ResetFaceTo(FaceEmotion.Hit, _playerIndex); // Well fed
        }
        else
        {
            ResetFaceTo(FaceEmotion.Neutral, _playerIndex);
        }

    }

    void ResetFaceTo(FaceEmotion _target, int _playerIndex)
    {
        timerResetFace[_playerIndex] = 0.5f;
        resetTarget[_playerIndex] = _target;
        doesFaceNeedReset[_playerIndex] = true;
    }
}
