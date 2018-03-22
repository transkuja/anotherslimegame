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
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Outline>().enabled = false;

            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = fontSizes[0];
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Outline>().enabled = true;
        }
        else
        {
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = fontSizes[0];
            scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Outline>().enabled = true;

            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = fontSizes[1];
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Outline>().enabled = false;
        }
    }

    public void SetScoreMiniGameTimeOnly(int _playerIndex, string _time)
    {
        Transform scorePanel = transform.GetChild(0);
        float offset = scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition.y / 4;
        //scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition += offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Points).gameObject.SetActive(false);
        scorePanel.GetChild((int)ScorePanel.Time).localPosition -= offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().text = _time;

        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Text>().fontSize = fontSizes[0];
        scorePanel.GetChild((int)ScorePanel.Time).GetComponent<Outline>().enabled = true;
    }

    public void SetScoreMiniGamePtsOnly(int _playerIndex, string _points)
    {
        Debug.Log(transform.GetChild(0));
        Transform scorePanel = transform.GetChild(0);
        float offset = scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition.y / 4;
        //scorePanel.GetChild((int)ScorePanel.PlayerIndex).localPosition += offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.PlayerIndex).GetComponent<Text>().text = "P" + (_playerIndex + 1);
        scorePanel.GetChild((int)ScorePanel.Time).gameObject.SetActive(false);
        scorePanel.GetChild((int)ScorePanel.Points).localPosition -= offset * Vector3.up;
        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().text = _points + "pts";

        scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Text>().fontSize = fontSizes[0];
        if (scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Outline>())
            scorePanel.GetChild((int)ScorePanel.Points).GetComponent<Outline>().enabled = true;
    }

}
