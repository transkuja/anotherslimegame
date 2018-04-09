using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public float timerRotation = 0.0f;
    public float[] vectorActions;

    // Use this for initialization
    void Start ()
    {
        vectorActions = new float[2] { 0.4f, -1.5f };
    }
	
	// Update is called once per frame
	void Update ()
    {
        timerRotation += Time.deltaTime;
        if(timerRotation > 1.0f)
        {
            float action_z = 2f * Mathf.Clamp(vectorActions[0], -1f, 1f);
                transform.Rotate(new Vector3(0, 0, 1), action_z);
            float action_x = 2f * Mathf.Clamp(vectorActions[1], -1f, 1f);
                transform.Rotate(new Vector3(1, 0, 0), action_x);
            
            transform.Rotate(new Vector3(2.0f, 7.0f, 0.0f));

            timerRotation = 0.0f;
        }
	}
}
