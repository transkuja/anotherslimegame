using UnityEngine;
using UnityEngine.UI;
using System;

public class UI : MonoBehaviour {

    Text evolutionText;
    Text timerText;

    private void Start()
    {
        GameManager.UiReference = this;
        evolutionText = GetComponentInChildren<Text>();
        timerText = transform.GetChild(1).GetComponent<Text>();
    }

    public void NeedUpdate(string value)
    {
        evolutionText.text = value;
    }

    public void TimerNeedUpdate(float _currentGameFinalTimer)
    {
        if (!timerText.gameObject.activeInHierarchy) timerText.gameObject.SetActive(true);

        int minutes = Mathf.FloorToInt(_currentGameFinalTimer / 60);
        int seconds = (int)_currentGameFinalTimer % 60;
        String timeStr = string.Format("{0:00}:{1:00}", minutes, seconds);

        timerText.text = timeStr;
    }
}
