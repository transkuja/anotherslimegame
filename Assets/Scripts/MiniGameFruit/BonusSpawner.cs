using System.Collections;
using UnityEngine;

public class BonusSpawner : MonoBehaviour {

	public enum BonusType { ChangeFruit, Aspirator, FruitBonus };

    public GameObject aspirator;
    public GameObject fruitChange;
    public GameObject fruitBonus;

    [SerializeField]
    public float changeFruitSpawnDelay = 15.0f;

    [SerializeField]
    public float aspiratorFruitSpawnDelay = 25.0f;

    [SerializeField]
    public float fruitBonusSpawnDelay = 17.5f;

    public bool canChange;

    public MeshCollider boxColliderSpawn;
    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

    public Player playerTest;
    public Material matClementine;
    public Material matPomme;
    public Material matKiwi;
    public Material matFraise;


    void Start()
    {
        minX = -(boxColliderSpawn.transform.localScale.x / 2);
        maxX = boxColliderSpawn.transform.localScale.x / 2;

        minZ = -(boxColliderSpawn.transform.localScale.z / 2);
        maxZ = boxColliderSpawn.transform.localScale.z / 2;
    }

    public IEnumerator SpawnBonus(BonusType _type, float _time)
	{
        while (!GameManager.Instance.isTimeOver)
        {
            yield return new WaitForSeconds(_time);

            GameObject toInstantiate = GetBonusPrefabByType(_type);

            if (toInstantiate == null)
                Debug.Log("There's no prefab for the type " + _type + " , can't instantiate Bonus.");
            else if (toInstantiate == fruitChange)
            {
                toInstantiate = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.BonusFruit).GetItem(transform, new Vector3(Random.Range(minX, maxX), 18.15f, Random.Range(minZ, maxZ)), Quaternion.identity, true, false, 0);

                toInstantiate.GetComponent<Rigidbody>().useGravity = true;

                StartCoroutine(UnspawnBonus(toInstantiate));
            }
            else if (toInstantiate == aspirator)
            {
                toInstantiate = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.BonusFruit).GetItem(transform, new Vector3(Random.Range(minX, maxX), 18.15f, Random.Range(minZ, maxZ)), Quaternion.identity, true, false, 1);

                toInstantiate.GetComponent<Rigidbody>().useGravity = true;

                StartCoroutine(UnspawnBonus(toInstantiate));
            }
            else if(toInstantiate)
            {
                toInstantiate = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.BonusFruit).GetItem(transform, new Vector3(Random.Range(minX, maxX), 18.15f, Random.Range(minZ, maxZ)), Quaternion.identity, true, false, 2);
                toInstantiate.GetComponent<Rigidbody>().useGravity = true;

                //TO DO : Mettre le type du fruit random entre les 4 possible (Et l'indiquer avec un material ?) Ou bien rien et le premier qui le prend a les points (donc pas de type précis, il est multi-type)
                toInstantiate.GetComponent<FruitType>().typeFruit = (Fruit)Random.Range(0, (int)GameManager.Instance.PlayerStart.ActivePlayersAtStart);
                if(toInstantiate.GetComponent<FruitType>().typeFruit == Fruit.Clementine)
                {
                    toInstantiate.GetComponent<Renderer>().material = matClementine;
                }
                else if(toInstantiate.GetComponent<FruitType>().typeFruit == Fruit.Pomme)
                {
                    toInstantiate.GetComponent<Renderer>().material = matPomme;
                }
                else if(toInstantiate.GetComponent<FruitType>().typeFruit == Fruit.Kiwi)
                {
                    toInstantiate.GetComponent<Renderer>().material = matKiwi;
                }
                else if(toInstantiate.GetComponent<FruitType>().typeFruit == Fruit.Fraise)
                {
                    toInstantiate.GetComponent<Renderer>().material = matFraise;
                }

                StartCoroutine(UnspawnBonus(toInstantiate));
            }
        }
	}
	
    GameObject GetBonusPrefabByType(BonusType _type)
    {
        if (_type == BonusType.ChangeFruit)
            return fruitChange;
        if (_type == BonusType.Aspirator)
            return aspirator;
        if (_type == BonusType.FruitBonus)
            return fruitBonus;

        return null;
    }

    IEnumerator UnspawnBonus(GameObject go)
    {
        yield return new WaitForSeconds(5.0f);
        go.GetComponentInChildren<Projector>().gameObject.SetActive(false);

        go.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(0.2f);

        Destroy(go);
    }

    public void Update()
    {
        if(canChange)
        {
            StartCoroutine(ChangerFruit());
            canChange = false;
        }
    }

    public IEnumerator ChangerFruit()
    {
        Fruit typeToChange = (Fruit)(int)playerTest.GetComponent<PlayerController>().PlayerIndex;
        FruitType[] tabTest = GameObject.Find("FruitSpawner").GetComponent<FruitsSpawner>().GetComponentsInChildren<FruitType>();
        Fruit[] typeToSave = new Fruit[tabTest.Length];
        for (int i = 0; i < tabTest.Length; i++)
        {
            typeToSave[i] = tabTest[i].typeFruit;
            tabTest[i].typeFruit = typeToChange;
            if (tabTest[i].typeFruit == Fruit.Clementine)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material.color = matClementine.color;
            }
            else if (tabTest[i].typeFruit == Fruit.Pomme)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material.color = matPomme.color;
            }
            else if (tabTest[i].typeFruit == Fruit.Kiwi)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material.color = matKiwi.color;
            }
            else if (tabTest[i].typeFruit == Fruit.Fraise)
            {
                tabTest[i].gameObject.GetComponent<Renderer>().material.color = matFraise.color;
            }
        }
        yield return new WaitForSeconds(2.0f);
        for (int j = 0; j < tabTest.Length; j++)
        {
            tabTest[j].typeFruit = typeToSave[j];
            if (tabTest[j].typeFruit == Fruit.Clementine)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material.color = matClementine.color;
            }
            if (tabTest[j].typeFruit == Fruit.Pomme)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material.color = matPomme.color;
            }
            if (tabTest[j].typeFruit == Fruit.Kiwi)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material.color = matKiwi.color;
            }
            if (tabTest[j].typeFruit == Fruit.Fraise)
            {
                tabTest[j].gameObject.GetComponent<Renderer>().material.color = matFraise.color;
            }
        }
    }
    
    public void AspireFruit()
    {
        Fruit typeFruitPlayer = (Fruit)(int)playerTest.GetComponent<PlayerController>().PlayerIndex;
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
