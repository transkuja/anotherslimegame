using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodInputSettings : MonoBehaviour {

    // 2 states, Attached/NotAttached
    // 3 behaviours for "tail"
    // -- Attached && tail length < window => reduce tail length over time
    // -- Attached && tail length > window => store tail length, decrease the value but not the visual until it reaches the window size
    // -- Detached => increase tail length until it reaches its max value or the socket

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
