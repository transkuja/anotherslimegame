using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneObtentionEvent : MonoBehaviour {

    public void HideRune()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void RuneObtainedFx()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }

}
