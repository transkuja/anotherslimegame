using UnityEngine.UI;
using UnityEngine;

public class PlayerScore : MonoBehaviour {
    enum ScorePanel { PlayerIndex, Time, Points }
    [SerializeField]
    int[] fontSizes;

    public void SetScoreDefault(int _playerIndex, string _time, string _points)
    {
        Transform scorePanel = transform.GetChild(0);
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().text = _time;
        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().text = _points + "pts";

        if (GameManager.Instance.isTimeOver)
        {
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = fontSizes[1];

            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = fontSizes[0];
        }
        else
        {
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = fontSizes[0];

            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = fontSizes[1];
        }
    }

    public void SetScoreMiniGameTimeOnly(int _playerIndex, string _time, bool _isPlayingAlone = false)
    {
        Transform scorePanel = transform.GetChild(0);
        float offset = scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition.y / 4;
        //scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition += offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Points).gameObject.SetActive(false);
        scorePanel.GetChild((int)ScorePanel.Time).gameObject.SetActive(true);
        scorePanel.GetChild((int)ScorePanel.Time).localPosition -= offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().text = _time;

        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = fontSizes[0];

        if (_isPlayingAlone)
        {
            scorePanel.GetChild((int)ScorePanel.PlayerIndex).gameObject.SetActive(false);
            scorePanel.GetChild((int)ScorePanel.Time).localPosition += scorePanel.GetChild(3).localPosition;
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize += 20;
        }
    }

    public void SetScoreMiniGamePtsOnly(int _playerIndex, string _points, bool _isPlayingAlone = false)
    {
        Transform scorePanel = transform.GetChild(0);
        float offset = scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition.y / 4;
        //scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition += offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Time).gameObject.SetActive(false);
        scorePanel.GetChild((int)ScorePanel.Points).gameObject.SetActive(true);
        scorePanel.GetChild((int)ScorePanel.Points).localPosition -= offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().text = _points + "pts";

        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = fontSizes[0];

        if (_isPlayingAlone)
        {
            scorePanel.GetChild((int)ScorePanel.PlayerIndex).gameObject.SetActive(false);
            scorePanel.GetChild((int)ScorePanel.Points).localPosition += scorePanel.GetChild(3).localPosition;
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize += 20;
        }
    }

}
