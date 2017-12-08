using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CarAxle
{
    public WheelCollider wheelR;
    public WheelCollider wheelL;
    public bool motor;
    public bool steer;
}

public class CarController : MonoBehaviour {

    public float maxMotor;
    public float maxSteer;

    public CarAxle[] axles;
    public Collider[] ownColliders;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass -= Vector3.down;
        Gizmos.DrawSphere(GetComponent<Rigidbody>().centerOfMass, 0.5f);
        foreach(CarAxle axle in axles)
        {
            foreach(Collider c in ownColliders)
            {
                Physics.IgnoreCollision(axle.wheelR, c);
                Physics.IgnoreCollision(axle.wheelL, c);
            }
        }
    }

    private void FixedUpdate()
    {
        float motor = Input.GetAxis("Vertical") * maxMotor;
        float steer = Input.GetAxis("Horizontal") * maxSteer;

        foreach (CarAxle axle in axles)
        { 
            if(axle.motor)
            {
                axle.wheelR.motorTorque = motor;
                axle.wheelL.motorTorque = motor;
            }

            if(axle.steer)
            {
                axle.wheelR.steerAngle = steer;
                axle.wheelL.steerAngle = steer;
            }
            AlignWheel(axle.wheelL);
            AlignWheel(axle.wheelR);
        }
    }

    private void Update()
    {
        foreach (CarAxle axle in axles)
        {
            axle.wheelR.wheelDampingRate = 350.0f;
            axle.wheelL.wheelDampingRate = 350.0f;
        }
    }

    void AlignWheel(WheelCollider wheel)
    {
        if(wheel.transform.childCount > 0)
        {
            Transform pivot = wheel.transform.GetChild(0);

            Vector3 position;
            Quaternion rotation;

            wheel.GetWorldPose(out position, out rotation);
            pivot.position = position;
            pivot.rotation = rotation;
        }
    }

}
