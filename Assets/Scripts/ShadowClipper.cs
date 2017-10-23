using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowClipper : MonoBehaviour {

    Projector shadowProjector;
    float shadowDistanceTolerance  = 0.5f;
     
    private float origNearClipPlane;
    private float origFarClipPlane;
     
    void Start()
    {
        if (!shadowProjector) shadowProjector = transform.GetComponent<Projector>();
        origNearClipPlane = shadowProjector.nearClipPlane;
        origFarClipPlane = shadowProjector.farClipPlane;
    }

    void Update()
    {
        Ray ray = new Ray(shadowProjector.transform.position
                + shadowProjector.transform.forward.normalized * origNearClipPlane,
                shadowProjector.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, origFarClipPlane - origNearClipPlane,
                ~shadowProjector.ignoreLayers))
        {
            float dist = hit.distance + origNearClipPlane;
            shadowProjector.nearClipPlane = Mathf.Max(dist - shadowDistanceTolerance, 0);
            shadowProjector.farClipPlane = dist + shadowDistanceTolerance;
        }
    }
}
