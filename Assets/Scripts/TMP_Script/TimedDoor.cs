using UnityEngine;
using UnityEngine.UI;

public class TimedDoor : MonoBehaviour, IDoor
{
    public Text timer;
    private int minutes;
    private int seconds;
    public float time= 50.0f;

    // Use this for initialization
    void Start () {
        minutes = Mathf.FloorToInt(time / 60);
        seconds = (int)time % 60;

        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
	
	// Update is called once per frame
	void Update () {
        if (time > 0.0f)
        {
            time -= Time.deltaTime;

            minutes = Mathf.FloorToInt(time / 60);
            seconds = (int)time % 60;

            timer.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
        else
        {
            OpenDoor();
        }
}

    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }
}
