﻿using System.Collections;
using System.Collections.Generic;
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

    public void Start()
    {
        if( type == CollectableType.Rune)
        {
            if(GetComponent<CreateRuneEnumFromDatabase>() == null)
            {
                Debug.LogError("Start :It's a rune, it need a createEnumFromDatabase component link to the associated rune");
                return;
            }
            string s = GetComponent<CreateRuneEnumFromDatabase>().HideString;
            if (DatabaseManager.Db.IsUnlock<DatabaseClass.RuneData>(s))
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
            if (!Utils.IsAnEvolutionCollectable(GetComponent<Collectable>().type))
            {
                if (player.Collectables[(int)GetComponent<Collectable>().type] < Utils.GetMaxValueForCollectable(GetComponent<Collectable>().type))
                {
                    
                    IsAttracted = true;
                    playerTarget = player;
                    return;
                }
            }
            else if(player.activeEvolutions == 0)
            {
                IsAttracted = true;
                playerTarget = player;
            }

        }
    }

    public void Attract()
    {
        Vector3 direction = (playerTarget.transform.position - transform.position).normalized;

        GetComponent<Rigidbody>().MovePosition(transform.position + direction * movementSpeed * Time.deltaTime);
        if (Vector3.Distance(playerTarget.transform.position, transform.position) < GetComponent<BoxCollider>().bounds.extents.magnitude)
        {
            playerTarget.UpdateCollectableValue(type, (int)Value);

            if (type == CollectableType.Rune)
            {
                if (GetComponent<CreateRuneEnumFromDatabase>() == null)
                {
                    Debug.LogError("Attract fct : It's a rune, it need a createEnumFromDatabase component link to the associated rune");
                    return;
                }
                string s = GetComponent<CreateRuneEnumFromDatabase>().HideString;
                DatabaseManager.Db.SetUnlock<DatabaseClass.RuneData>(s, true);
            }

            if (AudioManager.Instance != null && AudioManager.Instance.coinFX != null) AudioManager.Instance.PlayOneShot(AudioManager.Instance.coinFX);

            if (GetComponent<PoolChild>())
            {
                GetComponent<PoolChild>().ReturnToPool();
            }
            else
            {
                Destroy(this.gameObject);
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
