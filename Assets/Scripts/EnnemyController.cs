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

    GameObject currentTarget = null;
    //private Animator rabiteAnimator;

    bool isDead = false;

    private void Start()
    {
        playerCharacterHub = GetComponent<PlayerCharacterHub>();
        player = GetComponent<Player>();

        if (useStartPositionAsZonePosition)
            zonePosition = transform.position;

        CurrentState = RabiteState.Wander;
        //rabiteAnimator = GetComponent<Animator>();
        rb = playerCharacterHub.Rb;
        separationMask = LayerMask.GetMask(new string[] { "Player" });
    }

    public void Update()
    {
        //ApplyGravity();

        if (GameManager.CurrentState != GameState.Normal || isDead)
        {
            //rabiteAnimator.StartPlayback();
            return;
        }

        //rabiteAnimator.StopPlayback();
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
        if (wanderTimer > nextActionTime)
        {
            //rabiteAnimator.SetBool("Ismoving", false);
            currentWanderState = (WanderState)Random.Range(0, 2);

            wanderTimer = 0.0f;
            nextActionTime = Random.Range(1.0f, 2.0f);
        }
        if(currentWanderState == WanderState.ChooseDestination)
        {
            currentWanderPosTarget = GetRandomPointInZone();
            playerToWanderTarget = currentWanderPosTarget - transform.position;
            //rabiteAnimator.SetBool("Ismoving", true);
            currentWanderState = WanderState.Moving;
        }
        if(currentWanderState == WanderState.Moving)
        {

            Vector3 playerToWanderTarget2 = new Vector3(playerToWanderTarget.x, 0, playerToWanderTarget.z);
            transform.LookAt(playerToWanderTarget2);

            HandleMovement(0, 1);

            if (Vector3.Distance(playerToWanderTarget, transform.position) < 2.0f)
            {
                //rabiteAnimator.SetBool("Ismoving", false);
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
                //rabiteAnimator.SetBool("Ismoving", true);
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

        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transformToTarget2, Vector3.up), Time.deltaTime * 8.0f);
        HandleMovement(0, 1);
        //MoveTowards(transformToTarget);

        if (Vector3.Distance(currentTarget.transform.position, transform.position) < attackRange)
        {
            //rabiteAnimator.SetBool("Ismoving", false);
            CurrentState = RabiteState.Attack;
        }
        else if(Vector3.Distance(currentTarget.transform.position, transform.position) > pursuitMaxRange)
        {
            CurrentState = RabiteState.Wander;
        }
    }

    void Attack()
    {
        //Vector3 transformToTarget2 = new Vector3(transformToTarget.x, 0, transformToTarget.z);
        transform.LookAt(currentTarget.transform.position);
        ////rabiteAnimator.SetBool("IsAttacking", true);
        //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform.position, Vector3.up), Time.deltaTime * 8.0f);


        if (Vector3.Distance(currentTarget.transform.position, transform.position) > attackMaxRange)
        {
            //rabiteAnimator.SetBool("IsAttacking", false);
            //rabiteAnimator.SetBool("Ismoving", true);
            playerCharacterHub.PlayerState = playerCharacterHub.dashState;
            CurrentState = RabiteState.Pursuit;
        }
    }

    void Die()
    {
        //rabiteAnimator.SetBool("IsDying", true);
        rb.constraints = RigidbodyConstraints.None;
        rb.drag = 0.2f;
        isDead = true;
    }

    public virtual void HandleMovement(float x, float y)
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

}
