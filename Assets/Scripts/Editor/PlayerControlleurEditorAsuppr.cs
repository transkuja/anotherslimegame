using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlayerControllerHub))]
public class PlayerControlleurEditorAsuppr : Editor
{
    public PlayerControllerHub playerController;
    private void OnEnable()
    {
        playerController = target as PlayerControllerHub;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Fix Stats"))
        {
            FixStat();
        }
        base.OnInspectorGUI();
    }
    public void FixStat()
    {
        if (playerController.stats == null)
        {
            playerController.stats = new Stats();
        }
        playerController.stats.Init(playerController);

    }
}
#endif