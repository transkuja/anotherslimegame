using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControlleurEditorAsuppr : Editor
{
    public PlayerController playerController;
    private void OnEnable()
    {
        playerController = target as PlayerController;
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
