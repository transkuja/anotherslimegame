using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursorHandlerFood : PlayerCursorHandler {

    public float timerStopUpdate = 3.0f;

    public override void Update () {
        if (timerStopUpdate <= 0.0f)
            return;

        timerStopUpdate -= Time.deltaTime;
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; ++i)
        {
            Vector2 computeDePommesition = Camera.main.WorldToScreenPoint(GameManager.Instance.PlayerStart.PlayersReference[i].transform.position + Vector3.up) / scaleFactor;
            rect[i].anchoredPosition = new Vector2(computeDePommesition.x, computeDePommesition.y + Screen.height * 0.025f);
        }
	}
}
