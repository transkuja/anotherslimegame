using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour {
    ParticleSystem[] particles;
    Collider flamesTrigger;
    Animator anim;

    [SerializeField]
    float maxActiveTime = 1.0f; 

    [SerializeField]            
    float maxOffTime = 1.0f;

    [SerializeField]
    float startTimeOffset = 1.0f;

    float timer = 0.0f;
    bool isActive = false;

    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            if(value)
            {
                foreach(ParticleSystem ps in particles)
                {
                    ps.Play();
                }
                anim.Play("ActivateFlameTrigger", 0);
            }
            else
            {
                foreach (ParticleSystem ps in particles)
                {
                    ps.Stop();
                }
                anim.Play("StopFlameTrigger", 0);
            }
            isActive = value;
        }
    }
    
    void Start () {
        particles = GetComponentsInChildren<ParticleSystem>();
        flamesTrigger = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        IsActive = false;
        timer = maxOffTime - startTimeOffset;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        if((IsActive && timer > maxActiveTime)||(!IsActive && timer > maxOffTime))
        {
            timer = 0.0f;
            IsActive = !IsActive;
        }
	}
}
