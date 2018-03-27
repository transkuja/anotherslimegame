using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimeFormat
{
    Min,
    TotalMin,
    MinSec,
    MinSecMil,
    SecMil,
    Sec,
    TotalSec,
    Mil,
    TotalMil
}

public class TimeFormatUtils : MonoBehaviour {
    public static string GetFormattedTime(float time, TimeFormat format)
    {
        string timeStr = string.Empty;
        TimeSpan ts = TimeSpan.FromSeconds(time);
        switch (format)
        {
            case TimeFormat.Min:
                timeStr = string.Format("{0:00}", ts.Minutes);
                break;
            case TimeFormat.MinSec:
                timeStr = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
                break;
            case TimeFormat.MinSecMil:
                timeStr = string.Format("{0:00}:{1:00}:{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
                break;
            case TimeFormat.SecMil:
                timeStr = string.Format("{0:00}:{1:000}", ts.Seconds, ts.Milliseconds);
                break;
            case TimeFormat.Sec:
                timeStr = string.Format("{0:00}", ts.Seconds);
                break;
            case TimeFormat.Mil:
                timeStr = string.Format("{0:000}", ts.Milliseconds);
                break;
            case TimeFormat.TotalMin:
                timeStr = string.Format("{0:00}", ts.TotalMinutes);
                break;
            case TimeFormat.TotalSec:
                timeStr = string.Format("{0:00}", ts.TotalSeconds);
                break;
            case TimeFormat.TotalMil:
                timeStr = string.Format("{0:000}", ts.TotalMilliseconds);
                break;
        }

        return timeStr;
    }
}
