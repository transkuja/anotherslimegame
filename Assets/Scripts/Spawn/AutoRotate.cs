using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

    public float xspeed = 0.5f;
    public float yspeed = 0.0f;
    public float zspeed = 0.0f;

    private void OnWillRenderObject()
    {
        transform.Rotate(
             xspeed * Time.deltaTime,
             yspeed * Time.deltaTime,
             zspeed * Time.deltaTime
        );
    }
}
