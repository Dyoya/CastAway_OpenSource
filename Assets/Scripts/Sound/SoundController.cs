using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public AudioMixer masteMixer;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public void AudioController()
    {
        float bgmSound = bgmSlider.value;
        float sfxSound = sfxSlider.value;

        if (bgmSound == -40f) masteMixer.SetFloat("BGM", -80);
        else masteMixer.SetFloat("BGM", bgmSound);

        if (sfxSound == -40f) masteMixer.SetFloat("SFX", -80);
        else masteMixer.SetFloat("SFX", bgmSound);

    }
}
