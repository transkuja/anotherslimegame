﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerChildren { SlimeMesh, ShadowProjector, BubbleParticles, SplashParticles, WaterTrailParticles, CameraTarget, DustTrailParticles, DashParticles, LandingParticles, TeleportParticles };
public enum BodyPart { Body, Wings, Hammer, Staff , Customization, Size, None}


public class PlayerCharacter : MonoBehaviour {

    private int evolutionBodyParts = 3;

    [Header("Base")]
    private Rigidbody rb;
    private Animator anim;

    public Rigidbody Rb
    {
        get
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            return rb;
        }
    }
    public Animator Anim
    {
        get
        {
            if (!anim) anim = GetComponentInChildren<Animator>();
            return anim;
        }
    }

    // Particles
    [Header("Particle Systems")]
    private ParticleSystem dustTrailParticles;
    private ParticleSystem dashParticles;
    private ParticleSystem bubbleParticles;
    private ParticleSystem splashParticles;
    private ParticleSystem waterTrailParticles;
    private ParticleSystem landingParticles;
    private ParticleSystem ghostParticles;
    private ParticleSystem teleportParticles;

    public ParticleSystem DustTrailParticles
    {
        get
        {
            if (!dustTrailParticles) dustTrailParticles = transform.GetChild((int)PlayerChildren.DustTrailParticles).GetComponent<ParticleSystem>();
            return dustTrailParticles;
        }
    }
    public ParticleSystem DashParticles
    {

        get
        {
            if (!dashParticles) dashParticles = transform.GetChild((int)PlayerChildren.DashParticles).GetComponent<ParticleSystem>();
            return dashParticles;
        }
    }
    public ParticleSystem BubbleParticles
    {
        get
        {

            if (!bubbleParticles) bubbleParticles = transform.GetChild((int)PlayerChildren.BubbleParticles).GetComponent<ParticleSystem>();
            return bubbleParticles;
        }
    }
    public ParticleSystem SplashParticles
    {
        get
        {
            if (!splashParticles) splashParticles = transform.GetChild((int)PlayerChildren.SplashParticles).GetComponent<ParticleSystem>();
            return splashParticles;
        }
    }
    public ParticleSystem WaterTrailParticles
    {
        get
        {
            if (!waterTrailParticles) waterTrailParticles = transform.GetChild((int)PlayerChildren.WaterTrailParticles).GetComponent<ParticleSystem>();
            return waterTrailParticles;
        }

    }
    public ParticleSystem LandingParticles
    {
        get
        {
            if (landingParticles == null) landingParticles = transform.GetChild((int)PlayerChildren.LandingParticles).GetComponent<ParticleSystem>();
            return landingParticles;
        }
    }

    public ParticleSystem TeleportParticles
    {
        get
        {
            if (teleportParticles == null) teleportParticles = transform.GetChild((int)PlayerChildren.TeleportParticles).GetComponent<ParticleSystem>();
            return teleportParticles;
        }
    }

    public ParticleSystem GhostParticles
    {
        get
        {
            if (!ghostParticles) ghostParticles = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild((int)BodyPart.Body).GetChild(2).GetComponent<ParticleSystem>();
            return ghostParticles;
        }
    }

    [Header("BodyParts")]
    private Transform mainGauche;
    private Transform mainDroite;
    private Transform oreilleGauche;
    private Transform oreilleDroite;
    private Transform[] evolutionParts;
    private Transform body;

    public Transform MainGauche
    {
        get
        {
            if(!mainGauche) mainGauche = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(0).GetChild(0).GetChild(0);
            return mainGauche;
        }
    }
    public Transform MainDroite
    {
        get
        {
            if(!mainDroite) mainDroite = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(0).GetChild(0).GetChild(1);
            return mainDroite;
        }
    }
    public Transform[] EvolutionParts
    {
        get
        {
            if (evolutionParts == null)
            {
                evolutionParts = new Transform[evolutionBodyParts];
                for (int i = 0; i < evolutionBodyParts; i++)// le probleme c'est corps : i+1 -> evolution.BodyPart
                    evolutionParts[i] = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(i + 1);
            }
            return evolutionParts;
        }
    }
    public Transform Body
    {
        get
        {
            if (!body) body = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild((int)BodyPart.Body);
            return body;
        }
    }
    public Transform OreilleGauche
    {
        get
        {
            if (!oreilleGauche) oreilleGauche = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild((int)BodyPart.Body).GetChild(1).GetChild(0);
            return oreilleGauche;
        }
    }
    public Transform OreilleDroite
    {
        get
        {
            if (!oreilleDroite) oreilleDroite = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild((int)BodyPart.Body).GetChild(1).GetChild(1);
            return oreilleDroite;
        }
    }



}
