using System;
using System.Collections;
using UnityEngine;

public enum PlayerChildren { Evolutions };
public class Player : MonoBehaviour {

    Rigidbody rb;
    bool canDoubleJump = false;

    [Header("Collectables")]
    [SerializeField] int[] collectables;


    public Rigidbody Rb
    {
        get
        {
            return rb;
        }

        set
        {
            rb = value;
        }
    }

    public bool CanDoubleJump
    {
        get
        {
            return canDoubleJump;
        }

        set
        {
            canDoubleJump = value;
        }
    }

    public int[] Collectables
    {
        get
        {
            return collectables;
        }

        set
        {
            collectables = value;
        }
    }

    public void UpdateCollectableValue(CollectableType type, int pickedValue)
    {
        collectables[(int)type] = Mathf.Clamp(collectables[(int)type] + pickedValue, 0, Utils.GetMaxValueForCollectable(type));
        if (GameManager.GameplayType == 1)
        {
            Evolution evolution = GameManager.EvolutionManager.GetEvolutionByCollectableType(type);
            if (collectables[(int)type] >= evolution.Cost)
                EvolveGameplay1(evolution);
        }
    }

    // GAMEPLAY TEST 1: all of this should be in an Evolution class handling all evolution parameters (+ we should be able to pickup collectables and "refresh" an evolution indefinitely)
    private void EvolveGameplay1(Evolution evolution)
    {
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)evolution.BodyPart).gameObject.SetActive(true);
        StartCoroutine("Detransform", evolution);
    }

    public void EvolveGameplay2(Evolution evolution)
    {
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)evolution.BodyPart).gameObject.SetActive(true);
        collectables[0] -= evolution.Cost;
        StartCoroutine("Detransform2", evolution);
    }

    private IEnumerator Detransform2(Evolution evolution)
    {
        yield return new WaitForSeconds(evolution.Duration);
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild(evolution.Id).gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator Detransform(Evolution evolution)
    {
        yield return new WaitForSeconds(evolution.Duration);
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild(evolution.Id).gameObject.SetActive(false);
        collectables[(int)evolution.AssociatedCollectable] -= evolution.Cost;
        yield return null;
    }

    void Start () {
        rb = GetComponent<Rigidbody>();
        collectables = new int[(int)CollectableType.Size];
	}

}
