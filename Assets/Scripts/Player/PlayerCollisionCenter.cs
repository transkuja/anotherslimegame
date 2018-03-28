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

    LayerMask defaultMask;

    private List<AIRabite> impactedRabite = new List<AIRabite>();
    int separationMaskRabite;
    Collider[] rabiteCollided;
    float sphereCheckRadiusRabite;

    int separationMask2;
    Collider[] collectablesCollided;
    float sphereCheckRadiusCollectables;

    int separationMask3;
    Collider[] breakablesCollided;

    PlayerControllerHub playerController;
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
    public bool surfaceWaterAnimLaunched = false;

    PlayerControllerHub _PlayerController
    {
        get
        {
            if (playerController == null)
                playerController = GetComponent<PlayerControllerHub>();
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
        playerController = GetComponent<PlayerControllerHub>();
        rb = GetComponent<Rigidbody>();
        repulsionFactor = 35;
        impactedRabite = new List<AIRabite>();

        // default mask
        defaultMask = 0;

        separationMaskRabite = LayerMask.GetMask(new string[] { "Rabite" });
        sphereCheckRadiusRabite = 5.0f;

        separationMask = LayerMask.GetMask(new string[] { "Player", "GhostPlayer" });
        sphereCheckRadius = 5.0f;

        separationMask2 = LayerMask.GetMask(new string[] { "Collectable" });
        sphereCheckRadiusCollectables = 3.0f;

        separationMask3 = LayerMask.GetMask(new string[] { "Breakable" });

    }

    private void Update()
    {
        if (playerController.PlayerState is DashState || playerController.PlayerState is DashDownState)
        {
            playersCollided = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask);
            if (playersCollided.Length > 0)
            {
                for (int i = 0; i < playersCollided.Length; i++)
                {
                    Vector3 playerToTarget = playersCollided[i].transform.position - transform.position;
                    Vector3 playerCenterToTargetCenter = (playersCollided[i].transform.position + Vector3.up * 0.5f) - (transform.position + Vector3.up * 0.5f);
                    
                    // TODO : faire une fonction plus tard
                    if (player == playersCollided[i].GetComponent<Player>())
                        continue;

                    if ( playerController.PlayerState is DashDownState)
                    {
                        if (!impactedPlayers.Contains(playersCollided[i].GetComponent<Player>()))
                        {
                            Physics.IgnoreCollision(playersCollided[i].GetComponent<Collider>(), GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedPlayers.Add(playersCollided[i].GetComponent<Player>());
                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position + Vector3.up * 0.5f + playerCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerController.PlayerState = playerController.frozenState;
                            playersCollided[i].GetComponent<PlayerControllerHub>().PlayerState = playersCollided[i].GetComponent<PlayerControllerHub>().frozenState;
                            velocityOnImpact = playerController.Rb.velocity;
                            playerController.Rb.velocity = Vector3.zero;
                            impactedPlayersOldVelocities.Add(playersCollided[i].GetComponent<Rigidbody>().velocity);
                            playersCollided[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //Set vibrations
                            UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.8f, 0.8f, .1f);
                            UWPAndXInput.GamePad.VibrateForSeconds(playersCollided[i].GetComponent<PlayerControllerHub>().playerIndex, 0.8f, 0.8f, .1f);

                            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                        }
                    }

                    else if (playersCollided[i].transform != transform && Vector3.Angle(playerToTarget, transform.forward) < 45) // Verification en cone
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
                            playersCollided[i].GetComponent<PlayerControllerHub>().PlayerState = playersCollided[i].GetComponent<PlayerControllerHub>().frozenState;
                            velocityOnImpact = playerController.Rb.velocity;
                            playerController.Rb.velocity = Vector3.zero;
                            impactedPlayersOldVelocities.Add(playersCollided[i].GetComponent<Rigidbody>().velocity);
                            playersCollided[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //Set vibrations
                            UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.8f, 0.8f, .1f);
                            UWPAndXInput.GamePad.VibrateForSeconds(playersCollided[i].GetComponent<PlayerControllerHub>().playerIndex, 0.8f, 0.8f, .1f);

                            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                        }
                    }
                }
            }
        }


        if (playerController.PlayerState is DashState || playerController.PlayerState is DashDownState)
        {
            rabiteCollided = Physics.OverlapSphere(transform.position, sphereCheckRadiusRabite, separationMaskRabite);
            if (rabiteCollided.Length > 0)
            {
                for (int i = 0; i < rabiteCollided.Length; i++)
                {
                    Vector3 rabiteToTarget = rabiteCollided[i].transform.position - transform.position;
                    Vector3 rabiteCenterToTargetCenter = (rabiteCollided[i].transform.position + Vector3.up * 0.5f) - (transform.position + Vector3.up * 0.5f);

                    if (playerController.PlayerState is DashDownState)
                    {
                        if (!impactedRabite.Contains(rabiteCollided[i].GetComponent<AIRabite>()))
                        {
                            Physics.IgnoreCollision(rabiteCollided[i].GetComponent<Collider>(), GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedRabite.Add(rabiteCollided[i].GetComponent<AIRabite>());
                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position + Vector3.up * 0.5f + rabiteCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(rabiteToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerController.PlayerState = playerController.frozenState;

                            //rabiteCollided[i].GetComponent<PlayerControllerHub>().PlayerState = playersCollided[i].GetComponent<PlayerControllerHub>().frozenState;
                            velocityOnImpact = playerController.Rb.velocity;
                            playerController.Rb.velocity = Vector3.zero;

                            rabiteCollided[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //Set vibrations
                            UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.8f, 0.8f, .1f);

                            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                        }
                    }

                    else if (rabiteCollided[i].transform != transform && Vector3.Angle(rabiteToTarget, transform.forward) < 45) // Verification en cone
                    {
                        if (!impactedRabite.Contains(rabiteCollided[i].GetComponent<AIRabite>()))
                        {
                            Physics.IgnoreCollision(rabiteCollided[i].GetComponent<Collider>(), GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedRabite.Add(rabiteCollided[i].GetComponent<AIRabite>());
                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position + Vector3.up * 0.5f + rabiteCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(rabiteToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerController.PlayerState = playerController.frozenState;
                          
                            velocityOnImpact = playerController.Rb.velocity;
                            playerController.Rb.velocity = Vector3.zero;
                            rabiteCollided[i].GetComponent<Rigidbody>().velocity = Vector3.zero;

                            //Set vibrations
                            UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.8f, 0.8f, .1f);

                            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                        }
                    }
                }
            }
        }


        collectablesCollided = Physics.OverlapSphere(transform.position, sphereCheckRadiusCollectables, separationMask2);
        if (collectablesCollided.Length > 0)
        {
            for (int i = 0; i < collectablesCollided.Length; i++)
            {
                //Vector3 collectableToTarget = collectablesCollided[i].transform.position - transform.position;

                if (collectablesCollided[i].transform != transform) // Verification en cone
                {
                    Collectable c = collectablesCollided[i].GetComponent<Collectable>();
                    if (c.isActiveAndEnabled && !c.IsAttracted && !c.haveToDisperse)
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
                    impactedPlayers[i].GetComponent<PlayerControllerHub>().PlayerState = impactedPlayers[i].GetComponent<PlayerControllerHub>().freeState;
                    ImpactHandling(impactedPlayers[i]);
                }
                for (int i = 0; i < impactedRabite.Count; i++)
                {
                    ImpactHandling(impactedRabite[i]);
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
            PlayerControllerHub collidedPlayerController = collision.transform.gameObject.GetComponent<PlayerControllerHub>();

            if (collidedPlayerController == null) return;

            if (!(_PlayerController.PlayerState is DashState)
                && !(collidedPlayerController.PlayerState is DashState))
            {
                // Default interaction no one is dashing of using an abilty
                // Can't confirm implications
                DefaultCollision(collision, collision.transform.gameObject.GetComponent<Player>());

                if (AudioManager.Instance != null && AudioManager.Instance.wahhFx != null)
                    //if (!AudioManager.Instance.sourceFX.isPlaying)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.wahhFx);
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

        // Collision with ground when underwater
        if (playerController != null && playerController.PlayerState == playerController.underwaterState && playerController.underwaterState != null && 
            (playerController.underwaterState.hasReachedTheSurface || transform.position.y > playerController.underwaterState.waterLevel))
        {
            if (collision.gameObject.layer == defaultMask)
            {
                playerController.PlayerState = playerController.freeState;
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        // Wall jump
        if (playerController.GetComponent<EvolutionAgile>() != null)
        {
            if (!playerController.IsGrounded && !(playerController.PlayerState == playerController.wallJumpState) && collision.gameObject.layer == defaultMask.value)
            {
                if (Vector3.Angle(Vector3.up, collision.contacts[0].normal) > 65 || collision.contacts[0].normal == Vector3.zero) // a partir de quel moment le mur est trop incliné, (marche dessus plutot que sauter)
                {
                    playerController.wallJumpState.pushDirection = collision.contacts[0].normal;
                    playerController.PlayerState = playerController.wallJumpState;
                }
            }

        }

   
    }

    public void ImpactHandling(Player playerImpacted)
    {

        // Damage Behavior
        if (GameManager.Instance.CurrentGameMode.TakesDamageFromPlayer)
        {
            if (GameManager.Instance.IsInHub())
                DamagePlayerHub();
            else
                DamagePlayer(playerImpacted, PlayerUIStat.Points);
        }
        
        ExpulsePlayer(playerImpacted.GetComponent<Collider>().ClosestPoint(transform.position), playerImpacted.Rb, repulsionFactor);
    }


    public void DefaultCollision(Collision collision, Player playerImpacted)
    {
        //PlayerBouncyPhysics
        if ((transform.position.y - collision.transform.position.y) > bounceDetectionThreshold)
        {
            _Rb.velocity += Vector3.up * bounceStrength;
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



    public void DamagePlayerHub()
    {
        if (GameManager.Instance.GlobalMoney == 0)
            return;

        int numberOfCollectablesToDrop = (int)Mathf.Clamp(((float)GameManager.Instance.GlobalMoney / Utils.GetDefaultCollectableValue((int)CollectableType.Money)), 1, 6);

        GameManager.Instance.GlobalMoney -= numberOfCollectablesToDrop * Utils.GetDefaultCollectableValue((int)CollectableType.Money);

        for (int i = 0; i < numberOfCollectablesToDrop; i++)
        {
            GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Money).GetItem(null, transform.position + Vector3.up * 0.5f, player.transform.rotation, true);
            go.GetComponent<Collectable>().Disperse(i);
        }
    }

    public void DamagePlayer(Player player, PlayerUIStat _damageOn)
    {
        // Damage Behavior
        int typeCollectable = -1;
        int quantity = 0;

        if (_damageOn == PlayerUIStat.Life)
        {
            quantity = player.NbLife;
            // TMP
            typeCollectable = (int)CollectableType.Points;
        }

        else if (_damageOn == PlayerUIStat.Points)
        {
            quantity = player.NbPoints;
            // TMP
            typeCollectable = (int)CollectableType.Points;
        }

        if (quantity == 0 || typeCollectable == -1)
            return;



        int numberOfCollectablesToDrop = (int)Mathf.Clamp(((float)quantity / Utils.GetDefaultCollectableValue(typeCollectable)), 1, 6);

        player.UpdateCollectableValue((CollectableType)typeCollectable, -numberOfCollectablesToDrop * Utils.GetDefaultCollectableValue(typeCollectable));

        // Drop Points
        for (int i = 0; i < numberOfCollectablesToDrop; i++)
        {
            if (_damageOn == PlayerUIStat.Points)
            {
                GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.CollectablePoints).GetItem(null, transform.position + Vector3.up * 0.5f, player.transform.rotation, true);
                go.GetComponent<Collectable>().Disperse(i);
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
        if (!onceRepulsion && rbPlayerToExpulse.GetComponent<PlayerControllerHub>())
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
        if (target.GetComponent<PlayerControllerHub>() != null)
        {
            PlayerControllerHub _pcTarget = target.GetComponent<PlayerControllerHub>();
            _pcTarget.jumpState.NbJumpMade = 0;
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

    public void ImpactHandling(AIRabite rabite)
    {
        rabite.CurrentState = AIRabite.RabiteState.Dead;
        ExpulseRabite(rabite.GetComponent<Collider>().ClosestPoint(transform.position), rabite.GetComponent<Rigidbody>(), repulsionFactor);
    }

    public void ExpulseRabite(Vector3 collisionPoint, Rigidbody rbPlayerToExpulse, float repulsionFactor)
    {
        Vector3 direction = rbPlayerToExpulse.position - collisionPoint;
        direction.y = 0;

        direction.Normalize() ;
        //rbPlayerToExpulse.AddForce(new Vector3(direction.x * repulsionFactor, rbPlayerToExpulse.velocity.y + 15.0f, direction.z * repulsionFactor), ForceMode.Impulse);
        rbPlayerToExpulse.AddForceAtPosition(new Vector3(direction.x * repulsionFactor, rbPlayerToExpulse.velocity.y + 15.0f, direction.z * repulsionFactor), rbPlayerToExpulse.position + Vector3.up,ForceMode.Impulse);

        StartCoroutine(ReactivateCollider(rbPlayerToExpulse.GetComponent<AIRabite>()));
    }

    public IEnumerator ReactivateCollider(AIRabite p)
    {
        yield return new WaitForSeconds(invicibilityFrame/2.0f);
        impactedRabite.Remove(p);
        Physics.IgnoreCollision(p.GetComponent<Collider>(), GetComponent<Collider>(), false);
        yield return null;
    }
}
