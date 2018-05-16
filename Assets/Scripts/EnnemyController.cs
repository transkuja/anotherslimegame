using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyController : MonoBehaviour {

    private PlayerCharacterHub playerCharacterHub;
    private Player player;
    private Rigidbody rb;

    [Header("Zone")]
    [SerializeField]
    bool useStartPositionAsZonePosition = true;
    [SerializeField]
    Vector3 zonePosition;

    [SerializeField]
    float zoneHalfExtent = 30.0f;

    [Header("Parameters")]
    [SerializeField]
    float playerDetectionRadius = 10.0f;
    [SerializeField]
    float pursuitMaxRange = 40.0f;

    [SerializeField]
    float attackRange = 5.0f;

    [SerializeField]
    float attackMaxRange = 9.0f;

    int separationMask;

    public GameObject feedbackenemy;

    GameObject currentTarget = null;
    bool originReach = true;

    bool isDead = false;

    public enum RabiteState
    {
        Wander,
        Pursuit,
        Attack,
        Dead
    }

    public RabiteState CurrentState;

    private void OnDrawGizmosSelected()
    {
        switch (CurrentState)
        {
            case RabiteState.Wander:
                if (currentWanderPosTarget != Vector3.zero)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(currentWanderPosTarget, 2.0f);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);
                }
                break;
            case RabiteState.Pursuit:
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(currentTarget.transform.position, attackRange);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, pursuitMaxRange);
                break;
            case RabiteState.Attack:
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(currentTarget.transform.position, attackMaxRange);
                break;
            case RabiteState.Dead:
                break;
        }
    }


    private void Start()
    {
        playerCharacterHub = GetComponent<PlayerCharacterHub>();
        player = GetComponent<Player>();
        rb = playerCharacterHub.Rb;

        if (useStartPositionAsZonePosition)
            zonePosition = transform.position;

        CurrentState = RabiteState.Wander;
        separationMask = LayerMask.GetMask(new string[] { "Player" });
    }

    public void Update()
    {
        if (GameManager.CurrentState != GameState.Normal || isDead)
        {
            return;
        }

        switch(CurrentState)
        {
            case RabiteState.Wander:
                Wander();
                break;
            case RabiteState.Pursuit:
                Pursuit();
                break;
            case RabiteState.Attack:
                Attack();
                break;
            case RabiteState.Dead:
                Die();
                break;
        }
    }

    enum WanderState
    {
        Idle,
        ChooseDestination,
        Moving
    }

    WanderState currentWanderState = WanderState.Idle;

    float wanderTimer = 0.0f;
    float nextActionTime = 0.0f;
    Vector3 currentWanderPosTarget;
    Vector3 playerToWanderTarget;
    Collider[] playersCollided;

    void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (originReach && wanderTimer > nextActionTime)
        {
            currentWanderState = (WanderState)Random.Range(0, 2);

            wanderTimer = 0.0f;
            nextActionTime = Random.Range(1.0f, 2.0f);
        }
        else if (!originReach && wanderTimer > 15)
        {
            // Tp just in case
            transform.position = zonePosition;
            originReach = true;
            wanderTimer = 0.0f;
            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);

        }
        if (currentWanderState == WanderState.ChooseDestination)
        {
            currentWanderPosTarget = GetRandomPointInZone();
            playerToWanderTarget = currentWanderPosTarget - transform.position;

            currentWanderState = WanderState.Moving;
            originReach = false;
            rb.drag = 0;
        }
        if(currentWanderState == WanderState.Moving)
        {
            transform.LookAt(currentWanderPosTarget);

            HandleMovement(0, 1);

            if (Vector3.Distance(currentWanderPosTarget, transform.position) < 2.0f)
            {
                originReach = true;
                currentWanderState = WanderState.Idle;
                rb.velocity = Vector3.zero;
                rb.drag = 2;
            }
        }

        playersCollided = Physics.OverlapSphere(transform.position, playerDetectionRadius, separationMask);

        if (playersCollided.Length > 0)
        {
            currentTarget = playersCollided[0].gameObject;

            // Test useless except npc are on the same layer as player
            if (currentTarget.GetComponent<PlayerControllerHub>())
            {
                if (playersCollided[0].transform != transform && Vector3.Angle(currentTarget.transform.position - transform.position, transform.forward) < 200) // Verification en cone
                {
                    CurrentState = RabiteState.Pursuit;
                }
            }
        }
    }

    Vector3 GetRandomPointInZone()
    {
        return zonePosition + new Vector3(Random.Range(0.0f, zoneHalfExtent), 0, Random.Range(0.0f, zoneHalfExtent));
    }

    void Pursuit()
    {
        transform.LookAt(currentTarget.transform.position);
        HandleMovement(0, 1);

        if (Vector3.Distance(currentTarget.transform.position, transform.position) < attackRange)
        {
            CurrentState = RabiteState.Attack;
        }
        else if(Vector3.Distance(currentTarget.transform.position, transform.position) > pursuitMaxRange)
        {
            CurrentState = RabiteState.Wander;
        }
    }

    void Attack()
    {
        transform.LookAt(currentTarget.transform.position);
        if (Vector3.Distance(currentTarget.transform.position, transform.position) > attackMaxRange)
        {
            playerCharacterHub.PlayerState = playerCharacterHub.dashState;
            currentWanderState = WanderState.Moving;
            CurrentState = RabiteState.Pursuit;
        }
    }

    void Die()
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.drag = 0.2f;
        isDead = true;
        Destroy(feedbackenemy);
        GetComponentInChildren<PlayerCosmetics>().FaceEmotion = FaceEmotion.Hit;
    }

    public void HandleMovement(float x, float y)
    {
        Vector3 initialVelocity = playerCharacterHub.PlayerState.HandleSpeed(x, y);
        Vector3 velocityVec = initialVelocity.z * transform.forward;
        if (!playerCharacterHub.IsGrounded)
            velocityVec += initialVelocity.x * transform.right * player.airControlFactor;
        else
            velocityVec += initialVelocity.x * transform.right;

        playerCharacterHub.PlayerState.Move(velocityVec, player.airControlFactor, x, y);

        // TMP Animation
        playerCharacterHub.PlayerState.HandleControllerAnim(initialVelocity.x, initialVelocity.y);
    }


    public void OnCollisionEnter(Collision collision)
    {
        if(isDead)
        {
            // Force end state
            GetComponentInChildren<Animator>().SetBool("isExpulsed", false);
            GetComponentInChildren<Animator>().Play("Idle");
            transform.GetChild(0).rotation = Quaternion.identity;
            playerCharacterHub.PlayerState = playerCharacterHub.freeState;
            ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
            this.gameObject.SetActive(false);
            DropCollectableOnGround();
            Invoke("Reactivate", 60);
        }
    }

    void DropCollectableOnGround()
    {
        int numberOfCollectablesToDrop = Random.Range(5, 15);
        for (int i = 0; i < numberOfCollectablesToDrop; i++)
        {
            if (GameManager.Instance.IsInHub())
            {
                GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Money).GetItem(null, transform.position + Vector3.up * 0.5f, Quaternion.identity, true);
                go.GetComponent<Collectable>().Disperse(i);
            }
        }
    }

    public void Reactivate()
    {
   CurrentState = RabiteState.Wander;
        isDead = false;
        GetComponentInChildren<Animator>();
        transform.position = zonePosition+ Vector3.up;
        transform.rotation = Quaternion.identity;
        ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.HitParticles).GetItem(null, transform.position + 3.0f * Vector3.up, Quaternion.identity, true, false, (int)HitParticles.BigHit);
        
        this.gameObject.SetActive(true);
    }
}
