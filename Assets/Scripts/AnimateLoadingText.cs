using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimateLoadingText : MonoBehaviour {
    [SerializeField]
    float interval = 1f;

    Text text;

	IEnumerator Start () {
        text = GetComponent<Text>();
		while(true)
        {
            text.text = "Loading .";
            yield return new WaitForSeconds(interval);
            text.text = "Loading ..";
            yield return new WaitForSeconds(interval);
            text.text = "Loading ...";
            yield return new WaitForSeconds(interval);
        }
	}

}
