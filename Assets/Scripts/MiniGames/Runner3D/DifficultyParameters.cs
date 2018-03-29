using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyParameters
{
    [SerializeField] private string name;
    [SerializeField] private float[] table;
    [SerializeField] private float[] palierTab;

    [SerializeField] private int nbPalier;
    [SerializeField] private int nbOutput;

    public float[] PalierTab
    {
        get
        {
            return palierTab;
        }
        private set
        {
            palierTab = value;
        }
    }
    // find the right row : 
    public int GetPallierAt(float value)
    {
        int row = -1;
        for (int x = nbOutput - 1; x >= 0; x--)
        {
            if (value >= palierTab[x])
            {
                row = x;
                break;
            }
        }
        if (row == -1)
        {
            Debug.LogError("Invalid pallier :  min palier is :" + palierTab[0] + "asked value is :" + value);
            return 0;
        }
        return row;
    }

    public int GetTableRandAt(float value)
    {
        float rand = Random.Range(0, 100);
        // find the right row : 
        int row = GetPallierAt(value);
       
            // take random value at pct. 
        float curPct = 100;
        for (int x = nbOutput-1; x >=0;x-- )
        {
            int id = row * nbOutput + x;
            curPct -= table[id];
            if ( rand >= curPct || x == 0)
                return x;
        }
        return 0;
    }
}




