using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    public CollectableType type;
    public string idRune;
    public bool needInitialisation = true;
    public bool haveToDisperse = false;

    private uint movementSpeed = 15;
    private int value = 5;

    // Index in database to know if it has already been broken. -1 if no persistence.
    public int persistenceIndex = -1;

    private bool isAttracted = false;
    Player playerTarget;

    public int Value
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

    public void Start()
    {
        if( type == CollectableType.Rune)
        {
            if (DatabaseManager.Db.IsUnlock<DatabaseClass.RuneData>(idRune))
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }

    private void OnEnable()
    {
        if (GetComponent<PoolChild>())
        {
            if (GetComponent<FruitType>())
            {
                haveToDisperse = false;
            }
            else
            {
                haveToDisperse = true;
            }
            isAttracted = false;
            if (GetComponent<Collider>())
                GetComponent<Collider>().enabled = true;
            playerTarget = null;
            if (GetComponent<Animator>())
                GetComponent<Animator>().enabled = true;
            if (GetComponent<Rigidbody>())
                GetComponent<Rigidbody>().WakeUp();
        }
    }

    public void Init()
    {
        Value = Utils.GetDefaultCollectableValue((int)type);
        needInitialisation = false;
    }

    public void Disperse(int index)
    {
        haveToDisperse = true;
        Value = Utils.GetDefaultCollectableValue((int)type);
        Vector3 dir = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f)).normalized;
        GetComponent<Rigidbody>().AddForce(dir*Random.Range(7.5f, 12.0f), ForceMode.Impulse);
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
            if (!Utils.IsAnEvolutionCollectable(GetComponent<Collectable>().type) || player.activeEvolutions <= 0.8f)
            {
                IsAttracted = true;
                if(GetComponent<Animator>())
                    GetComponent<Animator>().enabled = false;
                if (GetComponent<Rigidbody>())
                    GetComponent<Rigidbody>().Sleep();
                // En theorie sa sert a rien
                GetComponent<Collider>().enabled = false;
                playerTarget = player;
                return;
            }
        }
    }

    public void Attract()
    {
        transform.position = Vector3.Lerp(transform.position, (playerTarget.transform.position + Vector3.up * .5f), Time.deltaTime * movementSpeed);
        //GetComponent<Rigidbody>().velocity = (direction * movementSpeed * Time.deltaTime * 50.0f);
        //Debug.Log(Vector3.Distance(playerTarget.transform.position + Vector3.up * 0.5f, transform.position));
        if (Vector3.Distance(playerTarget.transform.position + Vector3.up * 0.5f, transform.position) <= 1.0f)
        {
            //Debug.Log("Entree");
            // Won't play the sound fx and wont update player money
            if (!(playerTarget.GetComponent<EnnemyController>() || playerTarget.GetComponent<PNJController>()))
            {
                if (GetComponent<FruitType>())
                {
                    //Debug.Log("EntreeFruit");
                    if (name == "fruitChanger(Clone)")
                    {
                        GetComponentInParent<Transform>().GetComponentInParent<BonusSpawner>().canChange = true;
                        GetComponentInParent<Transform>().GetComponentInParent<BonusSpawner>().playerTest = playerTarget;
                    }
                    if (name == "Aspirator(Clone)")
                    {
                        GetComponentInParent<Transform>().GetComponentInParent<BonusSpawner>().playerTest = playerTarget;
                        GetComponentInParent<Transform>().GetComponentInParent<BonusSpawner>().AspireFruit();

                    }
                    if (name == "FruitBonuss(Clone)")
                    {
                        if ((int)playerTarget.GetComponent<PlayerController>().PlayerIndex == (int)GetComponent<FruitType>().typeFruit)
                        {
                            playerTarget.UpdateCollectableValue(type, value + 1 * 18);
                        }
                    }
                    if ((int)playerTarget.GetComponent<PlayerController>().PlayerIndex == (int)GetComponent<FruitType>().typeFruit)
                    {
                        playerTarget.UpdateCollectableValue(type, value * (6 - (int)GetComponent<FruitType>().state));
                    }
                    else
                    {
                        playerTarget.UpdateCollectableValue(type, -value * 2);
                    }
                }
                else
                {
                    // FIx rune tmp rémi
                    if (type == CollectableType.Rune)
                    {
                        DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(idRune, true);
                    }
                    playerTarget.UpdateCollectableValue(type, value);
                }

                if (AudioManager.Instance != null && AudioManager.Instance.coinFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.coinFX);
            }

            if (GetComponent<PoolChild>())
            {
                GetComponent<PoolChild>().ReturnToPool();
            }
            else
            {
                if (persistenceIndex != -1 && !DatabaseManager.Db.alreadyCollectedCollectables[persistenceIndex])
                {
                    DatabaseManager.Db.alreadyCollectedCollectables[persistenceIndex] = true;
                    Destroy(this.gameObject);
                }
            }
        }
    }

    public IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(1.0f);
        haveToDisperse = false;
        yield return null;
    }
}
