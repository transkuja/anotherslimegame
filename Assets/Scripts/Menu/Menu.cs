using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(1);
        }
    }
}
