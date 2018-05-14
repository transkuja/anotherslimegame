using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nagePetitPoisson : MonoBehaviour {


    float i = 0.0f;
    float SineSpeedX;
    float SineSpeedZ;
    float SineDistanceX;
    float SineDistanceZ;
    float offsetX;
    float offsetZ;
    public Vector3 startPos;
    // Use this for initialization
    void Start()
    {
        i = 0.0f;
        startPos = transform.position;
        SineSpeedX = Random.Range(30.0f, 50.0f);
        SineSpeedZ = Random.Range(30.0f, 50.0f);
        SineDistanceX = Random.Range(0.5f, 0.7f);
        SineDistanceZ = Random.Range(0.5f, 0.7f);
 
    }

    // Update is called once per frame
    void Update()
    {
        i += Time.deltaTime;

        if (i > 50.0f)
        {
            i = 0;
            offsetX = Random.Range(-0.5f, 0.5f);
            offsetZ = Random.Range(-0.5f, 0.5f);
        }
        Vector3 oldpos = transform.position;
        Vector3 targetPos;

        targetPos.x = startPos.x + Mathf.Sin(i * Mathf.PI * SineSpeedX / 180) * SineDistanceX + offsetX;
        targetPos.y = startPos.y;
        targetPos.z = startPos.z + Mathf.Sin(i * Mathf.PI * SineSpeedZ / 180) * SineDistanceZ + offsetZ;
        transform.position = targetPos;
        transform.LookAt(transform.position + (targetPos - oldpos).normalized);

    }
}
