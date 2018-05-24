using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLogo : MonoBehaviour {
    public float rotationSpeed = 40f;
	void Update () {
        transform.eulerAngles -= Vector3.forward * Time.unscaledDeltaTime * rotationSpeed;
	}
}
