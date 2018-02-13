using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager s_instance = null;

    public AudioSource sourceMusic;
    public AudioSource sourceFX;

    public AudioClip musicGame;

    public AudioClip coinFX;
    public AudioClip youpiFX;
    public AudioClip hahahaFX;
    public AudioClip wahhFx;
    public AudioClip blobSound;
    public AudioClip punchFx;
    public AudioClip breakFx;
    public AudioClip cantPayFx;

    public AudioClip musicAssenseur;

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
            sourceFX.volume = volumeFXs;
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
        currentVolume = volumeMusic;
        sourceMusic.Play();
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
        sourceFX.PlayOneShot(clip, volumeFXs);
    }

    public void PlayOneShot(AudioClip clip, float volumeMultiplier)
    {
        sourceFX.PlayOneShot(clip, volumeFXs * volumeMultiplier);
    }

    public void Play(AudioClip clip, float volumeMultiplier = 1.0f)
    {
        if (sourceFX.clip != clip)
            sourceFX.clip = clip;
        sourceFX.volume = volumeFXs * volumeMultiplier;
        sourceFX.Play();
    }

}
