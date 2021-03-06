﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwimmingAudioHandle : MonoBehaviour {

    public void PlaySwimmingSound()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.swimmingFx != null)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.swimmingFx, 1.0f, 1.0f);
    }
}
