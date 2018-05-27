using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerChildren { SlimeMesh, ShadowProjector, BubbleParticles, SplashParticles, WaterTrailParticles, CameraTarget, DustTrailParticles, DashParticles, LandingParticles, TeleportParticles, RuneObtained };
public enum BodyPart { Body, Rig, Wings, Hammer, Staff, Customization, GhostParticles, Size, None}


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

    GameObject runeObtained;
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

    public GameObject RuneObtained
    {
        get
        {
            if (runeObtained == null) runeObtained = transform.GetChild((int)PlayerChildren.RuneObtained).gameObject;
            return runeObtained;
        }
    }

    public ParticleSystem GhostParticles
    {
        get
        {
            if (!ghostParticles) ghostParticles = transform.GetChild((int)PlayerChildren.SlimeMesh).GetChild(4).GetComponent<ParticleSystem>();
            return ghostParticles;
        }
    }

    [Header("BodyParts")]
    [SerializeField]
    private Transform mainGauche;
    [SerializeField]
    private Transform mainDroite;
    [SerializeField]
    private Transform[] evolutionParts;
    [SerializeField]
    private Transform body;

    public Transform MainGauche
    {
        get
        {
            return mainGauche;
        }
    }
    public Transform MainDroite
    {
        get
        {
            return mainDroite;
        }
    }
    public Transform[] EvolutionParts
    {
        get
        {
            return evolutionParts;
        }
    }
    public Transform Body
    {
        get
        {
            return body;
        }
    }
}
