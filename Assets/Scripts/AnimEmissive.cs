using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEmissive : MonoBehaviour {

    Material material;
    public float delay = 5.0f;
    public float offsetDelay;
    float time;

    IEnumerator Start () {
        material = GetComponent<MeshRenderer>().material;
        material.EnableKeyword("_EMISSION");
        time = 0;
        Color baseColor = material.GetColor("_EmissionColor");
        yield return new WaitForSeconds(offsetDelay);

        while (true)
        {
            yield return new WaitForSeconds(delay);
            Color colorToApply = baseColor;

            while (colorToApply.maxColorComponent < 2.0f)
            {
                time += Time.deltaTime*0.25f;
                colorToApply *= (1 + time);
                material.SetColor("_EmissionColor", colorToApply);
                yield return new WaitForEndOfFrame();
            }

            time = 0.0f;
            while (colorToApply.maxColorComponent > baseColor.r)
            {
                time += Time.deltaTime * 0.25f;
                colorToApply *= 1 / (1 + time);
                material.SetColor("_EmissionColor", colorToApply);
                yield return new WaitForEndOfFrame();
            }
        }
    }

}
