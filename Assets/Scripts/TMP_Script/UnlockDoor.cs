using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockDoor : MonoBehaviour
{
    public Text timer;
    public int minute;
    public int secondes;

    // Use this for initialization
    void Start () {
        minute = 1;
        secondes = 0;
        timer.text = "0" + minute + " : 0" + secondes;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
