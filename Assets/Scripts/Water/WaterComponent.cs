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
        jumpstatbuff = new StatBuff(Stats.StatType.JUMP_HEIGHT, waterResistance, -1, "water_jump_debuff");
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
     
            
            if (waterResistance != 0)
            {

                playerController.stats.AddBuff(movestatbuff);
                playerController.stats.AddBuff(dashstatbuff);
                playerController.stats.AddBuff(jumpstatbuff);
            }

            playerController.IsUnderWater = true;
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

            if (waterResistance != 0)
            {
                // TODO : Need a contains buff ?
                playerController.stats.RemoveBuff(movestatbuff);
                playerController.stats.RemoveBuff(dashstatbuff);
                playerController.stats.RemoveBuff(jumpstatbuff);
            }

            WaterImmersionCamera waterImmersionCamera = other.GetComponent<Player>().cameraReference.transform.GetChild(0).GetComponent<WaterImmersionCamera>();
            if(waterImmersionCamera)
                waterImmersionCamera.isImmerge = false;

            playerController.isGravityEnabled = true;
            playerController.IsUnderWater = false;
        }
    }

}
