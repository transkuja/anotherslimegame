using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRabite : MonoBehaviour {

    [Header("Zone")]
    [SerializeField]
    bool useStartPositionAsZonePosition = true;
    [SerializeField]
    Vector3 zonePosition;

    [SerializeField]
    float zoneHalfExtent = 30.0f;

    [Header("Colliders")]
    [SerializeField]
    Collider AliveCollider;

    [SerializeField]
    Collider DeadCollider;

    [Header("Parameters")]
    [SerializeField]
    float moveSpeed = 2500.0f;
    [SerializeField]
    float playerDetectionRadius = 10.0f;
    [SerializeField]
    float pursuitMaxRange = 40.0f;

    [SerializeField]
    float attackRange = 2.0f;

    [SerializeField]
    float attackMaxRange = 3.0f;

    int separationMask;

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
                if (currentWanderPosTarget != null)
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

    GameObject currentTarget = null;
    private Animator rabiteAnimator;
    private Rigidbody rb;

    bool isDead = false;

    private void Start()
    {
        if (useStartPositionAsZonePosition)
            zonePosition = transform.position;

        CurrentState = RabiteState.Wander;
        rabiteAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        separationMask = LayerMask.GetMask(new string[] { "Player" });
    }

    private void Update()
    {
        ApplyGravity();

        if (GameManager.CurrentState != GameState.Normal)
            return;

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
                if (!isDead)
                    Die();
                break;
        }
    }

    void MoveTowards(Vector3 direction)
    {
        rb.AddForce(direction.normalized * moveSpeed * Time.deltaTime);
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
        if (wanderTimer > nextActionTime)
        {
            rabiteAnimator.SetBool("Ismoving", false);
            currentWanderState = (WanderState)Random.Range(0, 2);

            wanderTimer = 0.0f;
            nextActionTime = Random.Range(1.0f, 3.0f);
        }
        if(currentWanderState == WanderState.ChooseDestination)
        {
            currentWanderPosTarget = GetRandomPointInZone();
            playerToWanderTarget = currentWanderPosTarget - transform.position;
            rabiteAnimator.SetBool("Ismoving", true);
            currentWanderState = WanderState.Moving;
        }
        if(currentWanderState == WanderState.Moving)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(playerToWanderTarget, Vector3.up), Time.deltaTime*8.0f);
            MoveTowards(playerToWanderTarget);
            if(Vector3.Distance(currentWanderPosTarget, transform.position) < 2.0f)
            {
                rabiteAnimator.SetBool("Ismoving", false);
                currentWanderState = WanderState.Idle;
            }
        }

        playersCollided = Physics.OverlapSphere(transform.position, playerDetectionRadius, separationMask);

        if (playersCollided.Length > 0)
        {
            currentTarget = playersCollided[0].gameObject;

            if (playersCollided[0].transform != transform && Vector3.Angle(currentTarget.transform.position - transform.position, transform.forward) < 200) // Verification en cone
            {
                CurrentState = RabiteState.Pursuit;
                rabiteAnimator.SetBool("Ismoving", true);
            }
        }

    }

    void ApplyGravity()
    {
        if(CurrentState == RabiteState.Dead)
        {
            rb.AddForce(Vector3.down * 9.81f * 50.0f * Time.deltaTime);
        }
        else
            rb.AddForce(Vector3.down * 9.81f * 500.0f * Time.deltaTime);
    }

    Vector3 GetRandomPointInZone()
    {
        return zonePosition + new Vector3(Random.Range(0.0f, zoneHalfExtent), 0, Random.Range(0.0f, zoneHalfExtent));
    }

    void Pursuit()
    {
        Vector3 transformToTarget = currentTarget.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transformToTarget, Vector3.up), Time.deltaTime * 8.0f);

        MoveTowards(transformToTarget);

        if (Vector3.Distance(currentTarget.transform.position, transform.position) < attackRange)
        {
            if (GameManager.Instance.DataContainer.GetComponent<SlimeDataContainer>().rabbitSelected[GameManager.Instance.PlayerStart.PlayersReference.IndexOf(currentTarget)])
                return;
            rabiteAnimator.SetBool("Ismoving", false);
            CurrentState = RabiteState.Attack;
        }
        else if(Vector3.Distance(currentTarget.transform.position, transform.position) > pursuitMaxRange)
        {
            CurrentState = RabiteState.Wander;
        }
    }

    void Attack()
    {
        rabiteAnimator.SetBool("IsAttacking", true);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up), Time.deltaTime * 8.0f);
        if (Vector3.Distance(currentTarget.transform.position, transform.position) > attackMaxRange)
        {
            rabiteAnimator.SetBool("IsAttacking", false);
            rabiteAnimator.SetBool("Ismoving", true);
            CurrentState = RabiteState.Pursuit;
        }
    }

    void Die()
    {
        rabiteAnimator.SetBool("IsDying", true);
        rb.constraints = RigidbodyConstraints.None;
        DeadCollider.enabled = true;
        AliveCollider.enabled = false;
        DeadCollider.material.bounciness = 0.75f;
        rb.drag = 0.2f;
        isDead = true;
    }
}
