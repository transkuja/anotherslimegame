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

    [SerializeField]
    float forwardSpeed = 20000.0f;
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
    CheckPoint LastCheckpoint;

    KartPlayerState currentState = KartPlayerState.Normal;
    GamePadState state;
    GamePadState previousState;
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
            }
        }
    }

    void Start () {
        dashTimer = dashCooldown;
        player = GetComponent<Player>();
        Rb = GetComponent<Rigidbody>();
        targetForward = transform.forward;
        state = GamePad.GetState(PlayerIndex);
        previousState = state;
    }

	void Update () {

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

        previousState = state;
	}

    void ApplyGravity()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 20000.0f);
    }

    void HandleNormalState()
    {
        ApplyGravity();
        if (GameManager.CurrentState != GameState.Normal)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, -transform.up, out hit))
        {
            if (hit.collider.GetComponent<DeathZone>())
            {
                rb.AddForce(-transform.up * 300.0f + rb.velocity * Time.deltaTime);
                return;
            }
        }
        if (dashTimer < dashCooldown)
            dashTimer += Time.deltaTime;

        // -1 if we're reversing, 1 if going forward
        float directionFactor = Vector3.Dot(rb.velocity, transform.forward) < -0.2f ? -1.0f : 1.0f;

        // Clamp Rotation speed relative to maxVelocity
        float velocityRatio = directionFactor * ((rb.velocity.magnitude > maxVelocityMagnitude) ? 1.0f : (rb.velocity.magnitude / maxVelocityMagnitude));

        transform.Rotate(Vector3.up * velocityRatio * state.ThumbSticks.Left.X * Time.deltaTime * turnSpeed);

        //rb.AddForce(transform.right * rb.velocity.magnitude * state.ThumbSticks.Left.X);

        rb.AddForce(transform.forward * Time.deltaTime * forwardSpeed * state.Triggers.Right);

        rb.AddForce(-transform.forward * Time.deltaTime * forwardSpeed / 2.0f * state.Triggers.Left);

        //rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocityMagnitude);

        if (dashTimer >= dashCooldown && state.Buttons.X == ButtonState.Pressed && previousState.Buttons.X == ButtonState.Released)
        {
            rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
            dashTimer = 0.0f;
        }
        targetForward = new Vector3(state.ThumbSticks.Left.X, 0, state.ThumbSticks.Left.Y);
        if (targetForward == Vector3.zero)
            targetForward = transform.forward;
        //transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, targetForward, Time.deltaTime * 6.0f * Mathf.Clamp(rb.velocity.magnitude/2.0f, 0.0f, 1.0f)), Vector3.up);
        //transform.rotation = Quaternion.LookRotation(Vector3.Lerp(transform.forward, rb.velocity.normalized, Time.deltaTime * 10.0f));


        //float directionFactor = Mathf.Round((state.Triggers.Right) + (state.Triggers.Left * -1));

    }

    void HandleHitState()
    {
        ApplyGravity();
        hitTimer += Time.deltaTime;
        if(hitTimer >= hitRecoveryTime)
        {
            hitTimer = 0.0f;
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
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.contacts[0].normal * 50.0f, ForceMode.Impulse);
            //Rb.AddForce(collision.contacts[0].normal * collision.impulse.magnitude * 2.0f, ForceMode.Impulse);
        }
    }

}
