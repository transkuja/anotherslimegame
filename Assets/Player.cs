using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Rigidbody rb;

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

    void Start () {
        rb = GetComponent<Rigidbody>();
	}

}
