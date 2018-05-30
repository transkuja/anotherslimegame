using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
    ;


public class EvolutionComponent : MonoBehaviour {
    Evolution evolution;
    public float timer = -1;
    protected PlayerControllerHub playerController;
    protected PlayerCharacterHub playerCharacter;
    protected Player player;
    protected bool isSpecialActionPushedOnce = false;
    protected bool isSpecialActionPushed = false;
    protected bool isSpecialActionReleased = false;

    GameObject affectedPart;
    bool timerHasBeenSet = false;

    protected Image feedbackCooldownImg;
    float startTimer;

    public virtual void Start()
    {
        player = GetComponent<Player>();
        playerController = GetComponent<PlayerControllerHub>();
        playerCharacter = GetComponent<PlayerCharacterHub>();
    }

    public float Timer
    {
        get
        {
            return timer;
        }

        set
        {
            timer = value;
            startTimer = value;
        }
    }

    public Evolution Evolution
    {
        get
        {
            return evolution;
        }
    }

    protected void SetPower(Powers powerName)
    {
        player.activeEvolutions++;

        // recuperation de l'évolution a partir du nom passé en paramètre
        evolution = GameManager.EvolutionManager.GetEvolutionByPowerName(powerName);

        // Si il y a une ancienne evolution qui n'est pas la meme on l'enleve
        if (GetComponentsInChildren<EvolutionComponent>().Length > 1)
        {
            if (GetComponentsInChildren<EvolutionComponent>()[0].evolution.Id != evolution.Id)
                Destroy(GetComponentsInChildren<EvolutionComponent>()[0]);
        }

        if (evolution.BodyPart == BodyPart.None)
        {
            if (powerName == Powers.Ghost)
            {
                //Change player appearence
                ((EvolutionGhost)(this)).SetGhostVisual();
                //Then do Nothing
                affectedPart = transform.gameObject;
                affectedPart.SetActive(true);
            }
            else if (powerName == Powers.Agile)
            {
                ParticleSystem.MainModule mainModule = GetComponent<PlayerCharacterHub>().DashParticles.GetComponent<ParticleSystem>().main;
                mainModule.loop = true;
                transform.GetChild((int)BodyPart.Body).GetChild(1).localScale = new Vector3(0.9f, 0.9f, 0.9f);
                GetComponent<PlayerCharacterHub>().DashParticles.Play();
            }
            else if (powerName == Powers.Strength)
            {
                transform.GetChild((int)BodyPart.Body).GetChild(1).localScale = new Vector3(1.3f, 1.3f, 1.3f);
                if (GetComponent<PlayerCharacterHub>().StrengthParticles && GetComponent<PlayerCharacterHub>().StrengthParticles.GetComponent<StrengthParticlesHandler>())
                    GetComponent<PlayerCharacterHub>().StrengthParticles.GetComponent<StrengthParticlesHandler>().enabled = true;
            }
        }
        else
        {
            affectedPart = playerCharacter.EvolutionParts[(int)evolution.BodyPart - 2].gameObject;
            affectedPart.SetActive(true);

        }


        if (GameManager.Instance.IsInHub() || GameManager.Instance.CurrentGameMode is Runner3DGameMode)
        {
            if (feedbackCooldownImg == null)
            {
                // Ça c'est cadeau
                feedbackCooldownImg = ResourceUtils.Instance.feedbackCooldown.transform.GetChild(SlimeDataContainer.instance.nbPlayers - 1).GetChild((int)GetComponent<PlayerControllerHub>().playerIndex).GetChild(1).GetComponent<Image>();               
            }
        }
    }

    public virtual void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (feedbackCooldownImg != null && !(GameManager.Instance.CurrentGameMode is Runner3DGameMode))
            {
                feedbackCooldownImg.fillAmount = timer / startTimer;
            }
            if (timer < 0.0f)
            {
                if (feedbackCooldownImg != null && !(GameManager.Instance.CurrentGameMode is Runner3DGameMode))
                {
                    feedbackCooldownImg.fillAmount = 0.0f;
                }
                Destroy(this);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        player.activeEvolutions--;
        if (affectedPart != null)
            affectedPart.SetActive(false);

        // Plarformist and wall jump clean up ! important
        playerCharacter.PlayerState = playerCharacter.freeState;
    }

    public virtual void OnCollisionEnter(Collision coll)
    {

    }
    public virtual void OnCollisionStay(Collision coll)
    {

    }
}
