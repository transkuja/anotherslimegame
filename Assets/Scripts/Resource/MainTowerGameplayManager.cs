using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTowerGameplayManager : MonoBehaviour {

    private Transform referenceTransform;

    public List<GameObject> prefabsGameplayPlateformsExterior;
    public List<GameObject> prefabsGameplayPlateformsInterior;

    public int initialHeight = 30;
    public int heightStep = 25;
    public int necessaryFloorsExt = 4;
    public int necessaryFloorsInt = 2;
    
    private void Start()
    {
        referenceTransform = HUBManager.instance.referenceTransform;
        if (referenceTransform == null)
        {
            Debug.LogWarning("No reference transform specified. Won't spawn anything.");
            return;
        }

        if (prefabsGameplayPlateformsExterior == null || prefabsGameplayPlateformsExterior.Count == 0)
        {
            Debug.LogWarning("No prefabs in list for gameplay outside the main tower.");
        }
        else
        {
            for (int i = 0; i < necessaryFloorsExt; i++)
            {
                if (i == 2) i++;
                GameObject go;
                if (i < prefabsGameplayPlateformsExterior.Count)
                    go = Instantiate(prefabsGameplayPlateformsExterior[i], referenceTransform);
                else
                    go = Instantiate(prefabsGameplayPlateformsExterior[Random.Range(0, prefabsGameplayPlateformsExterior.Count)], referenceTransform);

                go.transform.localPosition = new Vector3(0, initialHeight + i * heightStep, 0);
            }
        }

        if (prefabsGameplayPlateformsInterior == null || prefabsGameplayPlateformsInterior.Count == 0)
        {
            Debug.LogWarning("No prefabs in list for gameplay inside the main tower.");
        }
        else
        {
            for (int i = 0; i < prefabsGameplayPlateformsInterior.Count; i++)
            {
                GameObject go = Instantiate(prefabsGameplayPlateformsInterior[i], referenceTransform);
                //go.transform.localPosition = new Vector3(0, initialHeight + i * heightStep, 0);
                //go.transform.localRotation = Quaternion.identity;
            }
        }
    }
}
