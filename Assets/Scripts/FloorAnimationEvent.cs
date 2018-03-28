using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorAnimationEvent : MonoBehaviour {

	public void StopAnimation()
    {
        GetComponent<Animator>().SetBool("animate", false);
    }
}
