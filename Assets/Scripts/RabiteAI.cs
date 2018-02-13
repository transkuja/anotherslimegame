using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabiteAI : MonoBehaviour {

    public enum RabiteState {Idle, Move, Attack, Detect, Dead };

    private Animator rabiteAnimator;
    private Rigidbody rb;

    // Public to debug
    public RabiteState currentState;
    public Vector3 playerToTarget;
    public Transform playerToLookAt;

    private float cooldown;
    private float timer;

    private Vector3 velocity;

    private int separationMask;
    private Collider[] playersCollided;
    private float sphereCheckRadius;

    private float sphereCheckRadiusDetect;

    public Rigidbody Rb
    {
        get
        {
            return rb;
        }

        set
        {
            rb = value;
        }
    }

    public RabiteState CurrentState
    {
        get
        {
            return currentState;
        }

        set
        {
            currentState = value;
        }
    }

    public void Start()
    {
        rabiteAnimator = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody>();
        CurrentState = RabiteState.Idle;

        cooldown = 0.0f;
        timer = 2.0f;

        sphereCheckRadius = 4.0f;
        sphereCheckRadiusDetect = 8.0f;
        separationMask = LayerMask.GetMask(new string[] { "Player"});
    }

    public void Update()
    {
        if( CurrentState == RabiteState.Attack)
        {
            playersCollided = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask);

            if (playersCollided.Length > 0)
            {
                playerToLookAt = playersCollided[0].transform;
            }
        }
        if (CurrentState == RabiteState.Idle || CurrentState == RabiteState.Move)
        {
            playersCollided = Physics.OverlapSphere(transform.position, sphereCheckRadiusDetect, separationMask);

            if (playersCollided.Length > 0)
            {
                playerToTarget = playersCollided[0].transform.position - transform.position;

                if (playersCollided[0].transform != transform && Vector3.Angle(playerToTarget, transform.forward) < 200) // Verification en cone
                {
                    CurrentState = RabiteState.Detect;
                    cooldown = 0.0f;
                    timer = 0.2f;
                }
            }
        }
        if (CurrentState == RabiteState.Idle || CurrentState == RabiteState.Move || CurrentState == RabiteState.Detect)
        {
            playersCollided = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask);
      
            if (playersCollided.Length > 0)
            {
                playerToTarget = playersCollided[0].transform.position - transform.position;
  
                if (playersCollided[0].transform != transform && Vector3.Angle(playerToTarget, transform.forward) < 120) // Verification en cone
                {
                    CurrentState = RabiteState.Attack;
                    cooldown = 0.0f;
                    timer = 0.2f;
                }
            }
        }


        cooldown += Time.deltaTime;
        if (cooldown > timer)
        {
            switch (CurrentState)
            {
                case RabiteState.Idle:
                    CurrentState = RabiteState.Move;
                    velocity = transform.forward * 2;
                    rabiteAnimator.SetBool("Ismoving", true);
                    cooldown = 0.0f;
                    timer = 5.0f;
                    break;
                case RabiteState.Move:
                    velocity = Vector3.zero;
                    rabiteAnimator.SetBool("Ismoving", false);
                    CurrentState = RabiteState.Idle;
                    cooldown = 0.0f;
                    timer = 2.0f;
                    break;
                case RabiteState.Detect:
                    CurrentState = RabiteState.Move;
                    velocity = playerToTarget.normalized * 3;
                    transform.rotation = Quaternion.LookRotation(playerToLookAt.transform.position - transform.position, Vector3.up);
                    //transform.LookAt(playerToLookAt);
                    rabiteAnimator.SetBool("Ismoving", true);
                    cooldown = 0.0f;
                    timer = 4.0f;
                    break;
                case RabiteState.Attack:
                    velocity = playerToTarget.normalized * 2;
                    transform.rotation = Quaternion.LookRotation(playerToLookAt.transform.position - transform.position, Vector3.up);
                    //transform.LookAt(playerToLookAt);
                    rabiteAnimator.SetBool("IsAttacking", true);
                    break;
                case RabiteState.Dead:
                    rabiteAnimator.SetBool("IsDying", true);
                    playerToTarget = Vector3.zero; // useless but
                    return;
            }
            cooldown = 0.0f;
            velocity += Vector3.down * 9.81f;
        }

        //if (cooldown > 1f)
        //{
            Rb.velocity = velocity;
            if(CurrentState == RabiteState.Attack && cooldown > 2.0f)
            {
                rabiteAnimator.SetBool("IsAttacking", false);
                CurrentState = RabiteState.Idle;
                cooldown = 0.0f;
                timer = 2.0f;
                playerToTarget = Vector3.zero;
            }
   
        //}

    }
}
