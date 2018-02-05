using System.Collections;
using UnityEngine;

public class BonusSpawner : MonoBehaviour {

	enum BonusType { ChangeFruit, Aspirator };

    public GameObject aspirator;
    public GameObject fruitChange;

    [SerializeField]
    float changeFruitSpawnDelay = 15.0f;

    [SerializeField]
    float aspiratorFruitSpawnDelay = 25.0f;

    void Start()
    {
		StartCoroutine(SpawnBonus(BonusType.ChangeFruit, changeFruitSpawnDelay));
		StartCoroutine(SpawnBonus(BonusType.Aspirator, aspiratorFruitSpawnDelay));
    }

	IEnumerator SpawnBonus(BonusType _type, float _time)
	{
		while (true)
        {
            yield return new WaitForSeconds(_time);
            int randChild = Random.Range(0, transform.childCount);
            GameObject toInstantiate = GetBonusPrefabByType(_type);

            if (toInstantiate == null)
                Debug.Log("There's no prefab for the type " + _type + " , can't instantiate Bonus.");
            else
                Instantiate(toInstantiate, transform.GetChild(randChild).position + Vector3.up * 0.25f, Quaternion.identity, transform.GetChild(randChild));
		}
	}
	
    GameObject GetBonusPrefabByType(BonusType _type)
    {
        if (_type == BonusType.ChangeFruit)
            return fruitChange;
        if (_type == BonusType.Aspirator)
            return aspirator;

        return null;
    }


    public void changerFruit()
    {

    }

    public void AspireFruit()
    {

    }
}
