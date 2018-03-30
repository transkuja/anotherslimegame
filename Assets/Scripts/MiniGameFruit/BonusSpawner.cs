using System.Collections;
using UnityEngine;

public class BonusSpawner : MonoBehaviour {

	public enum BonusType { ChangeFruit, Aspirator };

    public GameObject aspirator;
    public GameObject fruitChange;

    [SerializeField]
    public float changeFruitSpawnDelay = 15.0f;

    [SerializeField]
    public float aspiratorFruitSpawnDelay = 25.0f;

    public bool canChange;
    public bool canAspirate;

    public BoxCollider boxColliderSpawn;
    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

    public Player playerTest;
    public Material matClementine;
    public Material matPomme;
    public Material matKiwi;
    public Material matFraise;


    /*void Start()
    {
        StartCoroutine(SpawnBonus(BonusType.ChangeFruit, changeFruitSpawnDelay));
		StartCoroutine(SpawnBonus(BonusType.Aspirator, aspiratorFruitSpawnDelay));
    }*/

	public IEnumerator SpawnBonus(BonusType _type, float _time)
	{
		while (true)
        {
            yield return new WaitForSeconds(_time);

            minX = boxColliderSpawn.transform.position.x -(boxColliderSpawn.transform.localScale.x / 2);
            maxX = boxColliderSpawn.transform.position.x + boxColliderSpawn.transform.localScale.x / 2;

            minZ = boxColliderSpawn.transform.position.z - (boxColliderSpawn.transform.localScale.z / 2);
            maxZ = boxColliderSpawn.transform.position.z + boxColliderSpawn.transform.localScale.z / 2;

            GameObject toInstantiate = GetBonusPrefabByType(_type);

            if (toInstantiate == null)
                Debug.Log("There's no prefab for the type " + _type + " , can't instantiate Bonus.");
            else
            {
                toInstantiate = Instantiate(toInstantiate, new Vector3(Random.Range(minX, maxX), 15, Random.Range(minZ, maxZ)), Quaternion.identity, transform);
                toInstantiate.GetComponent<Rigidbody>().useGravity = true;
            }
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

        Fruit typeToChange = playerTest.associateFruit;
        FruitType[] tabTest = GameObject.Find("FruitSpawner").GetComponent<FruitsSpawner>().GetComponentsInChildren<FruitType>();
        Fruit[] typeToSave = new Fruit[tabTest.Length];
        for (int i = 0; i < tabTest.Length; i++)
        {
            typeToSave[i] = tabTest[i].typeFruit;
            tabTest[i].typeFruit = typeToChange;
            if(tabTest[i].typeFruit == Fruit.Clementine)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material = matClementine;
            }
            else if (tabTest[i].typeFruit == Fruit.Pomme)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material = matPomme;
            }
            else if (tabTest[i].typeFruit == Fruit.Kiwi)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material = matKiwi;
            }
            else if (tabTest[i].typeFruit == Fruit.Fraise)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material = matFraise;
            }
        }
        yield return new WaitForSeconds(2.0f);
        for(int j = 0; j < tabTest.Length; j++)
        {
            tabTest[j].typeFruit = typeToSave[j];
            if (tabTest[j].typeFruit == Fruit.Clementine)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material = matClementine;
            }
            if (tabTest[j].typeFruit == Fruit.Pomme)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material = matPomme;
            }
            if (tabTest[j].typeFruit == Fruit.Kiwi)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material = matKiwi;
            }
            if (tabTest[j].typeFruit == Fruit.Fraise)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material = matFraise;
            }
        }
    }
    
    public void AspireFruit()
    {
        Fruit typeFruitPlayer = playerTest.associateFruit;
        FruitType[] tabTestAspirate = GameObject.Find("FruitSpawner").GetComponent<FruitsSpawner>().GetComponentsInChildren<FruitType>();
        foreach(FruitType type in tabTestAspirate)
        {
            if(typeFruitPlayer == type.typeFruit)
            {
                type.GetComponent<Collectable>().PickUp(playerTest);
            }
        }
    }
}
