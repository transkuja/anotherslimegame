using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2 solution : 
// A : recalculer la stat à chaque get. 
// B : PréEnregistrer le résultat. et la stat de base.

[System.Serializable]
public class Stats  {
	public enum StatType
    {
        GROUND_SPEED,
        AIR_CONTROL,
        TURN_SPEED, // non implémenté
        JUMP_HEIGHT, // non implémenté
        JUMP_NB,  // non implémenté
        DASH_FORCE,// non implémenté
        DASH_RESISTANCE, // non implémenté

        MAX_STATS
    }
    [System.Serializable]
    public struct Stat
    {
        [HideInInspector]public string name; // ne sert qu'a l'affichage dans l'éditor si une autre solution existe je suis chaud
        public float baseStat;
        [HideInInspector]public float currentStat;
    }
    private List<StatBuff> buffList;
    [SerializeField] private Stat[] stats = new Stat[(int)StatType.MAX_STATS]; // tableau contenant toutes les stats du joueur


    public Stats()
    {
        for (int i = 0; i < (int)StatType.MAX_STATS; i++)
        {
            stats[i].name = ((StatType)i).ToString();
            stats[i].currentStat = stats[i].baseStat;
        }
        buffList = new List<StatBuff>();
    }
    public void Init()
    {
        stats = new Stat[(int)StatType.MAX_STATS];
        stats[(int)Stats.StatType.GROUND_SPEED].baseStat = 30;
        stats[(int)Stats.StatType.AIR_CONTROL].baseStat = 20;

        for (int i = 0; i < (int)StatType.MAX_STATS; i++)
        {
            stats[i].name = ((StatType)i).ToString();
            stats[i].currentStat = stats[i].baseStat;
        }

        buffList = new List<StatBuff>();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="statType"></param>
    /// <param name="isCurrent">Permet de récuperer la stat avec en prenant en compte les buff </param>
    /// <returns></returns>
    public float  Get(StatType statType, bool isCurrent = true)
    {
        if (isCurrent)
            return stats[(int)statType].currentStat;
        else
            return stats[(int)statType].baseStat;
    }


    /// <summary>
    /// Change the value of the BASE stat of player (without appying buff)
    /// </summary>
    /// <param name="statType"> </param>
    /// <param name="value"></param>
    public void Set(StatType statType,float value)
    {
        stats[(int)statType].baseStat = value;
        UpdateStat(statType);
    }
    public void AddBuff(StatBuff buff)
    {
        if (buffList == null)
            buffList = new List<StatBuff>();
        buffList.Add(buff);
        UpdateStat(buff.StatType);
        Debug.Log("Buff Added");
    }
    public void RemoveBuff(StatBuff buff)
    {
        StatType stat = buff.StatType;
        buffList.Remove(buff);
        UpdateStat(stat);
        Debug.Log("Buff Removed");
    }
    private void UpdateStat(StatType stat)
    {
        float newStatValue = stats[(int)stat].baseStat;
        for (int i = 0;  i < buffList.Count;i++)
        {
            if (buffList[i].StatType == stat)
            {
                newStatValue *= buffList[i].Value;
            }
        }
        stats[(int)stat].currentStat = newStatValue;

        if (stat == StatType.JUMP_HEIGHT)
        {
        }
    }
    public void Update()
    {
        if (buffList != null)
            for (int i = 0;i < buffList.Count;i++)
            {
                StatBuff buff = buffList[i];
                float timer = buff.UpdateTimer();
                if (timer !=-1 && timer < 0)
                {
                    RemoveBuff(buff);
                }
            }
    }
}

