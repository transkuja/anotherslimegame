using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFade : MonoBehaviour {
    List<Material> mats;
    [SerializeField]
    float speed = 0.5f;
	// Use this for initialization
	void Start () {
        speed = Random.Range(0.2f, 0.8f);
        Renderer[] mr = GetComponentsInChildren<Renderer>();
        mats = new List<Material>();
        for(int i = 0; i < mr.Length; i++)
        {
            Material[] ms = mr[i].materials;
            for(int j = 0; j < ms.Length; j++)
            {
                mats.Add(ms[j]);
                mats[mats.Count-1].color = Color.red;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        float h, s, v;
        foreach (Material m in mats)
        {
            Color.RGBToHSV(m.color, out h, out s, out v);

            h += Time.deltaTime * speed;

            m.color = Color.HSVToRGB(h, s, v);
        }
	}
}
