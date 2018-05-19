using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardFloor : MonoBehaviour {

    [SerializeField]
    GameObject warningFeedback;

    public void WarnPlayerSmthgBadIsComing()
    {
        warningFeedback.SetActive(true);
    }

    public int GetFloorIndex()
    {
        return transform.GetSiblingIndex() + transform.parent.GetSiblingIndex() * 8;
    }

}
