using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using DatabaseClass;

public class SlimeDataContainer : MonoBehaviour {
    [Tooltip("These are the player colors (R,B,G,Y). These are definitive, do not change them.")]
    public Color[] playerColorsMenu = new Color[4];
    [Tooltip("These are the player colors (R,B,G,Y). These are definitive, do not change them.")]
    public Color[] playerColorsUI = new Color[4];
    [Tooltip("These are the player colors (R,B,G,Y). These are definitive, do not change them.")]
    public Color[] playerColorsEmissive = new Color[4];

    public static SlimeDataContainer instance = null;
    public int nbPlayers = -1;
    public Color[] selectedColors = new Color[4];
    public int[] selectedFaces = new int[4];
    public bool[] colorFadeSelected = new bool[4];
    public string[] mustachesSelected = new string[4];
    public string[] hatsSelected = new string[4];
    public string[] earsSelected = new string[4];
    public string[] chinsSelected = new string[4];
    public string[] skinsSelected = new string[4];
    public string[] accessoriesSelected = new string[4];
    public string[] foreheadsSelected = new string[4];

    public bool launchedFromMinigameScreen = false;
    public int minigameVersion = 0;

    void Awake () {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);

            if (SceneManager.GetActiveScene().buildIndex == 0)
                FindObjectOfType<Menu>().DataContainer = this;

            GameManager.Instance.RegisterDataContainer(this);
        }

    }

    public void SaveData(int _nbPlayers, Color[] _selectedColors, int[] _selectedFaces, 
        string[] _selectedMustaches, string[] _selectedHats, string[] _selectedEars, string[] _selectedForeheads, 
        string[] _selectedChins, string[] _selectedSkins, string[] _selectedAccessories, int _minigameVersion = 0,
        bool[] _colorFadeSelected = null, bool _launchedFromMinigameScreen = false)
    {
        selectedColors = _selectedColors;
        selectedFaces = _selectedFaces;
        nbPlayers = _nbPlayers;
        colorFadeSelected = (_colorFadeSelected == null) ? new bool[4] : _colorFadeSelected;
        mustachesSelected = _selectedMustaches;
        hatsSelected = _selectedHats;
        earsSelected = _selectedEars;
        launchedFromMinigameScreen = _launchedFromMinigameScreen;
        minigameVersion = _minigameVersion;
        foreheadsSelected = _selectedForeheads;
        chinsSelected = _selectedChins;
        skinsSelected = _selectedSkins;
        accessoriesSelected = _selectedAccessories;
    }
}
