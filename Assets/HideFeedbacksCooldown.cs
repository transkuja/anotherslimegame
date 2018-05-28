using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HideFeedbacksCooldown : MonoBehaviour {

	void Start () {
        SceneManager.sceneLoaded += SuperFonction;
    }
	
    void SuperFonction(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "Podium" || SceneManager.GetActiveScene().name == "LoadingScreen")
        {
            for (int i = 0; i < 4; i++)
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SuperFonction;
    }

}
