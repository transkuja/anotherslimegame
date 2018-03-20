using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CascadaZoneTrigger : MonoBehaviour {

    PlayerController lastPlayerEntered;
    AudioSource cascadaAudioSource;
    float currentVolume;

    float timerFade;
    bool isFading = false;
    bool isFadingOut = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            lastPlayerEntered = other.GetComponent<PlayerController>();
            CascadaAudioSource.volume = AudioManager.Instance.VolumeFXs;
            currentVolume = AudioManager.Instance.VolumeFXs;
            IsFadingOut = false;
           // CascadaAudioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            if (lastPlayerEntered.PlayerIndex == other.GetComponent<PlayerController>().PlayerIndex)
            {
                lastPlayerEntered = null;
                IsFadingOut = true;
            }
        }
    }


    public AudioSource CascadaAudioSource
    {
        get
        {
            if (cascadaAudioSource == null)
                cascadaAudioSource = GetComponent<AudioSource>();
            return cascadaAudioSource;
        }

        set
        {
            cascadaAudioSource = value;
        }
    }

    public bool IsFadingOut
    {
        get
        {
            return isFadingOut;
        }

        set
        {
            isFadingOut = value;
            isFading = true;
            timerFade = 1.0f;
        }
    }

    private void Update()
    {
        if (isFading)
        {
            timerFade -= Time.deltaTime;
            if (timerFade < 0)
            {
                timerFade = 0.0f;
                CascadaAudioSource.volume = currentVolume;
                if (IsFadingOut)
                    CascadaAudioSource.Stop();
                else
                    CascadaAudioSource.Play();
                isFading = false;
            }
            else
            {
                CascadaAudioSource.volume = timerFade * currentVolume;
            }
        }
        else
        {
            CascadaAudioSource.volume = currentVolume;
        }

        if (lastPlayerEntered != null)
        {
            if (Vector3.Distance(lastPlayerEntered.transform.position, transform.position) < 10.0f)
            {
                CascadaAudioSource.volume = AudioManager.Instance.VolumeFXs * 2;
            }
            else
            {
                CascadaAudioSource.volume = AudioManager.Instance.VolumeFXs;
            }
        }
    }

}
