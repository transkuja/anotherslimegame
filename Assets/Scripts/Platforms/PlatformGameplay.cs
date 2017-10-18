using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WIP Antho
public class PlatformGameplay : MonoBehaviour {

    [Tooltip("Will make the player bounce on the platform")]
    public bool isBouncy = false;
    [Tooltip("Will make items bounce on the platform")]
    public bool isBouncyForItems = false;

    [Tooltip("Defines if the platform will move or not")]
    public bool isMoving = false;
    [Tooltip("The axis on which the platform moves (the vector will be normalized)")]
    public Vector3 movingAxis = Vector3.zero;
    [Tooltip("The platform velocity")]
    public float movingSpeed = 0.0f;
    [Tooltip("The distance the platform should travel")]
    public float movingDistance = 0.0f;
    [Tooltip("The platform will alternate between destination and original position")]
    public bool isAPingPongMovement = false;
    [Tooltip("The platform will only starts the movement when the player jumps on it")]
    public bool movementWhenPlayerJumpsOn = false;
    [Tooltip("The platform will return to its original position when the player jumps off")]
    public bool returnToPositionWhenPlayerExits = false;
    [Tooltip("The delay before the movement starts")]
    public float delayBeforeMovement = 0.0f;

    [Tooltip("Teleports a player to a distant place")]
    public bool isATeleporter;
    [Tooltip("The transform target for the teleportation")]
    public Transform teleporterTarget;

    [Tooltip("Will make the player slide on the platform")]
    public bool isSlippery;


    // Private variables
    Transform originPosition;
    Vector3 lerpOriginPosition;
    Vector3 lerpNewPosition;
    float moveLerpValue = 0.0f;
    float delayTimer;
    bool isInPong = false;
    bool hasPlayerJumpedOn = false;
    bool hasPlayerJumpedOff = false;
    bool hasPlatformReachedDestination = false;

    void Start () {
		if (isBouncy)
        {
            // isbouncy start process
        }
        if (isBouncyForItems)
        {
            // isBouncyForItems start process
        }
        if (isSlippery)
        {
            // isSlippery start process
        }
        if (isATeleporter)
        {
            // isATeleporter start process
        }
        if (isMoving)
        {
            if (movingAxis == Vector3.zero)
            {
                Debug.LogWarning("isMoving is set to true but no moving axis are defined!");
            }
            else
                movingAxis.Normalize();
        }
        originPosition = transform;
        lerpOriginPosition = transform.position;
        lerpNewPosition = transform.position + movingDistance * movingAxis;
        delayTimer = delayBeforeMovement;
    }

    void Update () {
        HandlePlatformMove();
    }

    void MovingProcess()
    {
        if (delayTimer < 0.0f)
        {
            transform.position = Vector3.Lerp(lerpOriginPosition, lerpNewPosition, moveLerpValue);
            moveLerpValue += Time.deltaTime * movingSpeed * ((!isInPong) ? 1 : -1);
            if (Vector3.Distance(transform.position, lerpNewPosition) < 0.1f)
            {
                hasPlatformReachedDestination = true;
                if (isAPingPongMovement)
                    isInPong = !isInPong;
                if (delayBeforeMovement > 0.0f)
                    delayTimer = delayBeforeMovement;
                moveLerpValue = (isInPong) ? 1.0f : 0.0f;
            }
            else
                hasPlatformReachedDestination = false;

        }
        else
        {
            delayTimer -= Time.deltaTime;
        }
    }

    void HandlePlatformMove()
    {
        if (returnToPositionWhenPlayerExits)
        {
            if (hasPlayerJumpedOff && hasPlatformReachedDestination)
            {
                isInPong = true;
                MovingProcess();
            }
        }

        if (movementWhenPlayerJumpsOn)
        {
            if (hasPlayerJumpedOn)
                MovingProcess();
        }
        else
            MovingProcess();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if collision avec player au dessus
    }

    private void OnCollisionExit(Collision collision)
    {
        // if collision avec player au dessus

    }
}
