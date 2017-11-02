using UnityEngine;
using System; // For math
using System.Collections;

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
    [SerializeField] float repulsionFactor = 30;

    PlayerController playerController;
    Rigidbody rb;
    bool onceRepulsion;


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
        if (playerController.DashingState == SkillState.Dashing)
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
                        // Damage Behavior
                        DamagePlayer(playersCollided[i].GetComponent<Player>());

                        // ExpluseForce
                        //if (_PlayerController.StrengthState == SkillState.Dashing) repulsionMultiplier *= -2;
                        RepulseRigibody(playersCollided[i].ClosestPoint(transform.position), playersCollided[i].gameObject.GetComponent<Rigidbody>(), repulsionFactor);
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
            else
            {
                int repulsionMultiplier = -1;

                // At least one is dashing
                if ( (_PlayerController.DashingState == SkillState.Dashing || _PlayerController.StrengthState == SkillState.Dashing)
                   && (collidedPlayerController.DashingState != SkillState.Dashing && collidedPlayerController.StrengthState != SkillState.Dashing) )
                {
                    // ThisPlayer is dashing

                    // Damage Behavior
                    DamagePlayer(collision.transform.gameObject.GetComponent<Player>());

                    // ExpluseForce
                    if (_PlayerController.StrengthState == SkillState.Dashing) repulsionMultiplier *= -2;
                    //RepulseRigibody(collision, collision.transform.gameObject.GetComponent<Rigidbody>(), repulsionFactor * repulsionMultiplier);
                }
                else if ((_PlayerController.DashingState != SkillState.Dashing && _PlayerController.StrengthState != SkillState.Dashing)
                         && (collidedPlayerController.DashingState == SkillState.Dashing && collidedPlayerController.StrengthState != SkillState.Dashing))
                {
                    // Collided is dashing

                    // Damage Behavior
                    DamagePlayer(_PlayerController.GetComponent<Player>());

                    // ExpluseForce
                    if (collidedPlayerController.StrengthState == SkillState.Dashing) repulsionMultiplier *= -2;
                    //RepulseRigibody(collision, _Rb, repulsionFactor * repulsionMultiplier);
                }
                else if ((_PlayerController.DashingState == SkillState.Dashing || _PlayerController.StrengthState == SkillState.Dashing)
                        && (collidedPlayerController.DashingState == SkillState.Dashing || collidedPlayerController.StrengthState == SkillState.Dashing))
                {
                    // Both are dashing

                    // Damage Behavior
                    DamagePlayer(collision.transform.gameObject.GetComponent<Player>());
                    DamagePlayer(_PlayerController.GetComponent<Player>());

                    // ExpluseForce
                    // Double the love
                    if (_PlayerController.StrengthState == SkillState.Dashing) repulsionMultiplier *= -2;
                    if (collidedPlayerController.StrengthState == SkillState.Dashing) repulsionMultiplier *= -2;
                    //RepulseRigibody(collision, _Rb, repulsionFactor * repulsionMultiplier);
                    //RepulseRigibody(collision, collision.transform.gameObject.GetComponent<Rigidbody>(), repulsionFactor * repulsionMultiplier);
                }
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
            for (int i = 0; i < Mathf.Clamp((float)(Mathf.Floor(player.Collectables[typeCollectable]) / Utils.GetDefaultCollectableValue(typeCollectable)), 1, 2); i++)
            {
                GameObject go = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                player.transform.position,
                player.transform.rotation,
                null,
                CollectableType.Points);

                //Dispersion des collectables
                go.GetComponent<Collectable>().Dispersion(i);

                go.GetComponent<SphereCollider>().enabled = false;
                player.UpdateCollectableValue(CollectableType.Points, -Utils.GetDefaultCollectableValue(typeCollectable));
                StartCoroutine(go.GetComponent<Collectable>().ReactivateCollider());
            }
        }
    }

    public void RepulseRigibody(Vector3 collisionPoint, Rigidbody rbPlayerToExpulse, float repulsionFactor)
    {
     
        if (!onceRepulsion)
        {
            Vector3 direction = rbPlayerToExpulse.position - collisionPoint;
            direction.y = 0;

            direction.Normalize();

            rbPlayerToExpulse.AddForce(direction * repulsionFactor, ForceMode.Impulse);
            onceRepulsion = true;
            StartCoroutine(ReactivateCollider());
        }

    }

    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(0.5f);
        onceRepulsion = false;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, 4.0f);
    }

   
}
