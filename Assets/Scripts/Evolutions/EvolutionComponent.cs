using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI
    ;


public class EvolutionComponent : MonoBehaviour {
    Evolution evolution;
    float timer;
    protected PlayerControllerHub playerController;
    protected PlayerCharacterHub playerCharacter;
    protected Player player;
    protected bool isSpecialActionPushedOnce = false;
    protected bool isSpecialActionPushed = false;
    protected bool isSpecialActionReleased = false;

    GameObject affectedPart;

    public virtual void Start()
    {
        timer = -1;
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
        } else
        {
            // Le probleme ici c'est le corps prend le spot 0
            affectedPart = playerCharacter.EvolutionParts[(int)evolution.BodyPart - 1].gameObject;

            if (evolution.BodyPart == BodyPart.Hammer)
                affectedPart.transform.SetParent(playerCharacter.MainGauche);
            else if (evolution.BodyPart == BodyPart.Staff)
                affectedPart.transform.SetParent(playerCharacter.MainDroite);
        }
        affectedPart.SetActive(true);
    }

    public virtual void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            if (timer < 0.0f)
            {
                Destroy(this);
            }
        }
    }

    protected virtual void OnDestroy()
    {
        player.activeEvolutions--;
        affectedPart.transform.SetParent(transform.GetChild((int)PlayerChildren.SlimeMesh));
        affectedPart.transform.SetSiblingIndex((int)evolution.BodyPart);
        affectedPart.SetActive(false);
    }

    public virtual void OnCollisionEnter(Collision coll)
    {

    }
    public virtual void OnCollisionStay(Collision coll)
    {

    }
}
