using UnityEngine;
using System; // For math
using System.Collections;
using System.Collections.Generic; // For List


public class PlayerCollisionCenter : MonoBehaviour {

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
    public float invicibilityFrame = 1.0f;
    bool onceRepulsion;


    PlayerController playerController;
    Rigidbody rb;

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

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        repulsionFactor = 35;
    }

    private void Update()
    {
        if (playerController.PlayerState is DashState)
        {
            int separationMask = LayerMask.GetMask(new string[] { "Player" });
            Collider[] playersCollided;
            float sphereCheckRadius = 4.0f;

            playersCollided = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask);
            if (playersCollided != null)
            {
                for (int i = 0; i < playersCollided.Length; i++)
                {
                    Vector3 playerToTarget = playersCollided[i].transform.position - transform.position;
                    if (playersCollided[i].transform != transform && Vector3.Angle(playerToTarget, transform.forward) < 45) // Verification en cone
                    {
                        if (!impactedPlayers.Contains(playersCollided[i].GetComponent<Player>()))
                        {
                            Physics.IgnoreCollision(playersCollided[i].GetComponent<Collider>(), GetComponent<Collider>(), true);
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedPlayers.Add(playersCollided[i].GetComponent<Player>());

                            // Damage Behavior
                            DamagePlayer(playersCollided[i].GetComponent<Player>());
                            //Physics.IgnoreCollision()
                            // ExpluseForce

                            //if (_PlayerController.StrengthState == SkillState.Dashing) repulsionMultiplier *= -2;
                            ExpulsePlayer(playersCollided[i].ClosestPoint(transform.position), playersCollided[i].GetComponent<Rigidbody>(), repulsionFactor);
                            //RepulseRigibody(playersCollided[i].ClosestPoint(transform.position), playersCollided[i].GetComponent<Rigidbody>(), repulsionFactor);
                        }
                    }
                }
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
            if (playerController.PlayerState is DashState && playerController.GetComponent<EvolutionStrength>())
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
            if (playerController.PlayerState is DashState && playerController.GetComponent<EvolutionStrength>())
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
        int typeCollectable = -1;
        switch (GameManager.CurrentGameMode.gameModeType)
        {
            case GameModeType.Escape:
                typeCollectable = (int)CollectableType.Points; break;
            case GameModeType.Arena:
                typeCollectable = (int)CollectableType.Points; break;
            default:
                break;
        }

        if (typeCollectable == -1) return;

        if (player.Collectables[typeCollectable] > 0)
        {
            int numberOfCollectablesToDrop = (int)Mathf.Clamp((float)(Mathf.Floor(player.Collectables[typeCollectable]) / Utils.GetDefaultCollectableValue(typeCollectable)), 1, 6);
            Vector3[] positions = SpawnManager.GetVector3ArrayOnADividedCircle(transform.position, player.GetComponent<MeshCollider>().bounds.extents.magnitude, numberOfCollectablesToDrop, SpawnManager.Axis.XZ);
            for (int i = 0; i < numberOfCollectablesToDrop; i++)
            {
                player.UpdateCollectableValue(CollectableType.Points, -Utils.GetDefaultCollectableValue(typeCollectable));

                GameObject go = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                positions[i] + Vector3.up*0.5f,
                player.transform.rotation,
                null,
                CollectableType.Points,
                true);

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
        //p.GetComponent<PlayerController>().BrainState = BrainState.Free;
        yield return null;
    }
    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(invicibilityFrame);
        onceRepulsion = false;
        //p.GetComponent<PlayerController>().BrainState = BrainState.Free;
        yield return null;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, 4.0f);
    }

    // c'est ça qui faut faire cadeau 
    //class ForcedJump
    //{
    //    bool forcedJumpInitialized = false;
    //    Vector3 direction;
    //}
    //ForcedJump jump;
    //AnimatorUpdateMode
    //if (Jump.forcedJumpInit)
    //    this.Rb.velocity = direction;


    void ForcedJump(Vector3 direction, float repulseStrength, Rigidbody target)
    {
        if (target.GetComponent<PlayerController>() != null)
        {
            PlayerController _pcTarget = target.GetComponent<PlayerController>();
            // pour le moment je laisse la state saut géré l'expulsion pour intégrer plus vite
            //mais à terme il faut le changer. On applique une force. C'est pas un état de saut.
            _pcTarget.jumpState.nbJumpMade = 0;
            _pcTarget.PlayerState.OnJumpPressed();
            _pcTarget.PlayerState.PushPlayer(direction* repulseStrength);
        }
    }
}
