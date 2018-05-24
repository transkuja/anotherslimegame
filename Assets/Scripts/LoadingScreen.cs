using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

    private void OnLevelWasLoaded(int level)
    {
        GameManager.Instance.FadeManager.FadeIn(0.2f);
    }

    private void Start()
    {
        StartCoroutine(LevelLoader.LoadLevelAsync(LevelLoader.TargetLevelId, true));
    }
}
