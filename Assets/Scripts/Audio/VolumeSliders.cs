using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VolumeSliders : MonoBehaviour {
    [SerializeField]
    Slider musicSlider;
    [SerializeField]
    Slider effectsSlider;

	void Start () {
        musicSlider.value = AudioManager.Instance.VolumeMusic;
        effectsSlider.value = AudioManager.Instance.VolumeFXs;
    }
	
    public void UpdateMusicVolume(float volume)
    {
        AudioManager.Instance.VolumeMusic = musicSlider.value;
    }

    public void UpdateEffectsVolume(float volume)
    {
        AudioManager.Instance.VolumeFXs = effectsSlider.value;
    }
}
