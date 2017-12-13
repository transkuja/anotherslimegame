using UnityEngine;
using System; // For math
using System.Collections;
using System.Collections.Generic; // For List

using Random = UnityEngine.Random;

public class PlayerCollisionCenter : MonoBehaviour {

    [SerializeField]
    bool drawGizmos = true;
    [SerializeField]
    GameObject hitParticles;
    // PlayerBouncyPhysics
    [SerializeField]
    [Range(10.0f, 2000.0f)]
    float bounceStrength = 25.0f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float bounceDetectionThreshold = 0.2f;
    [SerializeField]
    [Range(0.0f, 2.5f)]
    float impactForce;
    [SerializeField]
    [Range(0.0f, 40.0f)]
    float impactPropagationThreshold;

    // Collision with strength on one player
    [SerializeField] float repulsionFactor = 2000;

    // @<remi
    private List<Player> impactedPlayers = new List<Player>();
    private List<Vector3> impactedPlayersOldVelocities = new List<Vector3>();
    private Vector3 velocityOnImpact;

    public float invicibilityFrame = 1.0f;
    bool onceRepulsion;
    int separationMask;
    Collider[] playersCollided;
    float sphereCheckRadius;
 
    int separationMask2;
    Collider[] collectablesCollided;
    float sphereCheckRadiusCollectables;

    int separationMask3;
    Collider[] breakablesCollided;

    PlayerController playerController;
    Rigidbody rb;
    Player player;

    // Edge climbing twerk values
    [Header("Edge climbing twerk values")]
    public float forwardAngleDetection = 80;
    public float maxHeightDetection = 0.8f;
    public float recalageForward = 0.0f;
    public float recalageUp = 0.25f;
    public float heightAngleForActivation = 25;

    bool hasCollidedWithAPlayer = false;
    public float timerStopOnDashCollision = 0.3f;
    float currentTimerStop = 0.0f;

    WaterComponent waterComponentEntered;
    float waterLevel;
    float waterTolerance;
    public float waterUpliftStrength;
    [Range(0.01f, 1.0f)]
    public float modulateWaterForceFactor;
    PlayerController _PlayerController
    {
        get
        {
            if (playerController == null)
                playerController = GetComponent<PlayerController>();
            return playerController;
        }
    }

    Rigidbody _Rb
    {
        get
        {
            if (rb == null)
                rb = GetComponent<Rigidbody>();
            return rb;
        }
    }

    public Player PlayerComponent
    {
        get
        {
            if (player == null)
                player = GetComponent<Player>();
            return player;
        }
    }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        repulsionFactor = 35;

        separationMask = LayerMask.GetMask(new string[] { "Player" });
        sphereCheckRadius = 5.0f;

        separationMask2 = LayerMask.GetMask(new string[] { "Collectable" });
        sphereCheckRadiusCollectables = 3.0f;

        separationMask3 = LayerMask.GetMask(new string[] { "Breakable" });
    }

    private void Update()
    {
        if (playerController.PlayerState is DashState)
        {
            playersCollided = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask);
            if (playersCollided != null)
            {
                for (int i = 0; i < playersCollided.Length; i++)
                {
                    Vector3 playerToTarget = playersCollided[i].transform.position - transform.position;
                    Vector3 playerCenterToTargetCenter = (playersCollided[i].transform.position + Vector3.up * 0.5f) - (transform.position + Vector3.up * 0.5f);
                    if (playersCollided[i].transform != transform && Vector3.Angle(playerToTarget, transform.forward) < 45) // Verification en cone
                    {
                        if (!impactedPlayers.Contains(playersCollided[i].GetComponent<Player>()))
                        {
                            Physics.IgnoreCollision(playersCollided[i].GetComponent<Collider>(), GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedPlayers.Add(playersCollided[i].GetComponent<Player>());
                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position+Vector3.up*0.5f + playerCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerController.PlayerState = playerController.frozenState;
                            playersCollided[i].GetComponent<PlayerController>().PlayerState = playersCollided[i].GetComponent<PlayerController>().frozenState;
                            velocityOnImpact = playerController.Rb.velocity;
                            playerController.Rb.velocity = Vector3.zero;
                            impactedPlayersOldVelocities.Add(playersCollided[i].GetComponent<Rigidbody>().velocity);
                            playersCollided[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //Set vibrations
                            UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.8f, 0.8f, .1f);
                            UWPAndXInput.GamePad.VibrateForSeconds(playersCollided[i].GetComponent<PlayerController>().playerIndex, 0.8f, 0.8f, .1f);

                            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                        }
                    }
                }
            }
        }

        collectablesCollided = Physics.OverlapSphere(transform.position, sphereCheckRadiusCollectables, separationMask2);
        if (collectablesCollided != null)
        {
            for (int i = 0; i < collectablesCollided.Length; i++)
            {
                //Vector3 collectableToTarget = collectablesCollided[i].transform.position - transform.position;

                if (collectablesCollided[i].transform != transform) // Verification en cone
                {
                    Collectable c = collectablesCollided[i].GetComponent<Collectable>();
                    if (!c.IsAttracted && !c.haveToDisperse)
                    {
                        Physics.IgnoreCollision(collectablesCollided[i].GetComponent<Collider>(), GetComponent<Collider>(), true);
                        if(c.GetComponent<Animator>())
                            c.GetComponent<Animator>().enabled = false;
                        c.PickUp(GetComponent<Player>());
                    }
                }
            }
        }

        breakablesCollided = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask3);
        if (breakablesCollided.Length > 0)
        {
            //hasCollidedWithAPlayer = true;
            //currentTimerStop = timerStopOnDashCollision;

            //playerController.PlayerState = playerController.frozenState;
            //velocityOnImpact = playerController.Rb.velocity;
            //playerController.Rb.velocity = Vector3.zero;

            for (int i = 0; i < breakablesCollided.Length; i++)
            {
                breakablesCollided[i].GetComponent<Breakable>().HandleCollision(playerController);
            }
        }

        if (hasCollidedWithAPlayer)
        {
            currentTimerStop -= Time.deltaTime;
            if (currentTimerStop <= 0.0f)
            {
                playerController.PlayerState = playerController.freeState;
                for (int i = 0; i < impactedPlayers.Count; i++)
                {
                    impactedPlayers[i].GetComponent<PlayerController>().PlayerState = impactedPlayers[i].GetComponent<PlayerController>().freeState;
                    ImpactHandling(impactedPlayers[i]);
                }
                hasCollidedWithAPlayer = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Interaction Joueur / Joueur
        if (collision.gameObject.GetComponent<Player>())
        {
            PlayerController collidedPlayerController = collision.transform.gameObject.GetComponent<PlayerController>();

            if (collidedPlayerController == null) return;

            if (!(_PlayerController.PlayerState is DashState)
                && !(collidedPlayerController.PlayerState is DashState))
            {
                // Default interaction no one is dashing of using an abilty
                // Could be reduce to thisPlayerController.BrainState == BrainState.Occupied && collidedPlayerController.BrainState == BrainState.Occupied
                // Can't confirm implications
                DefaultCollision(collision, collision.transform.gameObject.GetComponent<Player>());

                if (AudioManager.Instance != null && AudioManager.Instance.wahhFx != null)
                    if (!AudioManager.Instance.sourceFX.isPlaying)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.wahhFx);
            }
        }

        if (collision.gameObject.tag == "HardBreakable")
        {
            // marche pas pour dash down ( on est déjà sortis de DashDownState quand on touche le sol)(j'ai mis une cheville dans DashDownState en attendant
            if ((playerController.PlayerState is DashState || playerController.PlayerState is DashDownState) && playerController.GetComponent<EvolutionStrength>())
            {
                if (collision.gameObject.GetComponent<Rigidbody>() != null)
                {
                    // TMP impredictable
                    collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    collision.gameObject.transform.parent = null;
                    RepulseRigibody(collision.contacts[0].point, collision.gameObject.GetComponent<Rigidbody>(), repulsionFactor);
                    //Destroy(collision.gameObject, 4);
                }
            }
        }

        if (collision.gameObject.tag == "Breakable")
        {
            if ((playerController.PlayerState is DashState || playerController.PlayerState is DashDownState) && playerController.GetComponent<EvolutionStrength>())
            {
                if (collision.gameObject.GetComponent<Rigidbody>() != null)
                {
                    // TMP impredictable
                    collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    collision.gameObject.transform.parent = null;
                    RepulseRigibody(collision.contacts[0].point, collision.gameObject.GetComponent<Rigidbody>(), repulsionFactor);
                    //Destroy(collision.gameObject, 4);
                }
            }
        }

        if (PlayerComponent.isEdgeAssistActive)
        {
            if (collision.gameObject.GetComponent<PlatformGameplay>())
            {
                float heightAngleDetection = Vector3.Angle(-collision.contacts[0].normal, Vector3.up);

                if (Vector3.Angle(-collision.contacts[0].normal, transform.forward) < forwardAngleDetection 
                    && collision.transform.position.y - transform.position.y > maxHeightDetection
                    && heightAngleDetection < heightAngleForActivation)
                {
                    transform.position = collision.transform.GetComponent<Collider>().ClosestPoint(transform.position) + recalageForward * transform.forward + recalageUp * transform.up;
                }
            }
        }
    }

    public void ImpactHandling(Player playerImpacted)
    {

        // Damage Behavior
        DamagePlayer(playerImpacted);
        //Physics.IgnoreCollision()
        // ExpluseForce

        //if (_PlayerController.StrengthState == SkillState.Dashing) repulsionMultiplier *= -2;
        
        ExpulsePlayer(playerImpacted.GetComponent<Collider>().ClosestPoint(transform.position), playerImpacted.Rb, repulsionFactor);
        //RepulseRigibody(playersCollided[i].ClosestPoint(transform.position), playersCollided[i].GetComponent<Rigidbody>(), repulsionFactor);

    }

    public void DefaultCollision(Collision collision, Player playerImpacted)
    {
        //PlayerBouncyPhysics
        if ((transform.position.y - collision.transform.position.y) > bounceDetectionThreshold)
        {
            _Rb.velocity += Vector3.up * bounceStrength;
            _PlayerController.canDoubleJump = true;
        }
        else
        {
            if (_Rb.velocity.magnitude > playerImpacted.Rb.velocity.magnitude)
            {
                if (_Rb.velocity.magnitude > impactPropagationThreshold)
                    playerImpacted.Rb.velocity += (_Rb.velocity * impactForce);
            }
            else
            {
                if (playerImpacted.Rb.velocity.magnitude > impactPropagationThreshold)
                    _Rb.velocity += (playerImpacted.Rb.velocity * impactForce);
            }
        }
    }

    public void DamagePlayer(Player player)
    {
        // Damage Behavior
        int typeCollectable = (int)CollectableType.Points;
        //switch (GameManager.CurrentGameMode.gameModeType)
        //{
        //    case GameModeType.Escape:
        //        typeCollectable = (int)CollectableType.Points; break;
        //    case GameModeType.Arena:
        //        typeCollectable = (int)CollectableType.Points; break;
        //    default:
        //        break;
        //}

        //if (typeCollectable == -1) return;

        if (player.Collectables[(int)CollectableType.Key] > 0)
        {
            int random = Random.Range(1, Utils.GetMaxValueForCollectable(CollectableType.Key)+1);
            if(random > Utils.GetMaxValueForCollectable(CollectableType.Key) - player.Collectables[(int)CollectableType.Key])
                typeCollectable = (int)CollectableType.Key;
        } 


        if (player.Collectables[typeCollectable] > 0)
        {
            int numberOfCollectablesToDrop;
            if (typeCollectable == (int)CollectableType.Key) numberOfCollectablesToDrop = 1;
            else numberOfCollectablesToDrop = (int)Mathf.Clamp(((float)(player.Collectables[typeCollectable]) / Utils.GetDefaultCollectableValue(typeCollectable)), 1, 6);

  
            Vector3[] positions = SpawnManager.GetVector3ArrayOnADividedCircle(transform.position, player.GetComponent<SphereCollider>().bounds.extents.magnitude, numberOfCollectablesToDrop, SpawnManager.Axis.XZ);
            for (int i = 0; i < numberOfCollectablesToDrop; i++)
            {
                player.UpdateCollectableValue((CollectableType)typeCollectable, -Utils.GetDefaultCollectableValue(typeCollectable));

                GameObject go;
                // TMP !!! DOUX pool thing + alternate key for collision check
                if (typeCollectable == (int)CollectableType.Key)
                {
                    go = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(transform.position + Vector3.up * 2f, transform.rotation, null, CollectableType.Key);
                    go.GetComponent<Collectable>().Init();
                    go.GetComponent<Collectable>().hasBeenSpawned = true;
                    go.GetComponent<Collectable>().lastOwner = player;
                }
                else
                    go = ResourceUtils.Instance.poolManager.collectablePointsPool.GetItem(null, positions[i] + Vector3.up * 0.5f, player.transform.rotation, true);

                go.GetComponent<Collectable>().Disperse(i, (positions[i] - transform.position + Vector3.up*1.5f).normalized);
            }
        }
    }

    public void ExpulsePlayer(Vector3 collisionPoint, Rigidbody rbPlayerToExpulse, float repulsionFactor)
    {
        Vector3 direction = rbPlayerToExpulse.position - collisionPoint;
        direction.y = 0;

        direction.Normalize();
        ForcedJump(direction, repulsionFactor, rbPlayerToExpulse);
        StartCoroutine(ReactivateCollider(rbPlayerToExpulse.GetComponent<Player>()));
    }

    public void RepulseRigibody(Vector3 collisionPoint, Rigidbody rbPlayerToExpulse, float repulsionFactor)
    {
        if (!onceRepulsion && rbPlayerToExpulse.GetComponent<PlayerController>())
        {
            onceRepulsion = true;

            Vector3 direction = rbPlayerToExpulse.position - collisionPoint;
            direction.y = 0;

            direction.Normalize();

            rbPlayerToExpulse.AddForce(direction * repulsionFactor, ForceMode.Impulse);
            StartCoroutine(ReactivateCollider());
        }
    }

    public IEnumerator ReactivateCollider(Player p)
    {
        yield return new WaitForSeconds(invicibilityFrame);
        impactedPlayers.Remove(p);
        Physics.IgnoreCollision(p.GetComponent<Collider>(), GetComponent<Collider>(),false);
        yield return null;
    }
    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(invicibilityFrame);
        onceRepulsion = false;
        yield return null;
    }

    public void ForcedJump(Vector3 direction, float repulseStrength, Rigidbody target)
    {
        if (target.GetComponent<PlayerController>() != null)
        {
            PlayerController _pcTarget = target.GetComponent<PlayerController>();
            _pcTarget.jumpState.nbJumpMade = 0;
            _pcTarget.PlayerState.OnJumpPressed();
            _pcTarget.PlayerState.PushPlayer(direction* repulseStrength);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 5.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<WaterComponent>())
        {
            waterComponentEntered = other.GetComponent<WaterComponent>();
            waterLevel = other.transform.position.y;
            waterTolerance = GetComponent<SphereCollider>().radius;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<WaterComponent>())
            waterComponentEntered = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (waterComponentEntered != null)
        {
            float forceFactor = 1f - transform.position.y + waterLevel;
            playerController.jumpState.nbJumpMade = 0;

            if (transform.position.y > waterLevel - waterTolerance)
            {
                playerController.isGravityEnabled = true;
            }
            // Sous l'eau
            else
            {
                if (transform.position.y < waterLevel - 2 * waterTolerance)
                {
                    playerController.isGravityEnabled = false;

                    if (forceFactor > 0f)
                    {
                        Vector3 uplift = ((forceFactor - rb.velocity.y * modulateWaterForceFactor)) * Vector3.up * waterUpliftStrength;
                        rb.AddForceAtPosition(uplift, transform.position);
                    }
                }
            }

            if (PlayerComponent.cameraReference)
            {
                WaterImmersionCamera waterImmersionCamera = PlayerComponent.cameraReference.transform.GetChild(0).GetComponent<WaterImmersionCamera>();
                if (waterImmersionCamera)
                {
                    if (PlayerComponent.cameraReference.transform.GetChild(0).position.y < waterLevel)
                    {
                        waterImmersionCamera.isImmerge = true;
                    }
                    else
                    {
                        waterImmersionCamera.isImmerge = false;
                    }
                }

            }
        }
    }
}
