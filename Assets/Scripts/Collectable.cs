using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    CollectableType type;
    private float value;
    bool isAttracted = false;
    bool haveToDisperse = false;

    [SerializeField]
    public bool needInitialisation = true;

    private Vector3[] positionsIntermediaire;
    private Vector3[] positions;
    private int myIndex = 0;
    private bool positionIntermediaireAtteintes = false; 

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

    public void Init(int numPiecesToDrop)
    {
        Value = Utils.GetDefaultCollectableValue((int)type);
        if (numPiecesToDrop > 0)
        {
            positionsIntermediaire = SpawnManager.GetVector3ArrayOnADividedCircle(transform.position + 3 * Vector3.up, 3, numPiecesToDrop, SpawnManager.Axis.XZ);
            positions = SpawnManager.GetVector3ArrayOnADividedCircle(transform.position, 6, numPiecesToDrop, SpawnManager.Axis.XZ);
    
        }
        needInitialisation = false;
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
        if (needInitialisation)
            Init(0);
        if (!haveToDisperse && isAttracted)
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
        // Parabola homemade
        // Don't handle collisions
        if (!positionIntermediaireAtteintes)
        {
            Vector3 direction = (positionsIntermediaire[myIndex] - transform.position).normalized;
            GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed / 4 * Time.deltaTime);
            if (Vector3.Distance(positionsIntermediaire[myIndex], transform.position) < GetComponentInChildren<MeshFilter>().mesh.bounds.extents.magnitude)
            {
                positionIntermediaireAtteintes = true;
            }
        }
        else
        {
            Vector3 direction = (positions[myIndex] - transform.position).normalized;

            GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed / 4 * Time.deltaTime);
            if (Vector3.Distance(positions[myIndex], transform.position) < GetComponentInChildren<MeshFilter>().mesh.bounds.extents.magnitude)
            {
                StartCoroutine(GetComponent<Collectable>().ReactivateCollider());
            }
        }
       
    }

    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<SphereCollider>().enabled = true;
        haveToDisperse = false;
        positionIntermediaireAtteintes = false;
        yield return null;
    }

    // if index is need to add a little bit more random
    // should not use the same index twice
    public void Dispersion(int index)
    {
        GetComponent<SphereCollider>().enabled = false;
        myIndex = index;
        haveToDisperse = true;
    }
}
