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
        if (GameManager.gameplayType == 1)
        {
            if (collectables[(int)type] == Utils.GetMaxValueForCollectable(type))
                EvolveGameplay1(type);
        }
    }

    // GAMEPLAY TEST 1: all of this should be in an Evolution class handling all evolution parameters (+ we should be able to pickup collectables and "refresh" an evolution indefinitely)
    private void EvolveGameplay1(CollectableType type)
    {
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)type).gameObject.SetActive(true); ;
        StartCoroutine("Detransform", type);
    }

    private IEnumerator Detransform(CollectableType type)
    {
        yield return new WaitForSeconds(5);
        transform.GetChild((int)PlayerChildren.Evolutions).GetChild((int)type).gameObject.SetActive(false);
        collectables[(int)type] = 0;
        yield return null;
    }

    void Start () {
        rb = GetComponent<Rigidbody>();
        collectables = new int[(int)CollectableType.Size];
	}

}
