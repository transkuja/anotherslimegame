﻿using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager s_instance = null;

    public AudioSource sourceMusic;
    public List<AudioSource> sourceFX;
    public AudioSource windFxSource;

    public AudioClip musicMenu;
    public AudioClip musicGame;
    public AudioClip musicMinigame;

    public AudioClip coinFX;
    public AudioClip youpiFX;
    public AudioClip hahahaFX;
    public AudioClip wahhFx;
    public AudioClip blobSound;
    public AudioClip punchFx;
    public AudioClip breakFx;
    public AudioClip cantPayFx;

    public AudioClip jumpFx;
    public AudioClip dashFx;
    public AudioClip splashWaterFx;
    public AudioClip swimmingFx;
    public AudioClip sandStepFx;
    public AudioClip cascadaFx;
    public AudioClip windBlowingFx;

    public AudioClip buttonValidationFx;
    public AudioClip changeOptionFx;
    public AudioClip countdownStepFx;
    public AudioClip countdownEndFx;
    public AudioClip shaveFx;
    public AudioClip incorrectFx;
    public AudioClip positiveSoundFx;
    public AudioClip buySoundFx;

    public AudioClip musicAscenseur;
    public AudioClip endElevatorFx;

    private AudioClip musicToPlay;
    private AudioClip previousMusic;
    

    [SerializeField]
    private float volumeMusic = 0.015f;
    [SerializeField]
    private float volumeFXs = 0.01f;


    private float currentVolume;

    public static AudioManager Instance
    {
        get
        {
            return s_instance;
        }
    }

    public float VolumeMusic
    {
        get
        {
            return currentVolume;
        }

        set
        {
            currentVolume = value;
            sourceMusic.volume = currentVolume;
        }
    }

    public float VolumeFXs
    {
        get
        {
            return volumeFXs;
        }

        set
        {
            volumeFXs = value;
            foreach (AudioSource source in sourceFX)
                source.volume = volumeFXs;
        }
    }

    void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            DontDestroyOnLoad(gameObject);
            //transform.GetChild(0).GetComponent<AudioSource>().clip = musicGame;
        }
        else if (s_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Should be called only in menu
        currentVolume = volumeMusic;
        sourceMusic.clip = musicMenu;
        sourceMusic.Play();
    }


    public void Init()
    {
       
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.IsInHub())
            {
                s_instance.Fade(musicGame);
            }
            else
                s_instance.Fade(musicMinigame);
        }
    }

    private bool isFading = false;
    private float timerFade = 0.0f;

    public void Fade(AudioClip _music, float timer = 1.0f, float multiplierVolume = 1.0f)
    {
        if (sourceMusic.clip != _music)
        {
            musicToPlay = _music;
            currentVolume = multiplierVolume * volumeMusic;
            timerFade = timer;
            isFading = true;
        }

    }

    public void PlayMusic(AudioClip clip)
    {
        isFading = false;
        sourceMusic.clip = clip;
        sourceMusic.volume = currentVolume;
        sourceMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            timerFade -= Time.deltaTime;
            if (timerFade < 0)
            {
                timerFade = 0.0f;
                sourceMusic.volume = currentVolume;
                sourceMusic.clip = musicToPlay;
                sourceMusic.Play();
                isFading = false;
            }
            else
            {
                sourceMusic.volume = timerFade * currentVolume;
            }
        }
        else
        {
            sourceMusic.volume = currentVolume;
        }

    }

    public void PlayOneShot(AudioClip clip)
    {
        int sourceFXIndex = 0;
        if (sourceFX[sourceFXIndex].isPlaying)
        {
            sourceFXIndex++;
        }

        sourceFX[sourceFXIndex].PlayOneShot(clip, volumeFXs);
    }

    public void PlayOneShot(AudioClip clip, float volumeMultiplier)
    {
        int sourceFXIndex = 0;
        if (sourceFX[sourceFXIndex].isPlaying)
        {
            sourceFXIndex++;
        }

        sourceFX[sourceFXIndex].PlayOneShot(clip, volumeFXs * volumeMultiplier);
    }

    public void Play(AudioClip clip, float volumeMultiplier = 1.0f)
    {
        int sourceFXIndex = 0;
        if (sourceFX[sourceFXIndex].isPlaying)
        {
            sourceFXIndex++;
        }

        if (sourceFX[sourceFXIndex].clip != clip)
            sourceFX[sourceFXIndex].clip = clip;
        sourceFX[sourceFXIndex].volume = volumeFXs * volumeMultiplier;
        sourceFX[sourceFXIndex].Play();
    }

    public void PlayWind()
    {
        if (!windFxSource.isPlaying)
        {
            windFxSource.clip = windBlowingFx;
            windFxSource.volume = volumeFXs * 10.0f;
            windFxSource.Play();
        }
    }

    public void StopWind()
    {
        if (windFxSource.isPlaying)
            windFxSource.Stop();

    }

    public void ClearFX(AudioClip _fxToClear)
    {
        foreach (AudioSource source in sourceFX)
        {
            if (source.clip == _fxToClear)
            {
                source.Stop();
                source.clip = null;
            }
        }
    }

}
