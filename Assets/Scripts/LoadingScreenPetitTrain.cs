using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenPetitTrain : MonoBehaviour {
    Transform[] childrenTransforms;

    [SerializeField]
    float offsetPercentage = 8f;
    [SerializeField]
    float height = 5f;
    [SerializeField]
    float frequency = 0.04f;
    [SerializeField]
    float speed = 150f;

    float posTimer = 0f;

    float offset;
    float percentHeight;
    float startHeight;

	void Start () {
        childrenTransforms = new Transform[transform.childCount];
        offset = GetScreenWidthPercentage(offsetPercentage);
        percentHeight = GetScreenHeightPercentage(height);
        posTimer = (Screen.width / 2.0f);
        startHeight = transform.position.y;

        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            childrenTransforms[i] = transform.GetChild(i);
            childrenTransforms[i].position = new Vector3((Screen.width/ 2.0f) +  i * offset, (Mathf.Sin((- (i * offset)) * frequency) * percentHeight) + startHeight, 0f);
        }
    }

	void Update () {
        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            childrenTransforms[i].position = new Vector3(posTimer - (i*offset), (Mathf.Sin((posTimer - (i * offset)) * frequency) * percentHeight) + startHeight, 0f);
        }
        posTimer += Time.deltaTime * speed;
        
        if (posTimer > Screen.width + offset * transform.childCount)
            posTimer -= Screen.width + offset * 1.5f * transform.childCount;
        
	}

    float GetScreenWidthPercentage(float percents)
    {
        return (percents * (float)(Screen.width)) / 100f;
    }

    float GetScreenHeightPercentage(float percents)
    {
        return (percents * (float)(Screen.height)) / 100f;
    }
}
