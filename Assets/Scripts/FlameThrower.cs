using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour {
    enum Direction
    {
        Left,
        Right
    }

    ParticleSystem[] particles;
    Collider flamesTrigger;
    Animator anim;

    [SerializeField]
    float maxActiveTime = 1.0f; 

    [SerializeField]            
    float maxOffTime = 1.0f;

    [SerializeField]
    float startTimeOffset = 1.0f;

    [SerializeField]
    float expulsionForce = 150.0f;

    [SerializeField]
    Direction expulseDirection;

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

    private void OnTriggerStay(Collider other)
    {
        
        if (other.GetComponent<PlayerControllerKart>())
        {
            other.GetComponent<PlayerControllerKart>().CurrentState = PlayerControllerKart.KartPlayerState.Hit;
            Vector3 expulseDir;

            if (expulseDirection == Direction.Left)
            {
                expulseDir = (transform.localToWorldMatrix * Vector3.left).normalized;
            }
            else
            {
                expulseDir = (transform.localToWorldMatrix * Vector3.right).normalized;
            }
             
            other.GetComponent<Rigidbody>().AddForce(expulseDir * expulsionForce + transform.up * expulsionForce/2.0f, ForceMode.Impulse);
        }
        else if (other.GetComponent<AIRabite>())
        {
            other.GetComponent<AIRabite>().CurrentState = AIRabite.RabiteState.Dead;
        }
    }
}
