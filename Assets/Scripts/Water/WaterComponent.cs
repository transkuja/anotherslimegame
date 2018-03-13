using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterComponent : MonoBehaviour {

    StatBuff movestatbuff;
    StatBuff dashstatbuff;
    StatBuff jumpstatbuff;
    public float waterResistance;

    public GameObject WaterToActivateAtRuntime;

    public GameObject WaterParticleSystemToInstantiate;

    public void Start()
    {
        WaterToActivateAtRuntime.SetActive(true);

        movestatbuff = new StatBuff(Stats.StatType.GROUND_SPEED, waterResistance, -1, "water_move_debuff");
        dashstatbuff = new StatBuff(Stats.StatType.DASH_FORCE, waterResistance, -1, "water_dash_debuff");
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
            PlayerControllerHub playerController = other.GetComponent<PlayerControllerHub>();
            if (other.transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>())
            {
                other.transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>().Stop();
            }
            other.transform.GetChild((int)PlayerChildren.WaterTrailParticles).GetComponent<ParticleSystem>().Stop();

        }
    }

}
