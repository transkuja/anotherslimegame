using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class ControllersChecker : MonoBehaviour {

    int controllersConnected = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	IEnumerator Start () {

        while (true)
        {
            controllersConnected = 0;
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                    controllersConnected++;

            }
            Controls.keyboardIndex = controllersConnected;
            yield return new WaitForSeconds(1.0f);
        }
    }

}
