using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterComponent : MonoBehaviour {
    public Vector3 buoyancyCentreOffset;
    public float bounceDamp;
    public float waterLevel;
    public float floatHeight;
    public float compensationGravity;

    StatBuff movestatbuff;
    StatBuff dashstatbuff;
    StatBuff jumpstatbuff;
    public float tolerance;
    public float waterResistance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            if(other.transform.childCount == 3 && other.transform.GetChild(2).GetComponent<ParticleRenderer>())
                // SEB C'est pour toi
            other.transform.GetChild(2).gameObject.SetActive(true);

            if (waterResistance != 0)
            {
                movestatbuff = new StatBuff(Stats.StatType.GROUND_SPEED, waterResistance, -1);
                other.GetComponent<PlayerController>().stats.AddBuff(movestatbuff);

                dashstatbuff = new StatBuff(Stats.StatType.DASH_FORCE, waterResistance, -1);
                other.GetComponent<PlayerController>().stats.AddBuff(dashstatbuff);


                jumpstatbuff = new StatBuff(Stats.StatType.JUMP_HEIGHT, waterResistance, -1);
                other.GetComponent<PlayerController>().stats.AddBuff(jumpstatbuff);
            }


        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();

            // Niveau de floattabilté, fonction de la hauteur du joueur
            Vector3 actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);

            float forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);
            Vector3 gravity = new Vector3(0, other.GetComponent<JumpManager>().GetGravity(), 0) * compensationGravity;
            if (other.transform.position.y > waterLevel - tolerance)
            {
                other.GetComponent<PlayerController>().IsGrounded = true;
            } else
            {
                if (forceFactor > 0f)
                {
                    Vector3 uplift = gravity * ((forceFactor - rigidbody.velocity.y) * bounceDamp);


                    rigidbody.AddForceAtPosition(uplift, actionPoint);
                }
            }


      
        }

    }

    private void OnTriggerExit(Collider other)
    {


        if (other.transform.childCount == 3 && other.transform.GetChild(2).GetComponent<ParticleRenderer>())
            // SEB C'est pour toi
            other.transform.GetChild(2).gameObject.SetActive(false);


        if (waterResistance != 0)
        {
            // TODO : Need a contains buff ?
            other.GetComponent<PlayerController>().stats.RemoveBuff(movestatbuff);
            other.GetComponent<PlayerController>().stats.RemoveBuff(dashstatbuff);
            other.GetComponent<PlayerController>().stats.RemoveBuff(jumpstatbuff);
        }
    }
}
