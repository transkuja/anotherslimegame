﻿using System.Collections;
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

    [SerializeField]
    int decreaseSpeed;
    int[] effectiveDecreaseSpeed = new int[4];
    int decreaseSpeedInitial;
    int decreaseSpeedBuffed;

    float[] foodMeters = new float[4];
    [SerializeField]
    PlayerControllerFood[] controllers;

    float[] timerResetFace = new float[4];
    bool[] doesFaceNeedReset = new bool[4];
    FaceEmotion[] resetTarget = new FaceEmotion[4];

    FoodGameMode gameMode;

    void Start () {
        gameMode = ((FoodGameMode)GameManager.Instance.CurrentGameMode);
        maxScale = gameMode.maxScale;
        decreaseSpeedInitial = decreaseSpeed;
        decreaseSpeedBuffed = (int)(decreaseSpeedInitial * 1.5f);
        for (int i = 0; i < 4; i++)
            effectiveDecreaseSpeed[i] = decreaseSpeed;
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
            if (foodMeters[i] <= 0.0f)
            {
                if (!controllers[i].AreInputsUnlocked)
                {
                    if (effectiveDecreaseSpeed[i] != decreaseSpeedInitial)
                        effectiveDecreaseSpeed[i] = decreaseSpeedInitial;
                    controllers[i].AreInputsUnlocked = true;
                    controllers[i].parentAnim.SetBool("wrong", false);
                    GameManager.Instance.PlayerStart.PlayersReference[i].GetComponentInChildren<PlayerCosmetics>().FaceEmotion 
                        = FaceEmotion.Neutral;
                }
            }
            else
            {
                foodMeters[i] -= Time.deltaTime * effectiveDecreaseSpeed[i];
                controllers[i].CurrentCombo = 1 + (foodMeters[i] * (maxScale - 1) / 100);
                GameManager.Instance.PlayerStart.PlayersReference[i].transform.localScale 
                    = Vector3.one * controllers[i].CurrentCombo;

                GameManager.Instance.PlayerStart.PlayersReference[i].transform.position 
                    = new Vector3(GameManager.Instance.PlayerStart.PlayersReference[i].transform.position.x,
                            GameManager.Instance.PlayerStart.PlayersReference[i].transform.position.y, 
                            (gameMode.startingPositions[i].z + controllers[i].CurrentCombo - 1));

                if (foodMeters[i] <= 100 - (foodMeterStep * 2) && 
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

        if (foodMeters[_playerIndex] >= 100)
        {
            effectiveDecreaseSpeed[_playerIndex] = decreaseSpeedBuffed;
            controllers[_playerIndex].AreInputsUnlocked = false;
            controllers[_playerIndex].parentAnim.SetBool("wrong", true);
            currentPlayer.GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Loser; // Ate too much
            doesFaceNeedReset[_playerIndex] = false;
            controllers[_playerIndex].CurrentCombo = 1.0f;
            if (AudioManager.Instance != null && AudioManager.Instance.incorrectFx != null)
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.incorrectFx);
        }
        else if (foodMeters[_playerIndex] >= 100 - (foodMeterStep * 3))
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
