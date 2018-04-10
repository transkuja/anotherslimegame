using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeAdapter : MonoBehaviour {
    public float multiplier = 0.5f;
    AudioSource source;
	void Start () {
        source = GetComponent<AudioSource>();
        if(AudioManager.Instance)
            source.volume = AudioManager.Instance.VolumeFXs * multiplier;
	}
	
	// Update is called once per frame
	void Update () {
        if(AudioManager.Instance || source.volume != AudioManager.Instance.VolumeFXs)
            source.volume = AudioManager.Instance.VolumeFXs * multiplier;
    }
}
