using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveMenuBackground : MonoBehaviour {

    RawImage rawImage;
    Rect rect;

    void Start () {
        rawImage = GetComponent<RawImage>();
    }
	
	void Update () {
        rect = rawImage.uvRect;
        rect.x -= Time.deltaTime / 25.0f;
        rect.y += Time.deltaTime / 25.0f;
        rawImage.uvRect = rect;
    }
}
