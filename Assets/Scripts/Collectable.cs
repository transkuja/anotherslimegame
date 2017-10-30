using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    CollectableType type;
    private float value;
    bool isAttracted = false;
    uint movementSpeed = 40;
    Player playerTarget;

    public Vector3[] position = new Vector3[4];

    public float Value
    {
        get
        {
            return value;
        }

        set
        {
            this.value = value;
        }
    }

    public void Start()
    {
        Value = Utils.GetDefaultCollectableValue(type);

        position[0] = new Vector3(-2, 0, -2);
        position[1] = new Vector3(2, 0, -2);
        position[2] = new Vector3(-2, 0, 2);
        position[3] = new Vector3(2, 0, 2);
    }

    private void OnTriggerEnter(Collider other)
    {
        PickUp(other.GetComponent<Player>());
    }

    private void PickUp(Player player)
    {
        if (player && !isAttracted)
        {
            // Grab everything not linked to evolution (points)
            if (!Utils.IsAnEvolutionCollectable(type))
            {
                if (player.Collectables[(int)type] < Utils.GetMaxValueForCollectable(type))
                {
                    if (AudioManager.Instance != null && AudioManager.Instance.coinFX!=null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.coinFX);
                    isAttracted = true;
                    playerTarget = player;
                    return;
                }
            }
            else
            {
                if (GameManager.CurrentGameMode.evolutionMode != EvolutionMode.GrabEvolution)
                {
                    if (player.Collectables[(int)type] < Utils.GetMaxValueForCollectable(type))
                    {
                        isAttracted = true;
                        playerTarget = player;
                    }
                }
                else
                {
                    if (player.activeEvolutions == 0)
                    {
                        isAttracted = true;
                        playerTarget = player;
                    }
                }
            }

        }    
    }

    private void FixedUpdate()
    {
        if (isAttracted)
            Attract();
    }

    void Attract()
    {
        Vector3 direction = (playerTarget.transform.position - transform.position).normalized;
   
        GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < GetComponent<MeshFilter>().mesh.bounds.extents.magnitude)
        {
            playerTarget.UpdateCollectableValue(type, (int)value);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<SphereCollider>().enabled = true;
        yield return null;
    }

    // if index is need to add a little bit more random
    // should not use the same index twice
    public Vector3 Dispersion(int index)
    {
        return position[UnityEngine.Random.Range(0, 3)];
    }
}
