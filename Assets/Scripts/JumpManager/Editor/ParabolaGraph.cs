using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Super crade a refac

public class ParabolaGraph : EditorWindow
{
    // editorWindo/texture stuff : 
    static public ParabolaGraph window;
    static public RenderTexture rt;
    public static Material mat;
    static public int textSizeX = 1200;
    static public int textSizeY = 800;
    static int graduationSize = 50;

    static Jump jump;
    public static void CreateGraph(ParabolaInspector inspector, Jump jumpTarget)
    {
        if (window == null)
        {
            window = ScriptableObject.CreateInstance("ParabolaGraph") as ParabolaGraph;

            window.position = new Rect(0, 0, textSizeX, textSizeY);
            window.minSize = new Vector2(textSizeX, textSizeY + 100);

            jump = jumpTarget;
            DrawCharts(textSizeX, textSizeY);
        }
        window.ShowUtility();
    }


    void OnGUI()
    {
        if (GUILayout.Button("Clear"))
            DrawCharts(textSizeX, textSizeY);
        GUILayout.Label(rt);
    }
    void OnDestroy()
    {
    }
    // je me souviens plus à quoi la moitiée de ce code sert .
    // creation texture et dessiner la grille
    static public void DrawCharts(int width, int height)
    {
        if (rt == null || rt.width != width || rt.height != height)
        {
            if (rt != null)
                rt.Release();
            rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        }
        if (mat == null)
        {
            mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            mat.hideFlags = HideFlags.HideAndDontSave;
        }
        RenderTexture oldActiveRT = RenderTexture.active;
        RenderTexture.active = rt;

        GL.Clear(false, true, Color.white);
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, width, 0, height);
        mat.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(Color.green);
        float previous = 0;
        for (int i = 0; i < width; i++)
        {
            GL.Vertex3(i, (previous / 0.1f) * height, 0);
            GL.Vertex3(i + 1, (previous / 0.1f) * height, 0);
        }
        GL.End();

        DrawJump(jump);


        GL.Begin(GL.LINES);
        GL.Color(new Color(0, 0, 0, 0.5f));

        for (int i = 0; i < textSizeY; i += graduationSize)  // Horizontal
        {
            GL.Vertex3(0, i , 0);
            GL.Vertex3(width, i , 0);
        }
        
        for (int i = 0; i < textSizeX; i += graduationSize)  // Horizontal
        {
            GL.Vertex3(i, 0, 0);
            GL.Vertex3(i, width, 0);
        }

        // contour gauche
        GL.Color(Color.black);
        GL.Vertex3(1, height, 0);
        GL.Vertex3(1, 1, 0);

        // contour haut
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(width, 1, 0);

        // contour droite
        GL.Vertex3(width, 0, 0);
        GL.Vertex3(width, height, 0);

        // contour bas
        GL.Vertex3(width, height, 0);
        GL.Vertex3(0, height, 0);

        GL.End();
        GL.PopMatrix();
        GL.Flush();
        RenderTexture.active = oldActiveRT;
    }

    // A partir d'ici caca à réécrire.

    static void DrawJump(Jump jump)
    {
        if (jump.HasFallingParabola)
        {
            DrawHalfParabola(jump.upParabola, Color.black, true);
            DrawHalfParabola(jump.FallingParabola, Color.red, false, jump.upParabola.Xz_h * graduationSize);
        }
        else
            DrawParabola(jump.upParabola, Color.black);
        if (jump.IsContinue)
            DrawParabola(jump.MinJumpParabola, Color.red);
    }
    static void DrawParabola(Parabola p, Color color)
    {
        p.ComputeValues(10); // La valeur on s'en fout, ça change rien (mais 0 foutrait la merde)
        int pas = 100;
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex3(0, 0, 0);
        float parabolaMaxTime = 2 * p.getT_h();
        for (float i = 1; i < pas; i++)
        {
            float time = i * parabolaMaxTime / pas;
            float paraboleH = p.getPosition(time);
            float x = p.Xz_h * 2 * i / pas * graduationSize;
            float y = paraboleH * graduationSize;
            GL.Vertex3(x, y, 0);
            GL.Vertex3(x, y, 0);
        }
        GL.End();
    }
    static void DrawHalfParabola(Parabola p, Color color, bool firstHalf, float decalage = 0)
    {
        p.ComputeValues(10); // La valeur on s'en fout, ça change rien (mais 0 foutrait la merde)
        int pas = 100;
        int iEnd = 0;
        int iBegin = 0;
        float timeEnd = 2 * p.getT_h();

        GL.Begin(GL.LINES);
        GL.Color(color);

        if (firstHalf)
        {
            GL.Vertex3(0, 0, 0);
            iEnd = 50;
        }
        else
        {
            iBegin = pas / 2;
            GL.Vertex3(p.Xz_h * graduationSize, p.getPosition(timeEnd / 2) * graduationSize, 0);
        }
        for (float i = 1 + iBegin; i < pas - iEnd; i++)
        {
            float time = i * timeEnd / pas;
            float paraboleH = p.getPosition(time);
            float x = p.Xz_h * 2 * (i - iBegin) / pas * graduationSize + decalage;
            float y = paraboleH * graduationSize;
            GL.Vertex3(x, y, 0);
            GL.Vertex3(x, y, 0);
        }
        GL.End();
    }
}

