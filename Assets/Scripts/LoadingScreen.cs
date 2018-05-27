using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour {

    private void Start()
    {
        StartCoroutine(LevelLoader.LoadLevelAsync(LevelLoader.TargetLevelId, true));
    }
}
