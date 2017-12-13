using UnityEngine;

public class WaterImmersionCamera : MonoBehaviour {

    public bool isImmerge;
    public Color color;
    public float fogdensity = 0.20f;

    //The scene's default fog settings
    private bool defaultFog;
    private Color defaultFogColor;
    private float defaultFogDensity;
    private Material defaultSkybox;

    // Use this for initialization
    void Start () {
        defaultFog = RenderSettings.fog;
        defaultFogColor = RenderSettings.fogColor;
        defaultFogDensity = RenderSettings.fogDensity;
        defaultSkybox = RenderSettings.skybox;
    }

    // Update is called once per frame
    private void OnPreRender()
    {
        if (isImmerge)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = color/*new color(0, 0.4f, 0.7f, 0.6f)*/;
            RenderSettings.fogDensity = fogdensity;
            RenderSettings.skybox = null;
        }
    }

    private void OnPostRender()
    {
        if (!isImmerge)
        {
            RenderSettings.fog = defaultFog;
            RenderSettings.fogColor = defaultFogColor;
            RenderSettings.fogDensity = defaultFogDensity;
            RenderSettings.skybox = defaultSkybox;
        }
    }
}
