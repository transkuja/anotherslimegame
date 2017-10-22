using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeModeDoor : MonoBehaviour, IDoor {



    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }
}
