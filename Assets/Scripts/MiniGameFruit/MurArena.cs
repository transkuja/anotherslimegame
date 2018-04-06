using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurArena : MonoBehaviour {

    public GameObject objectWhoCollide;


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void OnCollisionEnter(Collision collision)
    {
        objectWhoCollide = collision.gameObject;
        if(objectWhoCollide.GetComponent<FruitType>())
        {
            Physics.IgnoreCollision(objectWhoCollide.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}
