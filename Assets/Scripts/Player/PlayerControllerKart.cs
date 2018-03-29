using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWPAndXInput;

public class PlayerControllerKart : PlayerController {

    public enum KartPlayerState
    {
        Normal,
        Hit,
        FinishedRace
    }

    Animator anim;
    AudioSource sound;
    [SerializeField]
    float forwardSpeed = 425.0f;
    [SerializeField]
    float maxVelocityMagnitude = 20.0f;
    [SerializeField]
    float turnSpeed = 200.0f;
    [SerializeField]
    float dashForce = 100.0f;
    [SerializeField]
    float dashCooldown = 1.0f;
    [SerializeField]
    float hitRecoveryTime = 0.75f;
    [SerializeField]
    float startDrag;
    [SerializeField]
    CheckPoint LastCheckpoint;

    KartPlayerState currentState = KartPlayerState.Normal;

    Vector3 targetForward;
    float dashTimer = 0.0f;
    float hitTimer = 0.0f;

    public KartPlayerState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;
            if(currentState == KartPlayerState.Hit)
            {
                hitTimer = 0.0f;
                anim.Play("Hit", 0);
                anim.speed = 1.25f;
            }
            else
            {
                anim.Play("Idle", 0);
                anim.speed = 1f;
            }
        }
    }

    public float HitRecoveryTime
    {
        get
        {
            return hitRecoveryTime;
        }
        set
        {
            hitRecoveryTime = value;
        }
    }

    void Start () {
        sound = GetComponent<AudioSource>();
        dashTimer = dashCooldown;
        player = GetComponent<Player>();
        Rb = GetComponent<Rigidbody>();
        targetForward = transform.forward;
        anim = GetComponent<Animator>();
        startDrag = rb.drag;
    }

	public override void Update () {
        base.Update();
        sound.pitch = rb.velocity.magnitude / maxVelocityMagnitude + 1.8f;
        state = GamePad.GetState(PlayerIndex);
        switch(CurrentState)
        {
            case KartPlayerState.Normal:
                HandleNormalState();
                break;
            case KartPlayerState.Hit:
                HandleHitState();
                break;
            case KartPlayerState.FinishedRace:
                HandleFinishedRaceState();
                break;
        }
	}

    void ApplyGravity(float gravity = 500.0f)
    {
        rb.AddForce(Vector3.down * Time.deltaTime * gravity, ForceMode.VelocityChange);
    }

    //Force should be handled in fixed update?
    void HandleNormalState()
    {
        ApplyGravity();
        if (GameManager.CurrentState != GameState.Normal)
            return;

        if (dashTimer < dashCooldown)
            dashTimer += Time.deltaTime;

        // -1 if we're reversing, 1 if going forward
        float directionFactor = Vector3.Dot(rb.velocity, transform.forward) < -0.2f ? -1.0f : 1.0f;
        
        // Clamp Rotation speed relative to maxVelocity
        float velocityRatio = directionFactor * ((rb.velocity.magnitude > maxVelocityMagnitude) ? 1.0f : (rb.velocity.magnitude / maxVelocityMagnitude));

        transform.Rotate(Vector3.up * velocityRatio * state.ThumbSticks.Left.X * Time.deltaTime * turnSpeed);

        rb.AddForce(transform.forward * Time.deltaTime * forwardSpeed * state.Triggers.Right, ForceMode.VelocityChange);

        rb.AddForce(-transform.forward * Time.deltaTime * forwardSpeed / 2.0f * state.Triggers.Left, ForceMode.VelocityChange);

        if (dashTimer >= dashCooldown && state.Buttons.X == ButtonState.Pressed && prevState.Buttons.X == ButtonState.Released)
        {
            rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            dashTimer = 0.0f;
        }
        targetForward = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);
        if (targetForward == Vector3.zero)
            targetForward = transform.forward;
    }

    void HandleHitState()
    {
        ApplyGravity(125f);
        rb.drag = 2f;
        hitTimer += Time.deltaTime;
        if(hitTimer >= hitRecoveryTime)
        {
            hitTimer = 0.0f;
            rb.drag = startDrag;
            CurrentState = KartPlayerState.Normal;
        }
    }

    void HandleFinishedRaceState()
    {
        ApplyGravity();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerControllerKart>())
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 50.0f, ForceMode.Impulse);
    }

}
