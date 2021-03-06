﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrail : MonoBehaviour {

    public PlayerCharacterHub owner;
    [Tooltip("Must correspond to the time set in the pool manager")]
    public float lifeTime;
    MeshRenderer mr;
    ParticleSystem ps;
    float finalAlpha = 0.65f;
    float fadeTime = 0.5f;
    bool fadeIn = true;
    bool fadeOut = false;
    float fadeInTimer = 0.0f;
    float fadeOutTimer = 0.0f;
    float lifeTimer = 0.0f;
    public Color color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    private void OnEnable()
    {
        lifeTimer = 0.0f;
        lifeTime = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.GhostTrail).timerReturnToPool;
        fadeOutTimer = 0.0f;
        mr = GetComponent<MeshRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        mr.material.color = color;
        fadeIn = true;
        fadeOut = false;
        ps.Play();
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;
        if(lifeTimer >= lifeTime - ps.main.startLifetime.constant - 0.1f)
        {
            ps.Stop();
        }
        if(lifeTimer >= lifeTime-fadeTime-Time.deltaTime-0.1f && !fadeOut)
        {
            fadeOut = true;
        }
        if(fadeIn)
        {
            fadeInTimer += Time.deltaTime;
            Color col = mr.material.color;
            col.a = Mathf.Lerp(0.0f, finalAlpha, fadeInTimer / fadeTime);
            mr.material.color = col;
            if(fadeInTimer >= fadeTime)
            {
                fadeInTimer = 0.0f;
                col.a = finalAlpha;
                mr.material.color = col;
                fadeIn = false;
            }
        }
        else if(fadeOut)
        {
            fadeOutTimer += Time.deltaTime;
            Color col = mr.material.color;
            col.a = Mathf.Lerp(finalAlpha, 0.0f, fadeOutTimer / fadeTime);
            mr.material.color = col;
            if (fadeOutTimer >= fadeTime)
            {
                col.a = 0.0f;
                mr.material.color = col;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacterHub playerCharacter = other.GetComponent<PlayerCharacterHub>();
        if(playerCharacter && playerCharacter != owner)
        {
            if(playerCharacter.PlayerState == playerCharacter.restrainedByGhostState)
            {
                ((RestrainedByGhostState)(playerCharacter.PlayerState)).ResetTimer();
            }
            else
            {
                playerCharacter.PlayerState = playerCharacter.restrainedByGhostState;
            }
        } 
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerCharacterHub playerCharacter = other.GetComponent<PlayerCharacterHub>();
        if (playerCharacter && playerCharacter != owner)
        {
            if (playerCharacter.PlayerState == playerCharacter.restrainedByGhostState)
            {
                ((RestrainedByGhostState)(playerCharacter.PlayerState)).ResetTimer();
            }
            else
            {
                playerCharacter.PlayerState = playerCharacter.restrainedByGhostState;
            }
        }
    }
}
