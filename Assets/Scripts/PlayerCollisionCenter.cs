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
    }

    private void Update()
    {
        if (playerController.DashingState == SkillState.Dashing || playerController.StrengthState == SkillState.Dashing)
        {
            int separationMask = LayerMask.GetMask(new string[] { "Player" });
            Collider[] playersCollided;
            float sphereCheckRadius = 4.0f;

            playersCollided = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask);
            if (playersCollided != null)
            {

                for (int i = 0; i < playersCollided.Length; i++)
                {
                    if (playersCollided[i].transform != transform)
                    {
                        if (!impactedPlayers.Contains(playersCollided[i].GetComponent<Player>()))
                        {
                            // Don't reimpacted the same player twice see invicibilityFrame
                            impactedPlayers.Add(playersCollided[i].GetComponent<Player>());

                            // Damage Behavior
                            DamagePlayer(playersCollided[i].GetComponent<Player>());

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

            if (_PlayerController.DashingState != SkillState.Dashing
                && _PlayerController.StrengthState != SkillState.Dashing
                && collidedPlayerController.DashingState != SkillState.Dashing 
                && collidedPlayerController.StrengthState != SkillState.Dashing)
            {
                // Default interaction no one is dashing of using an abilty
                // Could be reduce to thisPlayerController.BrainState == BrainState.Occupied && collidedPlayerController.BrainState == BrainState.Occupied
                // Can't confirm implications
                DefaultCollision(collision, collision.transform.gameObject.GetComponent<Player>());
            }
        }

        if (collision.gameObject.tag == "HardBreakable")
        {
            if (GetComponent<EvolutionStrength>() != null && (GetComponent<PlayerController>().StrengthState == SkillState.Dashing || GetComponent<PlayerController>().DashingState == SkillState.Dashing))
            {
                if (collision.gameObject.GetComponent<Rigidbody>() != null)
                {
                    // TMP impredictable
                    collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    collision.gameObject.transform.parent = null;
                    //RepulseRigibody(collision, collision.gameObject.GetComponent<Rigidbody>(), repulsionFactor);
                    //Destroy(collision.gameObject, 4);
                }
            }
        }

        if (collision.gameObject.tag == "Breakable")
        {
            if (GetComponent<PlayerController>().StrengthState == SkillState.Dashing || GetComponent<PlayerController>().DashingState == SkillState.Dashing)
            {
                if (collision.gameObject.GetComponent<Rigidbody>() != null)
                {
                    // TMP impredictable
                    collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    collision.gameObject.transform.parent = null;
                    //RepulseRigibody(collision, collision.gameObject.GetComponent<Rigidbody>(), repulsionFactor);
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
            int numberOfCollectablesToDrop = (int)Mathf.Clamp((float)(Mathf.Floor(player.Collectables[typeCollectable]) / Utils.GetDefaultCollectableValue(typeCollectable)), 1, 2);
            for (int i = 0; i < numberOfCollectablesToDrop; i++)
            {
                player.UpdateCollectableValue(CollectableType.Points, -Utils.GetDefaultCollectableValue(typeCollectable));

                GameObject go = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                player.transform.position + player.GetComponent<MeshCollider>().bounds.extents,
                player.transform.rotation,
                null,
                CollectableType.Points);

                //Dispersion des collectables
                go.GetComponent<Collectable>().Dispersion(i, numberOfCollectablesToDrop);
            }
        }
    }

    public void ExpulsePlayer(Vector3 collisionPoint, Rigidbody rbPlayerToExpulse, float repulsionFactor)
    {
        if (!onceRepulsion)
        {
            onceRepulsion = true;

            Vector3 direction = rbPlayerToExpulse.position - collisionPoint;
            direction.y = 0;

            direction.Normalize();

            ForcedJump(direction, repulsionFactor, rbPlayerToExpulse);
            StartCoroutine(ReactivateCollider(rbPlayerToExpulse.GetComponent<Player>()));
        }
    }

    public void RepulseRigibody(Vector3 collisionPoint, Rigidbody rbPlayerToExpulse, float repulsionFactor)
    {
        if (!onceRepulsion)
        {
            onceRepulsion = true;

            Vector3 direction = rbPlayerToExpulse.position - collisionPoint;
            direction.y = 0;

            direction.Normalize();

            rbPlayerToExpulse.AddForce(direction * repulsionFactor, ForceMode.Impulse);
            StartCoroutine(ReactivateCollider(rbPlayerToExpulse.GetComponent<Player>()));
        }
    }

    public IEnumerator ReactivateCollider(Player p)
    {
        yield return new WaitForSeconds(invicibilityFrame);
        onceRepulsion = false;
        impactedPlayers.Remove(p);
        p.GetComponent<PlayerController>().BrainState = BrainState.Free;
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

            PlayerController _pc = target.GetComponent<PlayerController>();
            _pc.forcedJump.repulseForce = direction * repulseStrength;
            // Wtf je sais que c'est aussi fait dans le player Controller mais le comportement est bien meilleur avec cette ligne
            _pc.forcedJump.AddForcedJumpForce(target);  
            _pc.forcedJump.StartJump();
            _pc.BrainState = BrainState.Occupied;
            _pc.Jump();
        }
    }
}
