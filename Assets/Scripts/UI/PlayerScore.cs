using UnityEngine.UI;
using UnityEngine;

public class PlayerScore : MonoBehaviour {
    enum ScorePanel { PlayerIndex, Time, Points }

    public void SetScoreDefault(int _playerIndex, string _time, string _points)
    {
        Transform scorePanel = transform.GetChild(1);
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
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

    public void SetScoreMiniGameTimeOnly(int _playerIndex, string _time)
    {
        Transform scorePanel = transform.GetChild(1);
        float offset = scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition.y / 2.0f;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition = offset * Vector2.up;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Points).gameObject.SetActive(false);
        scorePanel.GetChild((int)ScorePanel.Time).localPosition = -offset * Vector2.up;
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().text = _time;

        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = 20;
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Outline>().enabled = true;
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<AnimText>().enabled = true;
    }

    public void SetScoreMiniGamePtsOnly(int _playerIndex, string _points)
    {
        Transform scorePanel = transform.GetChild(1);
        float offset = scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition.y / 2.0f;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition = offset * Vector2.up;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Time).gameObject.SetActive(false);
        scorePanel.GetChild((int)ScorePanel.Points).localPosition = -offset * Vector2.up;
        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().text = _points + "pts";

        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = 20;
        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Outline>().enabled = true;
        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<AnimText>().enabled = true;
    }

}
