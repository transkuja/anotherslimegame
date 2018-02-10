﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseClass;

public class SlimeDataContainer : MonoBehaviour {
    public static SlimeDataContainer instance = null;
    public int nbPlayers = -1;
    public Color[] selectedColors = new Color[4];
    public int[] selectedFaces = new int[4];
    public bool[] colorFadeSelected = new bool[4];
    public bool launchedFromMinigameScreen = false;

    void Start () {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
            GameManager.Instance.RegisterDataContainer(this);
        }

    }
	
	public void SaveData(int _nbPlayers, Color[] _selectedColors, int[] _selectedFaces, bool[] _colorFadeSelected = null, bool _launchedFromMinigameScreen = false)
    {
        selectedColors = _selectedColors;
        selectedFaces = _selectedFaces;
        nbPlayers = _nbPlayers;
        colorFadeSelected = (_colorFadeSelected == null) ? new bool[4] : _colorFadeSelected;
        launchedFromMinigameScreen = _launchedFromMinigameScreen;
    }
}
