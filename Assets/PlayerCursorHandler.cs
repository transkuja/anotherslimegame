using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursorHandler : MonoBehaviour {

    [SerializeField]
    GameObject[] cursorRef;
    float tick = 0.1f;
    float curTick;

    RectTransform[] rect = new RectTransform[4];
    float scaleFactor;

    private void Start()
    {
        for (int i = 0; i < 4; ++i)
            rect[i] = transform.GetChild(i).GetComponent<RectTransform>();

        scaleFactor = GetComponent<Canvas>().scaleFactor;
    }

    private void OnEnable()
    {
        //curTick = 0.0f;    
    }

    void Update () {
        //curTick -= Time.deltaTime;
        //if (curTick < 0.0f)
        //{
        //    curTick = tick;
        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; ++i)
        {
            Vector2 computeDePommesition = Camera.main.WorldToScreenPoint(GameManager.Instance.PlayerStart.PlayersReference[i].transform.position) / scaleFactor;
            rect[i].anchoredPosition = new Vector2(computeDePommesition.x, computeDePommesition.y + Screen.height * 0.05f);
        }

        //cursorRef.transform.position = 
        //}
	}
}
