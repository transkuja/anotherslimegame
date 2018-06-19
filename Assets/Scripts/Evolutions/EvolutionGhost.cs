using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class EvolutionGhost : EvolutionComponent
{
    Material baseMat;
    Material baseMatEars;
    Material ghostMat;
    Material baseDustTrailMat;
    Material ghostDustTrailMat;

    float maxEmissionTime = 2.0f;

    float currentEmissionTimeLeft = 2.0f;

    float trailComponentSpawnIntervalTime = 0.1f;

    Transform parent = null;

    float emissionTimeRegenRate = 0.5f;
    float timeSinceLastTrailComponentSpawned = 0.0f;

    float minEmissionTimeThreshold = 1.0f;

    bool hitZero = false;

    bool isButtonPressedAndDidNotHitZero = false;

    float timeBeforeLastButtonPress = 0.0f;

    float timeBeforeRegenStartAfterButtonPush = 0.5f;

    float timeMultiplicatorBeforeRegenStarts = 2.0f;

    public float MaxEmissionTime
    {
        get
        {
            return maxEmissionTime;
        }

        set
        {
            maxEmissionTime = value;
        }
    }

    public float CurrentEmissionTimeLeft
    {
        get
        {
            return currentEmissionTimeLeft;
        }

        set
        {
            currentEmissionTimeLeft = value;
        }
    }

    public float TrailComponentSpawnIntervalTime
    {
        get
        {
            return trailComponentSpawnIntervalTime;
        }

        set
        {
            trailComponentSpawnIntervalTime = value;
        }
    }

    public float EmissionTimeRegen
    {
        get
        {
            return emissionTimeRegenRate;
        }

        set
        {
            emissionTimeRegenRate = value;
        }
    }

    public bool HitZero
    {
        get
        {
            return hitZero;
        }

        set
        {
            hitZero = value;
        }
    }

    public void SetGhostVisual()
    {
        baseMat = playerCharacter.Body.GetComponentInChildren<Renderer>().sharedMaterial;
        ghostMat = ResourceUtils.Instance.refPrefabGhost.GhostMaterial;
        baseDustTrailMat = playerCharacter.DustTrailParticles.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        ghostDustTrailMat = ResourceUtils.Instance.refPrefabGhost.GhostDustTrailMaterial;


        playerCharacter.Body.GetComponentInChildren<Renderer>().material = ghostMat;
        if( playerCharacter.GetComponentInChildren<PlayerCosmetics>().Ears != "None")
        {
            foreach( Renderer r in playerCharacter.GetComponentInChildren<CustomizableSockets>().GetSocket(CustomizableType.Ears).GetComponentsInChildren<Renderer>())
                r.material = ghostMat;
        }

        if (playerCharacter.GhostParticles)
            playerCharacter.GhostParticles.Play();
        playerCharacter.DustTrailParticles.GetComponent<ParticleSystemRenderer>().material = ghostDustTrailMat;
        playerCharacter.DustTrailParticles.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = ghostDustTrailMat;


    }

    public void RemoveGhostVisual()
    {
        playerCharacter.Body.GetComponentInChildren<Renderer>().material = baseMat;
        if (playerCharacter.GhostParticles)
            playerCharacter.GhostParticles.Stop();

        if (playerCharacter.GetComponentInChildren<PlayerCosmetics>().Ears != "None")
        {
            playerCharacter.GetComponentInChildren<PlayerCosmetics>().Ears = playerCharacter.GetComponentInChildren<PlayerCosmetics>().Ears;
        }


        playerCharacter.DustTrailParticles.GetComponent<ParticleSystemRenderer>().material = baseDustTrailMat;
        playerCharacter.DustTrailParticles.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = baseDustTrailMat;
    }

    public override void Start()
    {
        base.Start();
        SetPower(Powers.Ghost);
        gameObject.layer = LayerMask.NameToLayer("GhostPlayer");
        if (player.evolutionTutoShown != null && player.evolutionTutoShown.Length > 0 && !player.evolutionTutoShown[(int)Powers.Ghost] && !GameManager.Instance.CurrentGameMode.IsMiniGame())
        {
            player.evolutionTutoShown[(int)Powers.Ghost] = true;
            if ((int)GetComponent<PlayerController>().playerIndex == Controls.keyboardIndex)
                Utils.PopTutoText("Hold", KeyboardControlType.Stomp, "to leave a trail behind", player);
            else

                Utils.PopTutoText("Hold", ControlType.RightTrigger, "to leave a trail behind", player);
        }

        if (feedbackCooldownImg != null)
            feedbackCooldownImg.sprite = ResourceUtils.Instance.spriteUtils.Ghost;
    }

    protected new void OnDestroy()
    {
        //base.OnDestroy();
        player.activeEvolutions--;
        gameObject.layer = LayerMask.NameToLayer("Player");
        RemoveGhostVisual();

        playerCharacter.PlayerState = playerCharacter.freeState;
    }

    public override void Update()
    {
        base.Update();
        timeSinceLastTrailComponentSpawned += Time.deltaTime;
        if(!isButtonPressedAndDidNotHitZero)
        {
            if(timeBeforeLastButtonPress > timeBeforeRegenStartAfterButtonPush)
            {
                currentEmissionTimeLeft = Mathf.Clamp(currentEmissionTimeLeft + Time.deltaTime * emissionTimeRegenRate, 0f, maxEmissionTime);
            }
            else
            {
                timeBeforeLastButtonPress += Time.deltaTime;
            }

        }
            
        if(hitZero)
        {
            //Wait longer before starting to regnerate after you hit zero
            if (timeBeforeLastButtonPress > timeBeforeRegenStartAfterButtonPush*timeMultiplicatorBeforeRegenStarts)
            {
                // Regenerate twice as fast when starting from zero until reactivation threshold
                currentEmissionTimeLeft = Mathf.Clamp(currentEmissionTimeLeft + Time.deltaTime * emissionTimeRegenRate * 2.0f, 0f, maxEmissionTime);
            }
            else
            {
                timeBeforeLastButtonPress += Time.deltaTime;
            }
        }
    }

    public void HandleTrail(GamePadState state)
    {
        isButtonPressedAndDidNotHitZero = false;
        if (hitZero)
        {
            if (currentEmissionTimeLeft > minEmissionTimeThreshold)
                hitZero = false;
        }
        if (!hitZero)
        {
            
            if(Controls.LeaveATrail(state, (int)GetComponent<PlayerController>().playerIndex))
            {
                timeBeforeLastButtonPress = 0.0f;
                isButtonPressedAndDidNotHitZero = true;
                currentEmissionTimeLeft -= Time.deltaTime;
                if (currentEmissionTimeLeft <= 0.0f)
                {
                    hitZero = true;
                    currentEmissionTimeLeft = 0.0f;
                }

                if (timeSinceLastTrailComponentSpawned >= TrailComponentSpawnIntervalTime)
                {
                    
                    Ray ray = new Ray(transform.position+Vector3.up*.5f, Vector3.down);
                    RaycastHit hit = new RaycastHit();
                    if(Physics.Raycast(ray, out hit, 1.0f, ~(1 << LayerMask.NameToLayer("GhostTrail"))))
                    {
                        GameObject trailPane = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.GhostTrail).GetItem(parent, hit.point + Vector3.up * 0.01f, Quaternion.identity, true, true);

                        float scale = Random.Range(0.8f, 1.2f);
                        trailPane.transform.localScale *= scale;
                        timeSinceLastTrailComponentSpawned = 0.0f;
                        if (hit.collider.GetComponent<PlatformGameplay>())
                            trailPane.transform.SetParent(hit.collider.transform);
                        trailPane.GetComponent<GhostTrail>().owner = GetComponent<PlayerCharacterHub>();
                        trailPane.transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
                    }
                    
                }
            }
        }
    }
}
