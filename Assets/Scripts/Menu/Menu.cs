using XInputDotNetPure;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    // Controls

    public GameObject playerStart;
    private bool isSceneSet = false;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (isSceneSet) return;

        if (Input.anyKey)
        {
            SetScene();
        }
    }

    public void ToogleCountdownText(bool visible)
    {
        transform.GetChild(2).gameObject.SetActive(visible);
    }

    public void RefreshCountDown(float countdown)
    {
        int minutes = Mathf.FloorToInt(countdown / 60);
        int seconds = (int)countdown % 60;

        transform.GetChild(2).GetComponent<Text>().text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    public void SetScene()
    {
        isSceneSet = true;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        playerStart.gameObject.SetActive(true);
    }
}
