using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamOfLight : MonoBehaviour {

    LineRenderer lineRenderer;
    float timer;
    [SerializeField] float maxWidth;
    [SerializeField] Ease.EASE_TYPE easeFunction;
    [SerializeField] float speed = 2;
    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }
    public void Start()
    {
    }
    public void Activate()
    {
        if (lineRenderer.enabled == false)
        {
            timer = 0;
            lineRenderer.enabled = true;
            StartCoroutine(UpdateBeamOfLight());
        }
    }
    public IEnumerator UpdateBeamOfLight()
    {
        //yield return new WaitForSeconds(maxCoolDown);
        bool end = false;
        float sign = 1;
        while (!end)
        {
            timer += Time.deltaTime* sign* speed;
            float width = Ease.Evaluate(easeFunction, timer) * maxWidth;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            if (timer > 1)
            {
                yield return new WaitForSeconds(0.5f);
                sign = -1;
            }
            if (timer < 0)
                end = true;
            yield return null;
        }
        lineRenderer.enabled = false;
        yield return null;
    }

}
