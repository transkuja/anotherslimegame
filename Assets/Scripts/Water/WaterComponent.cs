using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterComponent : MonoBehaviour {

    public float waterResistance;

    public GameObject WaterToActivateAtRuntime;

    public void Start()
    {
        WaterToActivateAtRuntime.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Rigidbody>() && other.GetComponent<PlayerCharacterHub>())
        {
            PlayerCharacterHub playerCharacter = other.GetComponent<PlayerCharacterHub>();

            if (playerCharacter.BubbleParticles && playerCharacter.SplashParticles && playerCharacter.WaterTrailParticles && playerCharacter.DustTrailParticles)
            {
                playerCharacter.BubbleParticles.Play();
                playerCharacter.SplashParticles.Play();
                playerCharacter.WaterTrailParticles.Play();
                playerCharacter.DustTrailParticles.Stop();
            }

            if (playerCharacter)
            {
                playerCharacter.underwaterState.waterLevel = transform.position.y;
                playerCharacter.PlayerState = playerCharacter.underwaterState;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Rigidbody>() && other.GetComponent<PlayerCharacterHub>())
        {
            PlayerCharacterHub playerCharacter = other.GetComponent<PlayerCharacterHub>();
            if (playerCharacter.BubbleParticles && playerCharacter.WaterTrailParticles && playerCharacter.DustTrailParticles)
            {
                playerCharacter.BubbleParticles.Stop();
                playerCharacter.WaterTrailParticles.Stop();
                playerCharacter.DustTrailParticles.Play();
            }
        }

    }
}
