using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ScreenshotSaver : EditorWindow
{
    Object selectedAsset;
    Camera screenshotCamera;
    bool useSceneCam = false;
    bool useGameCam = true;
    [MenuItem("Window/Screenshot Saver")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ScreenshotSaver));
    }

    void OnGUI()
    {

        GUILayout.Label("Asset Preview Image Saver", EditorStyles.boldLabel);
        selectedAsset = EditorGUILayout.ObjectField("Asset", selectedAsset, typeof(Object), false);
        if (selectedAsset == null)
            GUI.enabled = false;
        if (GUILayout.Button("Create Image"))
        {
            CreateAssetImage();
        }
        GUILayout.Space(10.0f);
        GUI.enabled = true;

        GUILayout.Label("Screenshot Saver", EditorStyles.boldLabel);

        if (useGameCam)
            GUI.enabled = false;
        useSceneCam = EditorGUILayout.Toggle("Use Scene Camera", useSceneCam);
        if (useSceneCam)
            GUI.enabled = false;
        if (useGameCam)
            GUI.enabled = true;
        useGameCam = EditorGUILayout.Toggle("Use Main Camera", useGameCam);
        if (useGameCam)
            GUI.enabled = false;
        screenshotCamera = EditorGUILayout.ObjectField("Use Another Camera", screenshotCamera, typeof(Camera), true) as Camera;

        GUI.enabled = true;

        if (GUILayout.Button("Take screenshot"))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string path = EditorUtility.SaveFilePanel("Save Screenshot", "Assets/", name, "png");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);

        if(useSceneCam)
        {
            EditorApplication.ExecuteMenuItem("Window/Scene");
            screenshotCamera = SceneView.lastActiveSceneView.camera;
        }

        if(useGameCam)
        {
            screenshotCamera = Camera.main;
        }
        
        RenderTexture rt = new RenderTexture(screenshotCamera.pixelWidth, screenshotCamera.pixelHeight, 0);

        screenshotCamera.targetTexture = rt;
        screenshotCamera.Render();
        RenderTexture.active = rt;

        Texture2D img = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        img.ReadPixels(new Rect(0, 0, screenshotCamera.pixelWidth, screenshotCamera.pixelHeight), 0, 0);

        RenderTexture.active = null;
        screenshotCamera.targetTexture = null;

        byte[] pngImg = img.EncodeToPNG();
        FileStream fs = File.Create(path);
        fs.Write(pngImg, 0, pngImg.Length);
        fs.Close();
        AssetDatabase.Refresh();

        if (useSceneCam || useGameCam)
            screenshotCamera = null;
    }

    void CreateAssetImage()
    {
        string path = EditorUtility.SaveFilePanel("Save Asset Preview Image", "Assets/", name, "png");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        path = FileUtil.GetProjectRelativePath(path);

        Texture2D img = AssetPreview.GetAssetPreview(selectedAsset);
        byte[] pngImg = img.EncodeToPNG();

        FileStream fs = File.Create(path);
        fs.Write(pngImg, 0, pngImg.Length);
        fs.Close();
        AssetDatabase.Refresh();
    }
}
