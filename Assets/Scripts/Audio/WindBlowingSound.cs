using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBlowingSound : MonoBehaviour {

    bool isWindActive = false;
    float currentVolume;

    float timerFade;
    bool isFading = false;
    bool isFadingOut = false;

    public bool IsWindActive
    {
        get
        {
            return isWindActive;
        }

        set
        {
            isWindActive = value;
            isFadingOut = !isWindActive;
            isFading = true;
            timerFade = 1.0f;
        }
    }

    void Update () {

        if (isFading)
        {
            timerFade -= Time.deltaTime;
            if (timerFade < 0)
            {
                timerFade = 0.0f;
                AudioManager.Instance.windFxSource.volume = currentVolume;
                if (isFadingOut)
                    AudioManager.Instance.StopWind();
                else
                    AudioManager.Instance.PlayWind();
                isFading = false;
            }
            else
            {
                AudioManager.Instance.windFxSource.volume = timerFade * currentVolume;
            }
        }
        else
        {
            AudioManager.Instance.windFxSource.volume = currentVolume;
        }

        if (transform.position.y > 125.0f && !IsWindActive)
        {
            IsWindActive = true;
            currentVolume = AudioManager.Instance.VolumeFXs * 10.0f;
        }
        else if (transform.position.y < 125.0f && IsWindActive)
        {
            IsWindActive = false;
        }
    }
}
