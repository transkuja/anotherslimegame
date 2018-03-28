using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyParameters
{
    [SerializeField]private string name;
    [SerializeField]private float[] table;
    [SerializeField] private int nbPallier;
    [SerializeField] private int nbOutput;
    public float GetPct(int pallier, int output)
    {
        return table[pallier + output];
    }
}




