using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingTile : MonoBehaviour {
    [SerializeField]
    float timeUp = 3.0f;

    [SerializeField]
    float timeDown = 6.0f;

    [SerializeField]
    float offsetTime = 0.0f;

    [SerializeField]
    AnimationCurve riseCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField]
    AnimationCurve downCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

    [SerializeField]
    float maxHeight = 8.0f;

    [SerializeField]
    float moveDuration = 1.0f;

    [SerializeField]
    bool useLocalUp = true;

    float timer = 0.0f;
    Vector3 startPos;
    Vector3 axis;

    bool isUp = false;

	void Start () {
        startPos = transform.position;
        timer = -offsetTime;
        if (useLocalUp)
            axis = transform.up;
        else
            axis = Vector3.up;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if((isUp && timer >= timeUp) || (!isUp && timer >= timeDown))
        {
            if (!isUp)
            {
                StopCoroutine(GoDown());
                StartCoroutine(Rise());
            }
            else
            {
                StopCoroutine(Rise());
                StartCoroutine(GoDown());            
            }

            timer = 0.0f;
        }
	}

    IEnumerator Rise()
    {
        float riseTimer = 0.0f;

        if (moveDuration == 0.0f)
            moveDuration = 0.01f;
        while(riseTimer < moveDuration)
        {
            yield return null;
            riseTimer += Time.deltaTime;
            transform.position = startPos + axis * riseCurve.Evaluate(riseTimer / moveDuration) * maxHeight;
        }
        isUp = true;
        timer = 0.0f;
    }

    IEnumerator GoDown()
    {
        float downTimer = 0.0f;
        if (moveDuration == 0.0f)
            moveDuration = 0.01f;
        while (downTimer < moveDuration)
        {
            yield return null;
            downTimer += Time.deltaTime;
            transform.position = startPos + axis * downCurve.Evaluate(downTimer / moveDuration) * maxHeight;
        }
        isUp = false;
        timer = 0.0f;
    }
}
