using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ease
{
    [System.Serializable]
    public enum EASE_TYPE
    {
        LINEAR,
        QUAD_IN,
        QUAD_OUT,
        QUAD_INOUT,
        CUBE_IN,
        CUBE_OUT,
        CUBE_INOUT,
        QUART_IN,
        QUART_OUT,
        QUART_INOUT,
        QUINT_IN,
        QUINT_OUT,
        QUINT_INOUT,
        SINE_IN,
        SINE_OUT,
        SINE_INOUT,
        BOUNCE_IN,
        BOUNCE_OUT,
        BOUNCE_INOUT,
        CIRC_IN,
        CIRC_OUT,
        CIRC_INOUT,
        EXPO_IN,
        EXPO_OUT,
        EXPO_INOUT,
        BACK_IN,
        BACK_OUT,
        BACK_INOUT
    }

    public static float Evaluate(EASE_TYPE ease, float v)
    {
        switch (ease)
        {
            case EASE_TYPE.QUAD_IN:
                return quadIn(v);
            case EASE_TYPE.QUAD_OUT:
                return quadOut(v);
            case EASE_TYPE.QUAD_INOUT:
                return quadInOut(v);
            case EASE_TYPE.CUBE_IN:
                return cubeIn(v);
            case EASE_TYPE.CUBE_OUT:
                return cubeOut(v);
            case EASE_TYPE.CUBE_INOUT:
                return cubeInOut(v);
            case EASE_TYPE.QUART_IN:
                return quartIn(v);
            case EASE_TYPE.QUART_OUT:
                return quartOut(v);
            case EASE_TYPE.QUART_INOUT:
                return quartInOut(v);
            case EASE_TYPE.QUINT_IN:
                return quintIn(v);
            case EASE_TYPE.QUINT_OUT:
                return quintOut(v);
            case EASE_TYPE.QUINT_INOUT:
                return quintInOut(v);
            case EASE_TYPE.SINE_IN:
                return sineIn(v);
            case EASE_TYPE.SINE_OUT:
                return sineOut(v);
            case EASE_TYPE.SINE_INOUT:
                return sineInOut(v);
            case EASE_TYPE.BOUNCE_IN:
                return bounceIn(v);
            case EASE_TYPE.BOUNCE_OUT:
                return bounceOut(v);
            case EASE_TYPE.BOUNCE_INOUT:
                return bounceInOut(v);
            case EASE_TYPE.CIRC_IN:
                return circIn(v);
            case EASE_TYPE.CIRC_OUT:
                return circOut(v);
            case EASE_TYPE.CIRC_INOUT:
                return circInOut(v);
            case EASE_TYPE.EXPO_IN:
                return expoIn(v);
            case EASE_TYPE.EXPO_OUT:
                return expoOut(v);
            case EASE_TYPE.EXPO_INOUT:
                return expoInOut(v);
            case EASE_TYPE.BACK_IN:
                return backIn(v);
            case EASE_TYPE.BACK_OUT:
                return backOut(v);
            case EASE_TYPE.BACK_INOUT:
                return backInOut(v);
            default:
                return linear(v);
        }
    }

    // Easing constants.
    private static float PI = Mathf.PI;
    private static float PI2 = PI / 2.0f;
    private static float EL = 2 * PI / 0.45f;
    private static float B1 = 1.0f / 2.75f;
    private static float B2 = 2.0f / 2.75f;
    private static float B3 = 1.5f / 2.75f;
    private static float B4 = 2.5f / 2.75f;
    private static float B5 = 2.25f / 2.75f;
    private static float B6 = 2.625f / 2.75f;

    /** Linear easing */
    public static float linear(float t)
    {
        return t;
    }

    /** Quadratic in. */
    public static float quadIn(float t)
    {
        return t * t;
    }

    /** Quadratic out. */
    public static float quadOut(float t)
    {
        return -t * (t - 2);
    }

    /** Quadratic in and out. */
    public static float quadInOut(float t)
    {
        return t <= .5 ? t * t * 2 : 1 - (--t) * t * 2;
    }

    /** Cubic in. */
    public static float cubeIn(float t)
    {
        return t * t * t;
    }

    /** Cubic out. */
    public static float cubeOut(float t)
    {
        return 1 + (--t) * t * t;
    }

    /** Cubic in and out. */
    public static float cubeInOut(float t)
    {
        return t <= .5 ? t * t * t * 4 : 1 + (--t) * t * t * 4;
    }

    /** Quart in. */
    public static float quartIn(float t)
    {
        return t * t * t * t;
    }

    /** Quart out. */
    public static float quartOut(float t)
    {
        return 1 - (t -= 1) * t * t * t;
    }

    /** Quart in and out. */
    public static float quartInOut(float t)
    {
        return t <= .5f ? t * t * t * t * 8 : (1 - (t = t * 2 - 2) * t * t * t) / 2 + .5f;
    }

    /** Quint in. */
    public static float quintIn(float t)
    {
        return t * t * t * t * t;
    }

    /** Quint out. */
    public static float quintOut(float t)
    {
        return (t = t - 1) * t * t * t * t + 1;
    }

    /** Quint in and out. */
    public static float quintInOut(float t)
    {
        return ((t *= 2) < 1) ? (t * t * t * t * t) / 2 : ((t -= 2) * t * t * t * t + 2) / 2;
    }

    /** Sine in. */
    public static float sineIn(float t)
    {
        return -Mathf.Cos(PI2 * t) + 1;
    }

    /** Sine out. */
    public static float sineOut(float t)
    {
        return Mathf.Sin(PI2 * t);
    }

    /** Sine in and out. */
    public static float sineInOut(float t)
    {
        return -Mathf.Cos(PI * t) / 2.0f + .5f;
    }

    /** Bounce in. */
    public static float bounceIn(float t)
    {
        t = 1 - t;
        if (t < B1) return 1.0f - 7.5625f * t * t;
        if (t < B2) return 1.0f - (7.5625f * (t - B3) * (t - B3) + .75f);
        if (t < B4) return 1.0f - (7.5625f * (t - B5) * (t - B5) + .9375f);
        return 1.0f - (7.5625f * (t - B6) * (t - B6) + .984375f);
    }

    /** Bounce out. */
    public static float bounceOut(float t)
    {
        if (t < B1) return 7.5625f * t * t;
        if (t < B2) return 7.5625f * (t - B3) * (t - B3) + .75f;
        if (t < B4) return 7.5625f * (t - B5) * (t - B5) + .9375f;
        return 7.5625f * (t - B6) * (t - B6) + .984375f;
    }

    /** Bounce in and out. */
    public static float bounceInOut(float t)
    {
        if (t < .5f)
        {
            t = 1.0f - t * 2;
            if (t < B1) return (1.0f - 7.5625f * t * t) / 2.0f;
            if (t < B2) return (1.0f - (7.5625f * (t - B3) * (t - B3) + .75f)) / 2.0f;
            if (t < B4) return (1.0f - (7.5625f * (t - B5) * (t - B5) + .9375f)) / 2.0f;
            return (1.0f - (7.5625f * (t - B6) * (t - B6) + .984375f)) / 2.0f;
        }
        t = t * 2 - 1;
        if (t < B1) return (7.5625f * t * t) / 2.0f + .5f;
        if (t < B2) return (7.5625f * (t - B3) * (t - B3) + .75f) / 2.0f + .5f;
        if (t < B4) return (7.5625f * (t - B5) * (t - B5) + .9375f) / 2.0f + .5f;
        return (7.5625f * (t - B6) * (t - B6) + .984375f) / 2.0f + .5f;
    }

    /** Circle in. */
    public static float circIn(float t)
    {
        return -(Mathf.Sqrt(1 - t * t) - 1);
    }

    /** Circle out. */
    public static float circOut(float t)
    {
        return Mathf.Sqrt(1 - (t - 1) * (t - 1));
    }

    /** Circle in and out. */
    public static float circInOut(float t)
    {
        return t <= .5 ? (Mathf.Sqrt(1 - t * t * 4) - 1) / -2 : (Mathf.Sqrt(1 - (t * 2 - 2) * (t * 2 - 2)) + 1) / 2;
    }

    /** Exponential in. */
    public static float expoIn(float t)
    {
        return Mathf.Pow(2, 10 * (t - 1));
    }

    /** Exponential out. */
    public static float expoOut(float t)
    {
        return -Mathf.Pow(2, -10 * t) + 1;
    }

    /** Exponential in and out. */
    public static float expoInOut(float t)
    {
        return t < .5 ? Mathf.Pow(2, 10 * (t * 2 - 1)) / 2 : (-Mathf.Pow(2, -10 * (t * 2 - 1)) + 2) / 2;
    }

    /** Back in. */
    public static float backIn(float t)
    {
        return t * t * (2.70158f * t - 1.70158f);
    }

    /** Back out. */
    public static float backOut(float t)
    {
        return 1 - (--t) * (t) * (-2.70158f * t - 1.70158f);
    }

    /** Back in and out. */
    public static float backInOut(float t)
    {
        t *= 2;
        if (t < 1) return t * t * (2.70158f * t - 1.70158f) / 2.0f;
        t--;
        return (1 - (--t) * (t) * (-2.70158f * t - 1.70158f)) / 2.0f + .5f;
    }
}
