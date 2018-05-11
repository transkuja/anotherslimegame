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
        if (GameManager.Instance.PlayerStart.PlayersReference.Count != 3)
            Destroy(this.gameObject);
    }

}
