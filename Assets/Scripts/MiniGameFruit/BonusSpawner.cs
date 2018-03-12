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

    public BoxCollider boxColliderSpawn;
    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

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

            minX = boxColliderSpawn.transform.position.x -(boxColliderSpawn.transform.localScale.x / 2);
            maxX = boxColliderSpawn.transform.position.x + boxColliderSpawn.transform.localScale.x / 2;

            minZ = boxColliderSpawn.transform.position.z - (boxColliderSpawn.transform.localScale.z / 2);
            maxZ = boxColliderSpawn.transform.position.z + boxColliderSpawn.transform.localScale.z / 2;

            //int randChild = Random.Range(0, transform.childCount);
            GameObject toInstantiate = GetBonusPrefabByType(_type);

            if (toInstantiate == null)
                Debug.Log("There's no prefab for the type " + _type + " , can't instantiate Bonus.");
            else
                Instantiate(toInstantiate, new Vector3(Random.Range(minX, maxX), 35, Random.Range(minZ, maxZ)), Quaternion.identity, transform);
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
            StartCoroutine(ChangerFruit());
            canChange = false;
        }

        if(canAspirate)
        {
            AspireFruit();
            canAspirate = false;
        }

    }

    public IEnumerator ChangerFruit()
    {
        Fruit typeToChange = GameObject.Find("Player(Clone)").GetComponent<Player>().associateFruit; //Voir a recuperer l'info sans passer par GameObject.Find()
        FruitType[] tabTest = GameObject.Find("FruitSpawner").GetComponent<FruitsSpawner>().GetComponentsInChildren<FruitType>();
        Fruit[] typeToSave = new Fruit[tabTest.Length];
        for (int i = 0; i < tabTest.Length; i++)
        {
            typeToSave[i] = tabTest[i].typeFruit;
            tabTest[i].typeFruit = typeToChange;
            //changer le material
        }
        yield return new WaitForSeconds(2.0f);
        for(int j = 0; j < tabTest.Length; j++)
        {
            for(int k = 0; k < typeToSave.Length; k++)
            {
                tabTest[j].typeFruit = typeToSave[k];
                //Rechanger le material
            }
        }
    }
    
    public void AspireFruit()
    {
        Fruit typeFruitPlayer = GameObject.Find("Player(Clone)").GetComponent<Player>().associateFruit; //Voir a recuperer l'info sans passer par GameObject.Find()
        FruitType[] tabTestAspirate = GameObject.Find("FruitSpawner").GetComponent<FruitsSpawner>().GetComponentsInChildren<FruitType>();
        foreach(FruitType type in tabTestAspirate)
        {
            if(typeFruitPlayer == type.typeFruit)
            {
                type.GetComponent<Collectable>().PickUp(GameObject.Find("Player(Clone)").GetComponent<Player>());
            }
        }

    }
}
