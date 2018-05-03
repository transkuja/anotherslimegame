using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFade : MonoBehaviour {
    List<Material> mats;
    [SerializeField]
    float speed = 0.2f;
    [SerializeField]
    bool randomSpeed;
    // Use this for initialization
    void Start () {
        if (randomSpeed)
            speed = Random.Range(0.2f, 0.8f);
        Renderer[] mr = GetComponentsInChildren<Renderer>();
        mats = new List<Material>();
        for(int i = 0; i < mr.Length; i++)
        {
            Material[] ms = mr[i].materials;
            for(int j = 0; j < ms.Length; j++)
            {
                mats.Add(ms[j]);
                mats[mats.Count-1].SetColor("_EmissionColor", Color.red);
                mats[mats.Count - 1].EnableKeyword("_EMISSION");
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        float h, s, v;
        foreach (Material m in mats)
        {
            Color.RGBToHSV(m.GetColor("_EmissionColor"), out h, out s, out v);

            h += Time.deltaTime * speed;

            m.SetColor("_EmissionColor",Color.HSVToRGB(h, s, v));
            m.color = Color.white;
        }
	}
}
