using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFadeInAndOut : MonoBehaviour {

    private float duration = 3f;

    private float timer;
    public bool halfwaythere = false;

	// Update is called once per frame
	void Update () {
        timer += Time.unscaledDeltaTime;
        if (timer > duration)
        {
            Destroy(this.gameObject);
        } else
        { if (timer > duration / 2) halfwaythere = true;
            GetComponent<Image>().color = new Color(0f, 0f, 0f, (Mathf.Sin(Mathf.PI * (timer/duration))));
        }
    }
}
