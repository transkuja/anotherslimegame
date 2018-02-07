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

    [SerializeField]
    private Fruit type;

    public bool canChange;
    public bool canAspirate;

    void Start()
    {
		StartCoroutine(SpawnBonus(BonusType.ChangeFruit, changeFruitSpawnDelay));
		StartCoroutine(SpawnBonus(BonusType.Aspirator, aspiratorFruitSpawnDelay));
        //type = GetComponentInParent<FruitsSpawner>().GetComponentInChildren<FruitType>().typeFruit;
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

    public void Update()
    {
        if(canChange)
        {
            ChangerFruit();
            canChange = false;
        }

        if(canAspirate)
        {
            AspireFruit();
            canAspirate = false;
        }

    }

    public void ChangerFruit()
    {
        Fruit typeToChange = GetComponentInParent<Player>().associateFruit;
        Transform[] tabTest = GameObject.Find("Pool Container 0").GetComponentsInChildren<Transform>();
        foreach (Transform transformFruit in tabTest)
        {
            GetComponent<FruitType>().typeFruit = typeToChange;
        }
    }

    public void AspireFruit()
    {
        //Checker tout les fruits de la pool
        /*foreach (Fruit fruit in PoolManager.poolFruit)
        {
            //Si le type correspond a celui du joueur, on le recupere
            //if(type == GetComponentInParent<Player>().associateFruit)
            {
                Appeler Attract() uniquement pour les fruits qui sont du type ?
                Remove tout les fruits du type de la scene et attribuer les points ?
            }

        }*/

    }
}
