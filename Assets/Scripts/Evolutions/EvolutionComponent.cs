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
        if (GetComponentsInChildren<EvolutionComponent>().Length > 1)
            Destroy(GetComponentsInChildren<EvolutionComponent>()[0]);

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

        if (evolution.BodyPart == BodyPart.None && powerName == Powers.Ghost)
        {
            //Change player appearence
            ((EvolutionGhost)(this)).SetGhostVisual();
            //Then do Nothing
            affectedPart = transform.gameObject;
        }
        else
            affectedPart = playerCharacter.EvolutionParts[(int)evolution.BodyPart - 2].gameObject;

        affectedPart.SetActive(true);

        if (GameManager.Instance.IsInHub() || GameManager.Instance.CurrentGameMode is Runner3DGameMode)
        {
            if (feedbackCooldownImg == null)
            {
                // Ça c'est cadeau
                feedbackCooldownImg = ResourceUtils.Instance.feedbackCooldown.transform.GetChild(SlimeDataContainer.instance.nbPlayers - 1).GetChild((int)GetComponent<PlayerControllerHub>().playerIndex).GetComponentInChildren<UnityEngine.UI.Image>();               
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
                Destroy(this);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        player.activeEvolutions--;
        affectedPart.SetActive(false);
    }

    public virtual void OnCollisionEnter(Collision coll)
    {

    }
    public virtual void OnCollisionStay(Collision coll)
    {

    }
}
