using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

    private Rigidbody rb;
    private PlayerController pc;
    private Animator anim;

    public Rigidbody Rb
    {
        get
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            return rb;
        }
    }

    public PlayerController Pc
    {
        get
        {
            return pc;
        }

        set
        {
            pc = value;
        }
    }

    public Animator Anim
    {
        get
        {
            if(anim == null)
                anim = GetComponentInChildren<Animator>();
            return anim;
        }

        set
        {
            anim = value;
        }
    }

    // Use this for initialization
    void Awake () {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
	
}
