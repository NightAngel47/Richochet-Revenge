using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    public AudioSource audio;

    public void playSound(AudioClip clip)
    {
        audio.clip = clip;
        audio.Play();
    }
}
