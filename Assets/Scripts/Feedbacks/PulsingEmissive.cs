using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingEmissive : MonoBehaviour {

    Material material;
    [SerializeField]
    bool useCurrentEmissiveColor;
    [SerializeField]
    Color initialColor;
    float time;
    [SerializeField]
    float pulseRangeMin = 0.4f;
    [SerializeField]
    float pulseRangeMax = 1.5f;
    [SerializeField]
    float pulseSpeed = 5;

    IEnumerator Start()
    {
        material = GetComponentInChildren<Renderer>().material;
        material.EnableKeyword("_EMISSION");

        if (useCurrentEmissiveColor)
            initialColor = material.GetColor("_EmissionColor");
        material.SetColor("_EmissionColor", initialColor);

        time = 0;

        while (true)
        {
            Color colorToApply = material.GetColor("_EmissionColor");

            while (colorToApply.maxColorComponent < initialColor.maxColorComponent * pulseRangeMax && pulseRangeMax > 0.0f)
            {
                time += Time.deltaTime * pulseSpeed * 0.01f;
                colorToApply *= (1 + time);
                material.SetColor("_EmissionColor", colorToApply);
                yield return new WaitForEndOfFrame();
            }

            time = 0.0f;
            while (colorToApply.maxColorComponent > initialColor.maxColorComponent * pulseRangeMin && pulseRangeMin > 0.0f)
            {
                time += Time.deltaTime * pulseSpeed * 0.01f;
                colorToApply *= 1 / (1 + time);
                material.SetColor("_EmissionColor", colorToApply);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
