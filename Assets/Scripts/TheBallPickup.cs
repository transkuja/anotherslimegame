using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBallPickup : TheBall {

    int health = 3;

    int Health
    {
        get
        {
            return health;
        }

        set
        {
            health = value;
            if (health <= 0)
            {
                GameManager.EvolutionManager.AddEvolutionComponent(
                    GameManager.Instance.PlayerStart.PlayersReference[lastHitPlayer],
                    GameManager.EvolutionManager.GetEvolutionByPowerName(type),
                    true,
                    7
                );
                ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles)
                    .GetItem(null, transform.position, Quaternion.identity, true, false, (int)HitParticles.BigHit);
                Destroy(gameObject);
            }
        }
    }

    Powers type;

    public virtual void OnEnable()
    {
        lastHitPlayer = -1;

        GetComponentInChildren<MeshRenderer>().material.EnableKeyword("_EMISSION");
        if (Random.Range(0, 2) == 0)
        {
            type = Powers.Agile;
            GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.cyan);
        }
        else
        {
            type = Powers.Strength;
            GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", new Color(1.0f, 0.35f, 0.0f));
        }
    }

    int lastHitPlayer;

    public override void OnKick(PlayerCharacterHub _pch)
    {
        if (_pch.GetComponent<EnnemyController>())
            return;

        lastHitPlayer = (int)_pch.GetComponent<PlayerController>().PlayerIndex;
        int playerLeading = 0;
        int playerLosing = 0;
        int maxScore = -1;
        int minScore = 10000;
        for (int i = 0; i < GameManager.Instance.CurrentGameMode.curNbPlayers; i++)
        {
            if (maxScore < GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints)
            {
                maxScore = GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints;
                playerLeading = i;
            }

            if (minScore > GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints)
            {
                minScore = GameManager.Instance.PlayerStart.PlayersReference[i].GetComponent<Player>().NbPoints;
                playerLosing = i;
            }
        }

        if (GameManager.Instance.DataContainer != null && GameManager.Instance.DataContainer.launchedFromMinigameScreen)
        {
            if ((int)_pch.GetComponent<PlayerController>().PlayerIndex == playerLosing)
            {
                Health -= 2;
            }
            else
            {
                if ((int)_pch.GetComponent<PlayerController>().PlayerIndex == playerLeading)
                {
                    if (Random.Range(0, 3) == 0)
                        Health--;
                }
                else
                    Health--;
            }
        }
        else
        {
            Health--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<BoardFloor>())
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }
}
