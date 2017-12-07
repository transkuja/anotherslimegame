using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {

    public List<GameObject> pieces;
    public int poolSize = 50;
    public List<GameObject> pool;

	// Use this for initialization
	void Start () {
        GameObject poolParent = new GameObject("BreakablePiecesPool");

        for (int i = 0; i < poolSize; i++)
        {
            GameObject piece = Instantiate(pieces[Random.Range(0, pieces.Count)], poolParent.transform);
            piece.SetActive(false);
            pool.Add(piece);
        }
	}

}
