using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeDataContainer : MonoBehaviour {

    public Color[] selectedColors = new Color[4];
    public int[] selectedFaces = new int[4];

    void Start () {
        DontDestroyOnLoad(this);
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterDataContainer(this);
	}
	
	public void SaveData(Color[] _selectedColors, int[] _selectedFaces)
    {
        selectedColors = _selectedColors;
        selectedFaces = _selectedFaces;
    }
}
