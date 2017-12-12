using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    public CollectableType type;
    public bool needInitialisation = true;

    private Vector3 direction;

    uint movementSpeed = 40;
    private float value;
    public bool haveToDisperse = false;

    bool isAttracted = false;
    Player playerTarget;

    public GameObject panneau;

    public bool hasBeenSpawned = false;
    public Player lastOwner;

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

    public bool IsAttracted
    {
        get
        {
            return isAttracted;
        }

        set
        {
            isAttracted = value;
        }
    }

    private void OnEnable()
    {
        if (GetComponent<PoolChild>())
        {
            haveToDisperse = true;
            isAttracted = false;
            playerTarget = null;
            GetComponent<Animator>().enabled = true;
        }
    }

    public void Init()
    {
        Value = Utils.GetDefaultCollectableValue((int)type);
        needInitialisation = false;
    }

    public void Disperse(int index, Vector3 direction)
    {
        haveToDisperse = true;
        Value = Utils.GetDefaultCollectableValue((int)type);
        GetComponent<Rigidbody>().AddForce(direction*7.5f, ForceMode.Impulse);
        StartCoroutine(ReactivateCollider());
        needInitialisation = false;
    }

    private void Update()
    {
        if (needInitialisation)
            Init();

        if (!haveToDisperse && IsAttracted)
            Attract();

    }

    public void PickUp(Player player)
    {
        if (player && !IsAttracted && !haveToDisperse)
        {
            // Grab everything not linked to evolution (points)
            if (!Utils.IsAnEvolutionCollectable(GetComponent <Collectable>().type))
            {
                if (player.Collectables[(int)GetComponent<Collectable>().type] < Utils.GetMaxValueForCollectable(GetComponent<Collectable>().type))
                {
                    
                    IsAttracted = true;
                    playerTarget = player;
                    return;
                }
            }
            else
            {
                if (GameManager.CurrentGameMode.evolutionMode != EvolutionMode.GrabEvolution)
                {
                    if (player.Collectables[(int)GetComponent<Collectable>().type] < Utils.GetMaxValueForCollectable(GetComponent<Collectable>().type))
                    {
                        IsAttracted = true;
                        playerTarget = player;
                    }
                }
                else
                {
                    if (player.activeEvolutions == 0)
                    {
                        if (panneau != null)
                            panneau.SetActive(false);
                        IsAttracted = true;
                        playerTarget = player;
                    }
                }
            }

        }
    }

    public void Attract()
    {
        Vector3 direction = (playerTarget.transform.position - transform.position).normalized;

        GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < GetComponent<BoxCollider>().bounds.extents.magnitude)
        {
            playerTarget.UpdateCollectableValue(GetComponent<Collectable>().type, (int)GetComponent<Collectable>().Value);

            if (AudioManager.Instance != null && AudioManager.Instance.coinFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.coinFX);

            if (GetComponent<PoolChild>())
            {
                GetComponent<PoolChild>().ReturnToPool();
            }
            else
            {
                if (GetComponent<Collectable>().type == CollectableType.Key)
                {
                    if (hasBeenSpawned)
                    {
                        int currentlyHoldByOwner = lastOwner.Collectables[(int)CollectableType.Key];

                        KeyReset keyData = lastOwner.KeysReset[currentlyHoldByOwner];                      
                        playerTarget.AddKeyInitialPosition(keyData);
                        lastOwner.KeysReset[currentlyHoldByOwner] = null;
                    }
                    else
                        playerTarget.AddKeyInitialPosition(transform, KeyFrom.Shelter);

                }

                Destroy(this.gameObject);
            }
        }
    }

    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(2.0f);
        haveToDisperse = false;
        yield return null;
    }
}
