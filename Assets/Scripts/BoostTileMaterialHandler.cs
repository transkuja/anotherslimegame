using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTileMaterialHandler : MonoBehaviour {
    [SerializeField]
    float scrollSpeed = 10.0f;
    Material mat;
	// Use this for initialization
	void Start () {
        mat = GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        mat.SetTextureOffset("_DetailAlbedoMap", mat.GetTextureOffset("_DetailAlbedoMap") + Vector2.up * Time.deltaTime * scrollSpeed);
	}
}
