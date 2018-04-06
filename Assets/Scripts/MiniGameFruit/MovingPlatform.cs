using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public GameObject pivot;
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
        if(timerRotation > 0.5f)
        {
            float action_z = 2f * Mathf.Clamp(vectorActions[0], -1f, 1f);
           /* if ((gameObject.transform.rotation.z < 0.25f && action_z > 0f) ||
                (gameObject.transform.rotation.z > -0.25f && action_z < 0f))
            {*/
                gameObject.transform.Rotate(new Vector3(0, 0, 1), action_z);
            //}
            float action_x = 2f * Mathf.Clamp(vectorActions[1], -1f, 1f);
            /*if ((gameObject.transform.rotation.x < 0.25f && action_x > 0f) ||
                (gameObject.transform.rotation.x > -0.25f && action_x < 0f))
            {*/
                gameObject.transform.Rotate(new Vector3(1, 0, 0), action_x);
            //}


            //transform.Rotate(pivot.transform.position, 30.0f);
            transform.Rotate(new Vector3(2.0f, 7.0f, 0.0f));
            timerRotation = 0.0f;
        }
	}
}
