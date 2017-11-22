using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class EvolutionGhost : EvolutionComponent
{
    float maxEmissionTime = 3.0f;
    float currentEmissionTimeLeft = 3.0f;

    float trailComponentLifeTime = 10.0f;

    float trailComponentSpawnIntervalTime = 0.1f;

    float emissionTimeRegenRate = 0.1f;
    float timeSinceLastTrailComponentSpawned = 0.0f;

    float minEmissionTimeThreshold = 1.0f;

    bool isOnGround = false;

    bool hitZero = false;

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

    public override void Start()
    {
        base.Start();
        gameObject.layer = LayerMask.NameToLayer("GhostPlayer");
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    public override void Update()
    {
        base.Update();

        currentEmissionTimeLeft = Mathf.Clamp(currentEmissionTimeLeft + Time.deltaTime * emissionTimeRegenRate, 0f, maxEmissionTime);
        timeSinceLastTrailComponentSpawned += Time.deltaTime;
    }

    public void HandleTrail(GamePadState state)
    {
        if (hitZero && currentEmissionTimeLeft > minEmissionTimeThreshold)
            hitZero = false;
        if (!hitZero)
        {
            currentEmissionTimeLeft -= Time.deltaTime;
            if (currentEmissionTimeLeft <= 0.0f)
            {
                hitZero = true;
                currentEmissionTimeLeft = 0.0f;
            }
                
            if(timeSinceLastTrailComponentSpawned >= TrailComponentSpawnIntervalTime)
            {
                GameObject trailPane = Instantiate(ResourceUtils.Instance.refPrefabGhost.prefabGhostTrailPane);
                trailPane.transform.position = transform.position;
                float scale = Random.Range(0.8f, 1.2f);
                trailPane.transform.localScale *= scale;
                timeSinceLastTrailComponentSpawned = 0.0f;
                trailPane.GetComponent<GhostTrail>().owner = GetComponent<Player>();
                Destroy(trailPane, trailComponentLifeTime);
            }
            
        }
    }

    public override void OnCollisionEnter(Collision col)
    {
        if(col.collider.GetComponent<Ground>())
        {
            isOnGround = true;
        }
    }

    public override void OnCollisionStay(Collision col)
    {
        if (col.collider.GetComponent<Ground>())
        {
            isOnGround = true;
        }
    }

    public void OnCollisionExit(Collision col)
    {
        if (col.collider.GetComponent<Ground>())
        {
            isOnGround = false;
        }
    }
}
