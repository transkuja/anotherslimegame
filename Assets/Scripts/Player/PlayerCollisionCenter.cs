using UnityEngine;
using System.Collections;
using System.Collections.Generic; // For List

public class PlayerCollisionCenter : MonoBehaviour {

    private PlayerControllerHub playerController;
    private PlayerCharacterHub playerCharacter;
    private Rigidbody rb;
    private Player player;


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
    private List<AIRabite> impactedRabite = new List<AIRabite>();

    private float invincibilityFrame = 2.0f;
    bool onceRepulsion;
    int separationMask;
    Collider[] playersCollided;
    float sphereCheckRadius;

    LayerMask defaultMask;

    int separationMaskRabite;
    Collider[] rabiteCollided;
    float sphereCheckRadiusRabite;

    int separationMask2;
    Collider[] collectablesCollided;
    float sphereCheckRadiusCollectables;

    int separationMask3;
    Collider[] breakablesCollided;

    // Edge climbing twerk values
    [Header("Edge climbing twerk values")]
    public float forwardAngleDetection = 80;
    public float maxHeightDetection = 0.8f;
    public float recalageForward = 0.0f;
    public float recalageUp = 0.25f;
    public float heightAngleForActivation = 25;

    bool hasCollidedWithAPlayer = false;
    public bool canBeHit = true;
    public float timerStopOnDashCollision = 0.3f;
    float currentTimerStop = 0.0f;

    WaterComponent waterComponentEntered;
    float waterLevel;
    float waterTolerance;
    public float waterUpliftStrength;
    [Range(0.01f, 1.0f)]
    public float modulateWaterForceFactor;
    public bool surfaceWaterAnimLaunched = false;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerController = player.PlayerController as PlayerControllerHub;
        playerCharacter = player.PlayerCharacter as PlayerCharacterHub;

        rb = playerCharacter.Rb;
    }

    void Start()
    {
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
        if (playerCharacter.PlayerState is DashState || playerCharacter.PlayerState is DashDownState)
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

                    if (playerCharacter.PlayerState is DashDownState)
                    {
                        if (!impactedPlayers.Contains(playersCollided[i].GetComponent<Player>()))
                        {
                            Player impactedPlayer = playersCollided[i].GetComponent<Player>();
                            impactedPlayer.GetComponent<PlayerCollisionCenter>().canBeHit = false;
                            impactedPlayers.Add(impactedPlayer);

                            UWPAndXInput.PlayerIndex impactedPlayerIndex = playersCollided[i].GetComponent<PlayerControllerHub>().PlayerIndex;
                            PlayerCharacterHub impactedPlayerCharacter = playersCollided[i].GetComponent<PlayerCharacterHub>();
                            Rigidbody impactedPlayerRigidbody = impactedPlayerCharacter.Rb;

                            Physics.IgnoreCollision(playersCollided[i], GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame

                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position + Vector3.up * 0.5f + playerCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerCharacter.PlayerState = playerCharacter.frozenState;
                            impactedPlayerCharacter.PlayerState = impactedPlayerCharacter.frozenState;

                            rb.velocity = Vector3.zero;
                            impactedPlayersOldVelocities.Add(impactedPlayerRigidbody.velocity);
                            impactedPlayerRigidbody.velocity = Vector3.zero;

                            //Set vibrations
                            UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.8f, 0.8f, .1f);
                            UWPAndXInput.GamePad.VibrateForSeconds(impactedPlayerIndex, 0.8f, 0.8f, .1f);

                            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                        }
                    }

                    else if (playersCollided[i].transform != transform && Vector3.Angle(playerToTarget, transform.forward) < 45) // Verification en cone
                    {
                        if (!impactedPlayers.Contains(playersCollided[i].GetComponent<Player>()))
                        {
                            Player impactedPlayer = playersCollided[i].GetComponent<Player>();
                            impactedPlayer.GetComponent<PlayerCollisionCenter>().canBeHit = false;
                            impactedPlayers.Add(impactedPlayer);

                
                            PlayerCharacterHub impactedPlayerCharacter = impactedPlayer.PlayerCharacter as PlayerCharacterHub;

                            Rigidbody impactedPlayerRigidbody = impactedPlayerCharacter.Rb;

                            Physics.IgnoreCollision(playersCollided[i], GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame

                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position+Vector3.up*0.5f + playerCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(playerToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerCharacter.PlayerState = playerCharacter.frozenState;
                            impactedPlayerCharacter.PlayerState = impactedPlayerCharacter.frozenState;

                            rb.velocity = Vector3.zero;
                            impactedPlayersOldVelocities.Add(impactedPlayerRigidbody.velocity);
                            impactedPlayerRigidbody.velocity = Vector3.zero;

                            //Set vibrations
                            UWPAndXInput.GamePad.VibrateForSeconds(playerController.playerIndex, 0.8f, 0.8f, .1f);

                            if (impactedPlayer.PlayerController)
                            {
                                UWPAndXInput.PlayerIndex impactedPlayerIndex = impactedPlayer.PlayerController.PlayerIndex;
                                UWPAndXInput.GamePad.VibrateForSeconds(impactedPlayerIndex, 0.8f, 0.8f, .1f);
                            }
             

                            if (AudioManager.Instance != null && AudioManager.Instance.punchFx != null)
                                AudioManager.Instance.PlayOneShot(AudioManager.Instance.punchFx);
                        }
                    }
                }
            }
        }


        if (playerCharacter.PlayerState is DashState || playerCharacter.PlayerState is DashDownState)
        {
            rabiteCollided = Physics.OverlapSphere(transform.position, sphereCheckRadiusRabite, separationMaskRabite);
            if (rabiteCollided.Length > 0)
            {
                for (int i = 0; i < rabiteCollided.Length; i++)
                {
                    Vector3 rabiteToTarget = rabiteCollided[i].transform.position - transform.position;
                    Vector3 rabiteCenterToTargetCenter = (rabiteCollided[i].transform.position + Vector3.up * 0.5f) - (transform.position + Vector3.up * 0.5f);

                    if (playerCharacter.PlayerState is DashDownState)
                    {
                        if (!impactedRabite.Contains(rabiteCollided[i].GetComponent<AIRabite>()))
                        {
                            Physics.IgnoreCollision(rabiteCollided[i], GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedRabite.Add(rabiteCollided[i].GetComponent<AIRabite>());
                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position + Vector3.up * 0.5f + rabiteCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(rabiteToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerCharacter.PlayerState = playerCharacter.frozenState;

                            rb.velocity = Vector3.zero;

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
                            Physics.IgnoreCollision(rabiteCollided[i], GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedRabite.Add(rabiteCollided[i].GetComponent<AIRabite>());
                            GameObject go = Instantiate(hitParticles);
                            go.transform.position = transform.position + Vector3.up * 0.5f + rabiteCenterToTargetCenter / 2.0f;
                            go.transform.rotation = Quaternion.LookRotation(rabiteToTarget, Vector3.up);
                            Destroy(go, 10.0f);
                            hasCollidedWithAPlayer = true;
                            currentTimerStop = timerStopOnDashCollision;

                            playerCharacter.PlayerState = playerCharacter.frozenState;
                          
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
                        Physics.IgnoreCollision(collectablesCollided[i], GetComponent<Collider>(), true);
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
            for (int i = 0; i < breakablesCollided.Length; i++)
            {
                breakablesCollided[i].GetComponent<Breakable>().HandleCollision(playerCharacter, playerController);
            }
        }

        if (hasCollidedWithAPlayer)
        {
            currentTimerStop -= Time.deltaTime;
            if (currentTimerStop <= 0.0f)
            {
                playerCharacter.PlayerState = playerCharacter.freeState;
                for (int i = 0; i < impactedPlayers.Count; i++)
                {
                    ((PlayerCharacterHub)impactedPlayers[i].PlayerCharacter).PlayerState = ((PlayerCharacterHub)impactedPlayers[i].PlayerCharacter).freeState;
                    ImpactHandling(impactedPlayers[i]);
                }
                for (int i = 0; i < impactedRabite.Count; i++)
                {
                    ImpactHandling(impactedRabite[i]);
                }
                hasCollidedWithAPlayer = false;
            }
        }

        // Visu invincibility
        if (!canBeHit)
        {
            player.Clignote();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Interaction Joueur / Joueur
        if (collision.gameObject.GetComponent<Player>())
        {
            Player collidedPlayer = collision.transform.gameObject.GetComponent<Player>();

            if (collidedPlayer == null) return;

            if (!(playerCharacter.PlayerState is DashState)
                && !(((PlayerCharacterHub)collidedPlayer.PlayerCharacter).PlayerState is DashState))
            {
                // Default interaction no one is dashing of using an abilty
                // Can't confirm implications
                DefaultCollision(collision, ((PlayerCharacterHub)collidedPlayer.PlayerCharacter));

                if (AudioManager.Instance != null && AudioManager.Instance.wahhFx != null)
                    //if (!AudioManager.Instance.sourceFX.isPlaying)
                        AudioManager.Instance.PlayOneShot(AudioManager.Instance.wahhFx);
            }
        }

        if (player.isEdgeAssistActive)
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
        if (playerCharacter != null && playerCharacter.PlayerState == playerCharacter.underwaterState && playerCharacter.underwaterState != null && 
            (playerCharacter.underwaterState.hasReachedTheSurface || transform.position.y > playerCharacter.underwaterState.waterLevel))
        {
            if (collision.gameObject.layer == defaultMask)
            {
                playerCharacter.PlayerState = playerCharacter.freeState;
            }
        }
    }

    public void OnCollisionStay(Collision collision)
    {
        // Wall jump
        if (playerCharacter.GetComponent<EvolutionAgile>() != null)
        {
            if (!playerCharacter.IsGrounded && !(playerCharacter.PlayerState == playerCharacter.wallJumpState) && collision.gameObject.layer == defaultMask.value)
            {
                if (Vector3.Angle(Vector3.up, collision.contacts[0].normal) > 65 || collision.contacts[0].normal == Vector3.zero) // a partir de quel moment le mur est trop incliné, (marche dessus plutot que sauter)
                {
                    playerCharacter.wallJumpState.pushDirection = collision.contacts[0].normal;
                    playerCharacter.PlayerState = playerCharacter.wallJumpState;
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
                DamagePlayer(playerImpacted.GetComponent<Player>(), PlayerUIStat.Points);
        }
        
        ExpulsePlayer(playerImpacted.GetComponent<Collider>().ClosestPoint(transform.position), playerImpacted.PlayerCharacter.Rb, repulsionFactor);
    }


    public void DefaultCollision(Collision collision, PlayerCharacterHub impactedPlayerCharacter)
    {
        //PlayerBouncyPhysics
        if ((transform.position.y - collision.transform.position.y) > bounceDetectionThreshold)
        {
            rb.velocity += Vector3.up * bounceStrength;
        }
        else
        {
            if (rb.velocity.magnitude > impactedPlayerCharacter.Rb.velocity.magnitude)
            {
                if (rb.velocity.magnitude > impactPropagationThreshold)
                    impactedPlayerCharacter.Rb.velocity += (rb.velocity * impactForce);
            }
            else
            {
                if (impactedPlayerCharacter.Rb.velocity.magnitude > impactPropagationThreshold)
                    rb.velocity += (impactedPlayerCharacter.Rb.velocity * impactForce);
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

    public IEnumerator ReactivateCollider(Player p)
    {
        yield return new WaitForSeconds(invincibilityFrame);
        p.GetComponent<PlayerCollisionCenter>().canBeHit = true;
        p.ArreteDeClignoter();
        yield return new WaitForSeconds(0.3f);
        impactedPlayers.Remove(p);
        Physics.IgnoreCollision(p.GetComponent<Collider>(), GetComponent<Collider>(), false);

        yield return null;
    }

    public void ForcedJump(Vector3 direction, float repulseStrength, Rigidbody target)
    {
        if (target.GetComponent<PlayerCharacterHub>() != null)
        {
            PlayerCharacterHub _pcTarget = target.GetComponent<PlayerCharacterHub>();
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
        yield return new WaitForSeconds(invincibilityFrame/2.0f);
        impactedRabite.Remove(p);
        Physics.IgnoreCollision(p.GetComponent<Collider>(), GetComponent<Collider>(), false);
        yield return null;
    }
}
