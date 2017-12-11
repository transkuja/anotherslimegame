using UnityEngine.UI;
using UnityEngine;

public class PlayerScore : MonoBehaviour {
    enum ScorePanel { PlayerIndex, Time, Points }
    public void SetScore(int _playerIndex, string _time, string _points)
    {
        Transform scorePanel = transform.GetChild(1);
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "J" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().text = _time;
        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().text = _points + "pts";

        if (GameManager.Instance.isTimeOver)
        {
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = 16;
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Outline>().enabled = false;
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<AnimText>().enabled = false;

            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = 20;
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Outline>().enabled = true;
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<AnimText>().enabled = true;
        }
        else
        {
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = 20;
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Outline>().enabled = true;
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<AnimText>().enabled = true;

            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = 16;
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Outline>().enabled = false;
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<AnimText>().enabled = false;
        }
    }
}
