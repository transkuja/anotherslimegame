using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MiniGame { None, KickThemAll, Size }
// WARNING, will handle load scenes for now. REVAMP this later if we keep all in one scene.
public static class MinigameManager {

    public static string GetSceneNameFromMinigame(MiniGame _minigame)
    {
        if (_minigame == MiniGame.KickThemAll)
            return "SceneMinigamePush";
        return "";
    }

    public static bool IsAMiniGameScene()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        return
            activeSceneName == GetSceneNameFromMinigame(MiniGame.KickThemAll)
        // ||
        ;


    }
}
