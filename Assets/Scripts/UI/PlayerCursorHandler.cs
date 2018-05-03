using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursorHandler : MonoBehaviour {

    [SerializeField]
    protected GameObject[] cursorRef;

    protected RectTransform[] rect = new RectTransform[4];
    protected float scaleFactor;

    public virtual void Start()
    {
        for (int i = 0; i < 4; ++i)
            rect[i] = transform.GetChild(i).GetComponent<RectTransform>();

        scaleFactor = GetComponent<Canvas>().scaleFactor;
    }

    public virtual void Update () {

        for (int i = 0; i < GameManager.Instance.PlayerStart.ActivePlayersAtStart; ++i)
        {
            Vector2 computeDePommesition = Camera.main.WorldToScreenPoint(GameManager.Instance.PlayerStart.PlayersReference[i].transform.position) / scaleFactor;
            rect[i].anchoredPosition = new Vector2(computeDePommesition.x, computeDePommesition.y + Screen.height * 0.025f);
        }
	}
}
