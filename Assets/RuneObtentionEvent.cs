using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneObtentionEvent : MonoBehaviour {

    public void HideRune()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Invoke("StopParticles", 3.0f);
    }

    void StopParticles()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void RuneObtainedFx()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }

}
