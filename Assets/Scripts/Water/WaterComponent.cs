using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterComponent : MonoBehaviour {

    public float waterResistance;

    public GameObject WaterToActivateAtRuntime;

    public GameObject WaterParticleSystemToInstantiate;

    public void Start()
    {
        WaterToActivateAtRuntime.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null && other.GetComponent<Player>())
        {
            PlayerControllerHub playerController = other.GetComponent<PlayerControllerHub>();
            if (other.transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>() && other.transform.GetChild((int)PlayerChildren.SplashParticles).GetComponent<ParticleSystem>())
            {
                other.transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>().Play();
                other.transform.GetChild((int)PlayerChildren.SplashParticles).GetComponent<ParticleSystem>().Play();
                other.transform.GetChild((int)PlayerChildren.WaterTrailParticles).GetComponent<ParticleSystem>().Play();
            }
          
            playerController.underwaterState.waterLevel = transform.position.y;
            playerController.PlayerState = playerController.underwaterState;

        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.GetComponent<Rigidbody>() != null && other.GetComponent<Player>())
        {
            if (other.transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>())
            {
                other.transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>().Stop();
            }
            other.transform.GetChild((int)PlayerChildren.WaterTrailParticles).GetComponent<ParticleSystem>().Stop();

        }

    }

}
