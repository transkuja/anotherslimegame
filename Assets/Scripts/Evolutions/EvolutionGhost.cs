using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionGhost : EvolutionComponent
{
    Vector3 lastPlaneSpawnPosition;
    GameObject lastSpawnedPlane;
    [SerializeField]
    float maxDistanceFromLastPlane = 8.0f;
    public override void Start()
    {
        base.Start();
        lastPlaneSpawnPosition = Vector3.zero;
        gameObject.layer = LayerMask.NameToLayer("GhostPlayer");
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnCollisionEnter(Collision col)
    {
        if(col.collider.GetComponent<Ground>())
        {
            //Spawn splippery plane here
            lastPlaneSpawnPosition = col.contacts[0].point;
            
            //tmp to test
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.localScale = new Vector3(2.0f, 2.0f, 0.2f);
            go.transform.position = col.contacts[0].point + 0.01f* (col.contacts[0].normal);
            go.transform.rotation = Quaternion.LookRotation(col.contacts[0].normal, Vector3.forward);
            go.GetComponent<BoxCollider>().isTrigger = true;
            lastSpawnedPlane = go;
            go.transform.parent = col.collider.transform;
            Destroy(go, 10.0f);
        }
    }

    public override void OnCollisionStay(Collision col)
    {
        if (col.collider.GetComponent<Ground>())
        {
            // Spawn splippery plane here when max distance from previous spawned plane is reached
            if(lastSpawnedPlane == null || Vector3.Distance(transform.position, lastSpawnedPlane.transform.position) > maxDistanceFromLastPlane)
            {
                lastPlaneSpawnPosition = col.contacts[0].point;
                //tmp to test
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                
                
                go.transform.position = col.contacts[0].point + 0.01f * (col.contacts[0].normal);
                go.transform.rotation = Quaternion.LookRotation(-col.contacts[0].normal, Vector3.forward);
                go.GetComponent<BoxCollider>().isTrigger = true;
                lastSpawnedPlane = go;
                go.transform.parent = col.collider.transform;
                go.transform.localScale = new Vector3(2.0f, 2.0f, 0.2f);
                Destroy(go, 10.0f);
            }
        }
    }
}
