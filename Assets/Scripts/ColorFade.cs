using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFade : MonoBehaviour {
    List<Material> mats;
    [SerializeField]
    float speed = 0.5f;

    [SerializeField]
    bool randomSpeed = true;

    [SerializeField]
    bool isColorRelativeToPositionOnStart = false;
    [SerializeField]
    float positionRelativityMultiplier = 0.003f;
    // Use this for initialization
    void Start()
    {
        if (randomSpeed)
            speed = Random.Range(0.2f, 0.8f);
        Renderer[] mr = GetComponentsInChildren<Renderer>();
        mats = new List<Material>();
        for (int i = 0; i < mr.Length; i++)
        {
            Material[] ms = mr[i].materials;
            for (int j = 0; j < ms.Length; j++)
            {
                mats.Add(ms[j]);
                mats[mats.Count - 1].color = Color.red;
                if(isColorRelativeToPositionOnStart && GetComponent<MeshFilter>() != null)
                {
                    float h, s, v;
                    Color.RGBToHSV(mats[mats.Count - 1].color, out h, out s, out v);
                    h += GetComponent<MeshFilter>().sharedMesh.bounds.center.magnitude * positionRelativityMultiplier;
                    mats[mats.Count - 1].color = Color.HSVToRGB(h, s, v);
                }
                
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
