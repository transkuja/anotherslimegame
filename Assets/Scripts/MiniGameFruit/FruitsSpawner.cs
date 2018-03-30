using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitsSpawner : MonoBehaviour {

    [SerializeField]
    float fruitsSpawnDelay = 0.5f;

    public BoxCollider boxColliderSpawn;
    public float minX;
    public float minZ;
    public float maxX;
    public float maxZ;

    public uint nbPlayer;

    public Material matClementine;
    public Material matPomme;
    public Material matKiwi;
    public Material matFraise;

    public void Start()
    {
        nbPlayer = GameManager.Instance.PlayerStart.ActivePlayersAtStart;
    }


    public IEnumerator Spawner()
    {
        if (GameManager.CurrentState == GameState.Normal)
        {
            while (true)
            {
                yield return new WaitForSeconds(fruitsSpawnDelay);

                minX = -(boxColliderSpawn.transform.localScale.x / 2);
                maxX = boxColliderSpawn.transform.localScale.x / 2;

                minZ = -(boxColliderSpawn.transform.localScale.z / 2);
                maxZ = boxColliderSpawn.transform.localScale.z / 2;

                GameObject go = ResourceUtils.Instance.poolManager.GetPoolByName(PoolName.Fruits).GetItem(transform, new Vector3(Random.Range(minX, maxX), 15, Random.Range(minZ, maxZ)), Quaternion.identity, true);
                go.GetComponent<BoxCollider>().enabled = true;
                if (GameManager.Instance.DataContainer != null)
                {
                    switch (nbPlayer)
                    {
                        case 1:
                            if (go.name == "Clementine(Clone)")
                            {
                                matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                                go.GetComponent<Renderer>().material = matClementine;
                            }
                            Debug.Log("coucou");
                            break;
                        case 2:
                            if (go.name == "Clementine(Clone)")
                            {
                                matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                                go.GetComponent<Renderer>().material = matClementine;
                            }
                            else if (go.name == "Pomme(Clone)")
                            {
                                matPomme.color = GameManager.Instance.DataContainer.selectedColors[1];
                                go.GetComponent<Renderer>().material = matPomme;
                            }
                            break;
                        case 3:
                            if (go.name == "Clementine(Clone)")
                            {
                                matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                                go.GetComponent<Renderer>().material = matClementine;
                            }
                            else if (go.name == "Pomme(Clone)")
                            {
                                matPomme.color = GameManager.Instance.DataContainer.selectedColors[1];
                                go.GetComponent<Renderer>().material = matPomme;
                            }
                            else if (go.name == "Kiwi(Clone)")
                            {
                                matKiwi.color = GameManager.Instance.DataContainer.selectedColors[2];
                                go.GetComponent<Renderer>().material = matKiwi;
                            }
                            break;
                        case 4:
                            if (go.name == "Clementine(Clone)")
                            {
                                matClementine.color = GameManager.Instance.DataContainer.selectedColors[0];
                                go.GetComponent<Renderer>().material = matClementine;
                            }
                            else if (go.name == "Pomme(Clone)")
                            {
                                matPomme.color = GameManager.Instance.DataContainer.selectedColors[1];
                                go.GetComponent<Renderer>().material = matPomme;
                            }
                            else if (go.name == "Kiwi(Clone)")
                            {
                                matKiwi.color = GameManager.Instance.DataContainer.selectedColors[2];
                                go.GetComponent<Renderer>().material = matKiwi;
                            }
                            else if (go.name == "Fraise(Clone)")
                            {
                                matFraise.color = GameManager.Instance.DataContainer.selectedColors[3];
                                go.GetComponent<Renderer>().material = matFraise;
                            }
                            break;
                        default:
                            break;
                    }
                }  
            }
        }
    }
}
