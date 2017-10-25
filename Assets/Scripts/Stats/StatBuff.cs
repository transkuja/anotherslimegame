using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBuff  {

    private Stats.StatType statType;
    private float value;
    private float timer;

    public Stats.StatType StatType
    {
        get{return statType;}
        set {statType = value;}
    }

    public float Value
    {
        get{return value;}
        set{this.value = value;}
    }

    public float Timer
    {
        get{return timer;}
        set{timer = value;}
    }
    public float UpdateTimer()
    {
        if (timer !=-1) // Infinity
        {
            timer -= Time.deltaTime;
        }
        return timer;
    }
    /// <summary>
    /// Create Stat modification
    /// </summary>
    /// <param name="statType"> stat type </param>
    /// <param name="value"></param>
    /// <param name="timer">-1 for infinity </param>
    /// 
    public StatBuff(Stats.StatType _statType, float _value,float _delay)
    {
         statType = _statType;
         value = _value;
         timer = _delay;
    }
}
