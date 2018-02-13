using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiantMusicPlayer : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {

            if (AudioManager.Instance != null && AudioManager.Instance.musicAssenseur != null)
            {
                AudioManager.Instance.Fade(AudioManager.Instance.musicAssenseur, 0.5f, 3.0f);
            }
        
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            if (AudioManager.Instance != null && AudioManager.Instance.musicGame != null)
            {
                AudioManager.Instance.Fade(AudioManager.Instance.musicGame, 0.5f);
            }

        }
    }
}
