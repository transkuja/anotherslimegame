using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // Nouvelle idée : 
 // Une feuille qui tombe avec un mouvement de feuille.
 // platforme qui bouge


// instant --> Tombe tout de suite
// timer --> Tombe dans tant de temps. 
// réapparait au bout d'un certain temps

    // donner la meme gravié que le joueur pour tomber + vite. 
    // 

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour {

   
    [SerializeField] private float respawnDelay = 5;
    [SerializeField] private float fallDelay;
    bool mustFall;
    float timer;

    Vector3 startPosition;
    Quaternion startOrientation;
    Rigidbody rb;
    // Use this for initialization
    public void Awake()
    {
        mustFall = false;
        timer = fallDelay;
        startPosition = transform.position;
        startOrientation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
    public void Reset()
    {
        transform.position = startPosition;
        transform.rotation = startOrientation;
        rb.isKinematic = true;
        mustFall = false;
        GetComponent<Collider>().enabled = true;

    }

    // Update is called once per frame
    void Update () {

        if (mustFall)
        {
            timer -= Time.deltaTime;
            if (timer < 0) 
            {
                Fall();
            }
            if (timer < -respawnDelay)
            {
                Reset();
            }
        }
		
	}
    void Fall()
    {
        rb.isKinematic = false;
        GetComponent<Collider>().enabled = false;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.GetComponent<Player>()!=null)
        {
            mustFall = true;
            timer = fallDelay;
        }
    }
}
