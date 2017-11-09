using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterComponent : MonoBehaviour {
    public Vector3 buoyancyCentreOffset;
    public float bounceDamp;
    public float waterLevel;
    public float floatHeight;

    private void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            Vector3 actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
            float forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);

            if (forceFactor > 0f)
            {
                Vector3 uplift = -Physics.gravity * (forceFactor - rigidbody.velocity.y * bounceDamp);
                rigidbody.AddForceAtPosition(uplift, actionPoint);
            }
        }
    }
    

}
