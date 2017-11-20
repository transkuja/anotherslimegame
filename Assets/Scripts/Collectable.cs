using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    public CollectableType type;
    public bool needInitialisation = true;

    private Vector3 direction;
    private int myIndex = 0;

    uint movementSpeed = 40;
    private float value;
    public bool haveToDisperse = false;

    bool isAttracted = false;
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

    public void Init(int jessaipa)
    {
        Value = Utils.GetDefaultCollectableValue((int)type);
        needInitialisation = false;
    }

    public void Disperse(int index, Vector3 direction)
    {
        haveToDisperse = true;
        GetComponentInChildren<SphereCollider>().enabled = false;
        myIndex = index;
        haveToDisperse = true;
        Value = Utils.GetDefaultCollectableValue((int)type);
        GetComponent<Rigidbody>().AddForce(direction*7.5f, ForceMode.Impulse);
        StartCoroutine(GetComponent<Collectable>().ReactivateCollider());
        needInitialisation = false;
    }

    private void FixedUpdate()
    {
        if (needInitialisation)
            Init(0);
        if (!haveToDisperse && isAttracted)
            Attract();
    }

    public void PickUp(Player player)
    {
        if (player && !isAttracted)
        {
            // Grab everything not linked to evolution (points)
            if (!Utils.IsAnEvolutionCollectable(GetComponent <Collectable>().type))
            {
                if (player.Collectables[(int)GetComponent<Collectable>().type] < Utils.GetMaxValueForCollectable(GetComponent<Collectable>().type))
                {
                    if (AudioManager.Instance != null && AudioManager.Instance.coinFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.coinFX);
                    isAttracted = true;
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

    public void Attract()
    {
        Vector3 direction = (playerTarget.transform.position - transform.position).normalized;

        GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < GetComponent<MeshFilter>().mesh.bounds.extents.magnitude)
        {
            playerTarget.UpdateCollectableValue(GetComponent<Collectable>().type, (int)GetComponent<Collectable>().Value);
            Destroy(this.gameObject);
        }
    }

    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(2.0f);
        GetComponentInChildren<SphereCollider>().enabled = true;
        haveToDisperse = false;
        yield return null;
    }
}
