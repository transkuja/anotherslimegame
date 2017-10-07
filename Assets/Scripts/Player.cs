using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Rigidbody rb;
    bool canDoubleJump = false;

    public Rigidbody Rb
    {
        get
        {
            return rb;
        }

        set
        {
            rb = value;
        }
    }

    public bool CanDoubleJump
    {
        get
        {
            return canDoubleJump;
        }

        set
        {
            canDoubleJump = value;
        }
    }

    void Start () {
        rb = GetComponent<Rigidbody>();
	}

}
