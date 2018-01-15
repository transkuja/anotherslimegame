using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFade : MonoBehaviour {
    Material m;
    [SerializeField]
    float speed = 0.5f;
	// Use this for initialization
	void Start () {
        m = GetComponentInChildren<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        float h, s, v;

        Color.RGBToHSV(m.color, out h, out s, out v);

        h += Time.deltaTime * speed;

        m.color = Color.HSVToRGB(h, s, v);
	}
}
