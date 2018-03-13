using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class tmpRemi : MonoBehaviour {

    public PhysicMaterial physicMat;

	// Use this for initialization
	void Start () {
        Collider[] bite = FindObjectsOfType<Collider>();

        foreach (Collider b in bite)
        {
            if (!b.isTrigger)
            {
                b.sharedMaterial = physicMat;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
