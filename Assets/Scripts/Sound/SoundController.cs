using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class SoundController : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private const string BgmVolumeKey = "BgmVolume";
    private const string SfxVolumeKey = "SfxVolume";

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadSettings(); // ����� ���� �� �ε�
        ApplySettings(); // ���� �� ����
    }

    public void AudioController()
    {
        float bgmSound = bgmSlider.value;
        float sfxSound = sfxSlider.value;

        SetBgmVolume(bgmSound);
        SetSfxVolume(sfxSound);

        SaveSettings(); // ���� ���� PlayerPrefs�� ����

    }

    private void SetBgmVolume(float volume)
    {
        if (volume == -40f)
            masterMixer.SetFloat("BGM", -80);
        else
            masterMixer.SetFloat("BGM", volume);
    }

    private void SetSfxVolume(float volume)
    {
        if (volume == -40f)
            masterMixer.SetFloat("SFX", -80);
        else
            masterMixer.SetFloat("SFX", volume);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(BgmVolumeKey, bgmSlider.value);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxSlider.value);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        float savedBgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey, 0);
        float savedSfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0);

        bgmSlider.value = savedBgmVolume;
        sfxSlider.value = savedSfxVolume;
    }

    private void ApplySettings()
    {
        SetBgmVolume(bgmSlider.value);
        SetSfxVolume(sfxSlider.value);
    }
}