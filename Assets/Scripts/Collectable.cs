using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    CollectableType type;
    private float value;
    bool isAttracted = false;
    bool haveToDisperse = false;

    public Vector3[] positions;
    private int myIndex = 0;

    uint movementSpeed = 40;
    Player playerTarget;
    
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
        Value = Utils.GetDefaultCollectableValue((int)type);
        positions = SpawnManager.GetVector3ArrayOnADividedCircle(transform.position + Vector3.up, 6, 2, SpawnManager.Axis.XZ);
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
        if (haveToDisperse)
            Disperse();
    }

    void Attract()
    {
        Vector3 direction = (playerTarget.transform.position - transform.position).normalized;
   
        GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < GetComponentInChildren<MeshFilter>().mesh.bounds.extents.magnitude)
        {
            playerTarget.UpdateCollectableValue(type, (int)value);
            Destroy(this.gameObject);
        }
    }

    void Disperse()
    {

        Vector3 direction = (positions[myIndex] - transform.position).normalized;

        GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(positions[myIndex], transform.position) < GetComponentInChildren<MeshFilter>().mesh.bounds.extents.magnitude)
        {
            StartCoroutine(GetComponent<Collectable>().ReactivateCollider());
        }
       
    }

    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<SphereCollider>().enabled = true;
        haveToDisperse = false;
        yield return null;
    }

    // if index is need to add a little bit more random
    // should not use the same index twice
    public void Dispersion(int index, int numToDrop)
    {
        GetComponent<SphereCollider>().enabled = false;
        myIndex = index;
        haveToDisperse = true;
    }
}
