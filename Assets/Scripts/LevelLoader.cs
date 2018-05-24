using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class LevelLoader {

    static AsyncOperation currentOperation = null;
    static string targetLevelName;

    public static string TargetLevelId
    {
        get
        {
            return targetLevelName;
        }
    }

    public static void LoadLevelWithFadeOut(string levelName, float fadeTime = 0.2f)
    {
        targetLevelName = levelName;
        GameManager.Instance.FadeManager.FadeOut(fadeTime, LoadTargetLevel);
    }

    static void LoadTargetLevel()
    {
        SceneManager.LoadScene(targetLevelName);
    }

    public static void LoadLevelWithLoadingScreen(string levelName, bool fadeOut = true, float fadeTime = 0.2f)
    {
        targetLevelName = levelName;
        if (fadeOut)
            GameManager.Instance.FadeManager.FadeOut(fadeTime, LoadLoadingScreen);
        else
            LoadLoadingScreen();
    }

    static void LoadLoadingScreen()
    {
        SceneManager.LoadScene("LoadingScene");
    }


    public static IEnumerator LoadLevelAsync(string levelName, bool fadeOut = false, float fadeTime = 0.2f)
    {
        currentOperation = SceneManager.LoadSceneAsync(levelName);
        currentOperation.allowSceneActivation = false;
        while (currentOperation.progress >= 0.9f)
        {
            yield return null;
        }
        if (fadeOut)
            GameManager.Instance.FadeManager.FadeOut(fadeTime, ActivateScene);
        else
            ActivateScene();
    }

    static void ActivateScene()
    {
        currentOperation.allowSceneActivation = true;
    }

}
