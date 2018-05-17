using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDestroy : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        RemoveBlackScreen();
    }
    void RemoveBlackScreen()
    {
        if (GameManager.Instance.DataContainer)
        {
            if (GameManager.Instance.DataContainer.nbPlayers != 3)
                Destroy(this.gameObject);
        }
        else
            if (GameManager.Instance.ActivePlayersAtStart != 3)
                Destroy(this.gameObject);
    }

}
