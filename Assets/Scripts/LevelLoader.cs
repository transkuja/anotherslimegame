using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class LevelLoader {

    static AsyncOperation currentOperation = null;
    static string targetLevelName;
    static bool isLoading;

    public static string TargetLevelId
    {
        get
        {
            return targetLevelName;
        }
    }

    public static bool IsLoading
    {
        get
        {
            return isLoading;
        }
    }

    public static void LoadLevelWithFadeOut(string levelName, float fadeTime = 0.2f)
    {
        targetLevelName = levelName;
        isLoading = true;
        GameManager.Instance.FadeManager.FadeOut(fadeTime, LoadTargetLevel);
    }

    static void LoadTargetLevel()
    {
        isLoading = false;
        SceneManager.LoadScene(targetLevelName);
    }

    public static void LoadLevelWithLoadingScreen(string levelName, bool fadeOut = true, float fadeTime = 0.2f)
    {
        targetLevelName = levelName;
        isLoading = true;
        if (fadeOut)
            GameManager.Instance.FadeManager.FadeOut(fadeTime, LoadLoadingScreen);
        else
            LoadLoadingScreen();
    }

    static void LoadLoadingScreen()
    {
        isLoading = false;
        SceneManager.LoadScene("LoadingScene");
    }


    public static IEnumerator LoadLevelAsync(string levelName, bool fadeOut = false, float fadeTime = 0.2f)
    {
        isLoading = true;
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
        isLoading = false;
    }

}
