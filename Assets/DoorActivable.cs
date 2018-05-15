using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActivable : Activable {

    private Vector3 scalePorteOrigin;
    private Vector3 scalePorteArrive;

    private bool active = false;
    public float timer = 2.0f;
    private float currentTimer = 0.0f;



    public override void Active(bool _active)
    {
        active = true;
        isActive = _active;
        currentTimer = 0.0f;
    }

    // Use this for initialization
    void Start () {
        scalePorteOrigin = transform.localScale;
        scalePorteArrive = new Vector3(scalePorteOrigin.x, 0, scalePorteOrigin.z);
    }
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            currentTimer += Time.deltaTime;
            if (isActive)
            {
                transform.localScale = Vector3.Lerp(scalePorteOrigin, scalePorteArrive, currentTimer / timer);
            }
            else
            {
                transform.localScale = Vector3.Lerp(scalePorteArrive, scalePorteOrigin, currentTimer / timer);
            }

            if(currentTimer > timer)
            {
                active = false;
            }
        }
    }
}
