using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOn : MonoBehaviour {
    public bool isSwitchOn = false;

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            isSwitchOn = true;
        }
    }
}
