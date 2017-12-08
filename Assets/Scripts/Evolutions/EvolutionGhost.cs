using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class EvolutionGhost : EvolutionComponent
{
    float maxEmissionTime = 2.0f;

    float currentEmissionTimeLeft = 2.0f;

    float trailComponentLifeTime = 10.0f;

    float trailComponentSpawnIntervalTime = 0.1f;

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

    public float TrailComponentLifeTime
    {
        get
        {
            return trailComponentLifeTime;
        }

        set
        {
            trailComponentLifeTime = value;
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

    public override void Start()
    {
        base.Start();
        gameObject.layer = LayerMask.NameToLayer("GhostPlayer");
    }

    protected new void OnDestroy()
    {
        //base.OnDestroy();
        gameObject.layer = LayerMask.NameToLayer("Player");
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
            
            if(state.Buttons.B == ButtonState.Pressed)
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
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, 1.0f))
                    {
                        GameObject trailPane = Instantiate(ResourceUtils.Instance.refPrefabGhost.prefabGhostTrailPane);
                        trailPane.transform.position = hit.point + Vector3.up*0.01f;
                        float scale = Random.Range(0.8f, 1.2f);
                        trailPane.transform.localScale *= scale;
                        timeSinceLastTrailComponentSpawned = 0.0f;
                        if (hit.collider.GetComponent<PlatformGameplay>())
                            trailPane.transform.SetParent(hit.collider.transform);
                        trailPane.GetComponent<GhostTrail>().owner = GetComponent<PlayerController>();
                        trailPane.transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
                        Destroy(trailPane, trailComponentLifeTime);
                    }
                    
                }
            }
        }
    }
}
