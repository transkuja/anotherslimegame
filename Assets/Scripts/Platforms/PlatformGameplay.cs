using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WIP Antho
public class PlatformGameplay : MonoBehaviour {
    [Header("Debug")]
    [Tooltip("Draws GREEN gizmos to show how platform starts, CYAN gizmos for rotation center")]
    public bool drawGizmos = false;
    [Tooltip("Draws only the gizmo lines")]
    public bool drawLinesOnly = false;
    [Tooltip("Draws only the gizmo shapes")]
    public bool drawShapesOnly = false;

    [Tooltip("Reset the platform to origin position")]
    public bool resetPlatform = false;

    [Header("Physics")]
    [Tooltip("Will make the player bounce on the platform")]
    public bool isBouncy = false;
    [Tooltip("Will make items bounce on the platform")]
    public bool isBouncyForItems = false;
    [Tooltip("Will make the player slide on the platform")]
    public bool isSlippery;

    [Header("Movement")]
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
    public bool backToOriginOnPlayerExit = false;
    [Tooltip("The platform will return to its original position when the player jumps on it")]
    public bool backToOriginOnPlayerJumpOn = false;
    [Tooltip("The delay before the movement starts")]
    public float delayBeforeMovement = 0.0f;

    /*
     * Rotations
     * - Local x,y,z
     * - Based on an axis and a position
     * Combinaisons
     * - Multiple locals
     * - Local + axis/position
     * - Local + movement
     * - /!\ pas de axis/position si deja du mvt
     */
    [System.Serializable]
    public class Rotation
    {
        [Tooltip("Does the platform rotate around a local axis?")]
        public bool rotateAroundLocalAxis = false;
        [Tooltip("The platform will rotate around this axis")]
        public Vector3 rotateAxis = Vector3.zero;
        [Tooltip("The platform rotate velocity")]
        public float rotateSpeed = 10.0f;
    }

    [Header("Rotation")]
    [Tooltip("Defines if the platform will rotate or not")]
    public bool isRotating = false;
    [Tooltip("Moves the center of rotation")]
    public Vector3 newRotationCenter = Vector3.zero;
    public Rotation baseRotation;
    [Tooltip("Dual rotation is used to rotate a platform around a point, relative to the platform position, while rotating the platform itself.")]
    public bool isDualRotationEnabled = false;
    public Rotation secondRotation;

    [Header("Teleportation")]
    [Tooltip("Teleports a player to a distant place")]
    public bool isATeleporter;
    [Tooltip("The transform target for the teleportation")]
    public Transform teleporterTarget;


    // Private variables
    Transform originPosition;
    Vector3 lerpOriginPosition;
    Vector3 lerpNewPosition;
    float moveLerpValue = 0.0f;
    float delayTimer;
    bool isInPong = false;
    bool hasPlayerJumpedOn = false;
    bool hasPlayerJumpedOff = false;
    bool hasJumpAgainWhenDestReached = false;
    bool hasPlatformReachedDestination = false;
    bool isOnPlatform = false;

    Vector3 platformOriginPosition;
    Quaternion platformOriginRotation;
    Vector3 platformOriginScale;


    // Gizmos check variables
    bool drawLineOnlyPrevState;
    bool drawShapesOnlyPrevState;

    void Start() {
        platformOriginPosition = transform.position;
        platformOriginRotation = transform.rotation;
        platformOriginScale = transform.localScale;

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
            if (teleporterTarget == null)
                Debug.LogWarning("The platform is flagged as a teleporter but no target has been defined!");
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

        if (isRotating)
        {
            if (baseRotation.rotateAxis == Vector3.zero)
            {
                Debug.LogWarning("isRotating is set to true but no rotating axis are defined!");
            }
            else
                baseRotation.rotateAxis.Normalize();
        }
        originPosition = transform;
        lerpOriginPosition = transform.position;
        lerpNewPosition = transform.position + movingDistance * movingAxis;
        delayTimer = delayBeforeMovement;
        if (GetComponent<Ground>() == null && tag != "Ground")
        {
            Debug.LogWarning("There's nothing indicating that the platform is part of the ground. Adding Ground component.");
            gameObject.AddComponent<Ground>();
        }
    }

    void Update() {
        if (resetPlatform)
            ResetPlatformToOrigin();

        HandlePlatformMove();
        HandleTeleportation();
        HandlePlatformRotation();
    }

    void MovingProcess()
    {
        // Not a ping pong movement should only do the ping, except if a return is planned
        if (!isAPingPongMovement && isInPong
            && !backToOriginOnPlayerExit && !backToOriginOnPlayerJumpOn)
            return;


        // Handle specific pongs
        if (isInPong)
        {
            // On player exit first, 
            //      so that the behavior is still correct if both OnPlayerExit and OnPlayerJumpOn are checked
            if (backToOriginOnPlayerExit)
            {
                if (!hasPlayerJumpedOff)
                    return;
            }
            else if (backToOriginOnPlayerJumpOn)
            {
                if (!hasJumpAgainWhenDestReached)
                    return;
            }
        }

        // Movement process
        if (delayTimer < 0.0f)
        {
            transform.position = Vector3.Lerp(lerpOriginPosition, lerpNewPosition, moveLerpValue);
            moveLerpValue += Time.deltaTime * movingSpeed * ((!isInPong) ? 1 : -1);
            if ((Vector3.Distance(transform.position, lerpNewPosition) < 0.1f && !isInPong)
                || (Vector3.Distance(transform.position, lerpOriginPosition) < 0.1f && isInPong))
            {
                hasPlatformReachedDestination = true;
                hasJumpAgainWhenDestReached = false;
                isInPong = !isInPong;
                if (delayBeforeMovement > 0.0f)
                    delayTimer = delayBeforeMovement;
                moveLerpValue = (isInPong) ? 1.0f : 0.0f;

                // If no pong behaviour is specified, do not reset hasPlayerJumpedOn to ping pong indefinitely after activation
                if (!isInPong && (backToOriginOnPlayerExit || backToOriginOnPlayerJumpOn))
                    hasPlayerJumpedOn = false;
            }
            else
                hasPlatformReachedDestination = false;

        }
        else
        {
            delayTimer -= Time.deltaTime;
        }
    }

    void TeleportProcess()
    {
        if (isOnPlatform)
        {
            if (delayTimer < 0.0f)
            {
                Player player = GetComponentInChildren<Player>();
                if (player != null)
                {
                    player.transform.SetParent(null);
                    player.transform.position = teleporterTarget.position;
                    player.transform.rotation = teleporterTarget.rotation;
                    isOnPlatform = false;
                }
                if (delayBeforeMovement > 0.0f)
                    delayTimer = delayBeforeMovement;
            }
            else
            {
                delayTimer -= Time.deltaTime;
            }
        }
        else
            delayTimer = delayBeforeMovement;
    }

    void RotationProcess()
    {
        if (isRotating)
        {
            if (baseRotation.rotateAroundLocalAxis)
                transform.Rotate(baseRotation.rotateAxis, Time.deltaTime * baseRotation.rotateSpeed);
            else
            {
                transform.RotateAround(platformOriginPosition + newRotationCenter, baseRotation.rotateAxis, Time.deltaTime * baseRotation.rotateSpeed);
            }
            
            if (isDualRotationEnabled)
            {
                if (secondRotation.rotateAroundLocalAxis)
                    transform.Rotate(secondRotation.rotateAxis, Time.deltaTime * secondRotation.rotateSpeed);
                else
                    transform.RotateAround(transform.position, secondRotation.rotateAxis, Time.deltaTime * secondRotation.rotateSpeed);
            }
        }
    }

    void ResetPlatformToOrigin()
    {
        transform.position = platformOriginPosition;
        transform.rotation = platformOriginRotation;
        resetPlatform = false;
    }

    void HandlePlatformMove()
    {
        if (isMoving)
        {

            if (movementWhenPlayerJumpsOn)
            {
                if (hasPlayerJumpedOn)
                    MovingProcess();
            }
            else
                MovingProcess();
        }
    }

    void HandlePlatformRotation()
    {
        RotationProcess();
    }

    void HandleTeleportation()
    {
        if (isATeleporter && teleporterTarget != null)
            TeleportProcess();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with player from above
        if (collision.gameObject.GetComponent<Player>() != null
            && Vector3.Dot(collision.contacts[0].normal, Vector3.up) < -0.95f)
        {
            if (hasPlayerJumpedOff && hasPlatformReachedDestination) hasJumpAgainWhenDestReached = true;

            hasPlayerJumpedOn = true;
            hasPlayerJumpedOff = false;
            collision.transform.SetParent(transform);

            isOnPlatform = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null && hasPlayerJumpedOn)
        {
            hasPlayerJumpedOff = true;
            collision.transform.SetParent(null);
            isOnPlatform = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            GizmosCheck();

            // Platform origin
            Gizmos.color = Color.green;
            if (!drawLinesOnly)
            {
                MeshFilter meshFilter = GetComponentInParent<MeshFilter>();
                if (meshFilter == null) meshFilter = GetComponentInChildren<MeshFilter>();
                if (meshFilter != null)
                {
                    // /!\ platformOriginRotation returns an error when we stop play mode
                    Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh, platformOriginPosition, platformOriginRotation, platformOriginScale);
                }
                else
                    Gizmos.DrawCube(platformOriginPosition, Vector3.one);
            }

            if (!drawShapesOnly)
                Gizmos.DrawRay(transform.position, platformOriginPosition - transform.position);


            // Platform center of rotation
            if (isRotating)
            {
                Gizmos.color = Color.cyan;
                if (!drawShapesOnly)
                    Gizmos.DrawRay(transform.position, platformOriginPosition + newRotationCenter - transform.position);
                if (!drawLinesOnly)
                    Gizmos.DrawSphere(platformOriginPosition + newRotationCenter, 1.0f);
            }
        }
    }

    void GizmosCheck()
    {
        if (drawLinesOnly && !drawLineOnlyPrevState)
        {
            if (drawShapesOnly)
            {
                drawShapesOnlyPrevState = false;
                drawShapesOnly = false;
            }
            drawLineOnlyPrevState = drawLinesOnly;
        }
        if (drawShapesOnly && !drawShapesOnlyPrevState)
        {
            if (drawLinesOnly)
            {
                drawLineOnlyPrevState = false;
                drawLinesOnly = false;
            }
            drawLineOnlyPrevState = drawLinesOnly;
        }
    }
}
