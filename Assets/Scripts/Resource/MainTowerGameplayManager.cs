using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTowerGameplayManager : MonoBehaviour {

    [Tooltip("The transform from which all instantiates will be done. Instantiated platforms will be this transform's children.")]
    public Transform referenceTransform;

    public List<GameObject> prefabsGameplayPlateformsExterior;
    public List<GameObject> prefabsGameplayPlateformsInterior;

    public int initialHeight = 30;
    public int heightStep = 10;
    public int necessaryFloorsExt = 4;
    public int necessaryFloorsInt = 2;
    
    private void Start()
    {
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
                GameObject go = Instantiate(prefabsGameplayPlateformsExterior[Random.Range(0, prefabsGameplayPlateformsExterior.Count)], referenceTransform);
                go.transform.localPosition = new Vector3(0, initialHeight + i * heightStep, 0);
                go.transform.localRotation = Quaternion.identity;
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
