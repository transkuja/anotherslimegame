using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;
using UnityEngine.SceneManagement;

public class ControllersChecker : MonoBehaviour {

    [SerializeField]
    int controllersConnected = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	IEnumerator Start () {

        while (true)
        {
            // Controllers are not swapped once the game has started
            if (SceneManager.GetActiveScene().name == "Menu")
            {
                controllersConnected = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (GamePad.GetState((PlayerIndex)i).IsConnected)
                        controllersConnected++;

                }
                Controls.keyboardIndex = controllersConnected;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

}
