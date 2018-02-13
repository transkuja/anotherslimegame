using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class ReplaceWithPrefab : MonoBehaviour {
    [SerializeField]
    GameObject murSimpleNormal;
    [SerializeField]
    GameObject murSimpleMossy;

    [SerializeField]
    GameObject murOrneNormal;
    [SerializeField]
    GameObject murOrneMossy;

    [SerializeField]
    GameObject dalleNormal;
    [SerializeField]
    GameObject dalleMossy;

    [SerializeField]
    GameObject colonneNormal;
    [SerializeField]
    GameObject colonneMossy;

    [SerializeField]
    GameObject ouvertureNormal;
    [SerializeField]
    GameObject ouvertureMossy;

    [SerializeField]
    GameObject parapetNormal;
    [SerializeField]
    GameObject parapetMossy;

    [SerializeField]
    bool replace = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(replace)
        {
            ReplaceEVERYTHING("MurOrne", murOrneNormal, murOrneMossy);
            ReplaceEVERYTHING("MurSimple", murSimpleNormal, murSimpleMossy);
            ReplaceEVERYTHING("Colonne", colonneNormal, colonneMossy);
            ReplaceEVERYTHING("Ouverture", ouvertureNormal, ouvertureMossy);
            ReplaceEVERYTHING("Parapet", parapetNormal, parapetMossy);
            ReplaceEVERYTHING("Dalle", dalleNormal, dalleMossy);
            ReplaceEVERYTHING("Pillier", colonneNormal, colonneMossy);
            ReplaceEVERYTHING("Pilier", colonneNormal, colonneMossy);
            ReplaceEVERYTHING("Haut", colonneNormal, colonneMossy, true);
            ReplaceEVERYTHING("Mid", colonneNormal, colonneMossy, true);
            ReplaceEVERYTHING("Bas", colonneNormal, colonneMossy, true);
            replace = false;
        }
	}

    void ReplaceEVERYTHING(string nameContains, GameObject normalPrefab, GameObject mossyPrefab, bool exactName = false)
    {
        GameObject[] gos = FindObjectsOfType<GameObject>();
        for(int i = 0; i < gos.Length; i++)
        {
            bool condition;

            if(!exactName)
            {
                condition = gos[i].name.Contains(nameContains);
            }
            else
            {
                condition = gos[i].name == nameContains;
            }

            if(condition)
            {
                GameObject newObject;
                if (gos[i].GetComponent<MeshRenderer>() && (gos[i].GetComponent<MeshRenderer>().sharedMaterial.name.Contains("Mossy")))
                {
                    newObject = PrefabUtility.InstantiatePrefab(mossyPrefab) as GameObject;
                }
                else
                    newObject = PrefabUtility.InstantiatePrefab(normalPrefab) as GameObject;
                newObject.transform.SetParent(gos[i].transform.parent);
                newObject.transform.SetSiblingIndex(gos[i].transform.GetSiblingIndex());
                newObject.transform.position = gos[i].transform.position;
                newObject.transform.rotation = gos[i].transform.rotation;
                newObject.transform.localScale = gos[i].transform.localScale;
                while (gos[i].transform.childCount > 0)
                {
                    gos[i].transform.GetChild(0).SetParent(newObject.transform);
                }
               DestroyImmediate(gos[i]);
            }
        }

    }
}

