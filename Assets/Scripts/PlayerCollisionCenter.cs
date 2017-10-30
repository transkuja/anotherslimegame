using UnityEngine;
using System; // For math

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

    // Collision with strengh on one player
    [SerializeField] float expulseDashFactor = 30;
    // Collision with strengh on one player
    [SerializeField] float expulseStrenghFactor = 60;


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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            PlayerController collidedPlayerController = collision.transform.gameObject.GetComponent<PlayerController>();

            if (collidedPlayerController == null) return;

            if (_PlayerController.DashingState != SkillState.Dashing
                && _PlayerController.StrenghState != SkillState.Dashing
                && collidedPlayerController.DashingState != SkillState.Dashing 
                && collidedPlayerController.StrenghState != SkillState.Dashing)
            {
                // Default interaction no one is dashing of using an abilty
                // Could be reduce to thisPlayerController.BrainState == BrainState.Occupied && collidedPlayerController.BrainState == BrainState.Occupied
                // Can't confirm implications
                DefaultCollision(collision, collision.transform.gameObject.GetComponent<Player>());
            }
            else
            {
                // At least one is dashing
                if ( (_PlayerController.DashingState == SkillState.Dashing || _PlayerController.StrenghState == SkillState.Dashing)
                   && (collidedPlayerController.DashingState != SkillState.Dashing && collidedPlayerController.StrenghState != SkillState.Dashing) )
                {
                    // ThisPlayer is dashing
                    DamagePlayer(collision.transform.gameObject.GetComponent<Player>());

                    // ExpluseForce
                    ExpulsePlayer(collision, collision.transform.gameObject.GetComponent<Rigidbody>(), expulseDashFactor);
                }
                else if ((_PlayerController.DashingState == SkillState.Dashing || _PlayerController.StrenghState == SkillState.Dashing)
                         && (collidedPlayerController.DashingState != SkillState.Dashing && collidedPlayerController.StrenghState != SkillState.Dashing))
                {
                    // Collided is dashing

                    DamagePlayer(_PlayerController.GetComponent<Player>());

                    // ExpluseForce
                    ExpulsePlayer(collision, _Rb, expulseDashFactor);
                }
                else if ((_PlayerController.DashingState == SkillState.Dashing || _PlayerController.StrenghState == SkillState.Dashing)
                        && (collidedPlayerController.DashingState != SkillState.Dashing && collidedPlayerController.StrenghState != SkillState.Dashing))
                {
                    // Both are dashing

                    // Damage Behavior
                    DamagePlayer(collision.transform.gameObject.GetComponent<Player>());
                    DamagePlayer(_PlayerController.GetComponent<Player>());

                    // ExpluseForce
                    ExpulsePlayer(collision, _Rb, expulseDashFactor);
                    ExpulsePlayer(collision, collision.transform.gameObject.GetComponent<Rigidbody>(), expulseDashFactor);
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
            for (int i = 0; i < Mathf.Clamp((float)Mathf.Floor(player.Collectables[typeCollectable] / Utils.GetDefaultCollectableValue(typeCollectable)), 1, 2); i++)
            {
                GameObject go = ResourceUtils.Instance.refPrefabLoot.SpawnCollectableInstance(
                player.transform.position,
                player.transform.rotation,
                null,
                CollectableType.Points);

                //Dispersion des collectables
                go.GetComponent<Collectable>().Dispersion(i);

                go.GetComponent<SphereCollider>().enabled = false;
                player.UpdateCollectableValue(CollectableType.Points, -(int)go.GetComponent<Collectable>().Value);
                StartCoroutine(go.GetComponent<Collectable>().ReactivateCollider());
            }
        }
    }

    public void ExpulsePlayer(Collision collision, Rigidbody rbPlayerToExpulse, float expulseFactor)
    {
        Vector3 direction = collision.contacts[0].point - rb.position;
        direction.y = 0;
        direction.Normalize();

        rbPlayerToExpulse.AddForce(direction * expulseFactor, ForceMode.Impulse);
    }
}
