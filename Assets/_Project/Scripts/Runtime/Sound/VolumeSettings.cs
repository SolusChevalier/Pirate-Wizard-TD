using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    #region FIELDS

    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider MasterSlider;
    public Slider SFXSlider;
    public Toggle MusicToggle;
    public Toggle MasterToggle;
    public Toggle SFXToggle;

    [SerializeField]
    [Range(-80f, 20f)]
    private float masterVolume = 0f;

    private float lastMasterVolume = 0f;

    [SerializeField]
    private bool masterMute = false;

    [SerializeField]
    [Range(-80f, 20f)]
    private float musicVolume = 0f;

    private float lastMusicVolume = 0f;

    [SerializeField]
    private bool musicMute = false;

    [SerializeField]
    [Range(-80f, 20f)]
    private float sfxVolume = 0f;

    private float lastSfxVolume = 0f;

    [SerializeField]
    private bool sfxMute = false;

    #endregion FIELDS

    #region METHODS

    public void SetMusicVolume()
    {
        musicVolume = MusicSlider.value - 30;
        audioMixer.SetFloat("MusicVolume", musicVolume);
        if (musicMute)
        {
            musicMute = false;
            MusicToggle.isOn = false;
        }
    }

    public void SetMasterVolume()
    {
        masterVolume = MasterSlider.value - 30;
        audioMixer.SetFloat("MasterVolume", masterVolume);
        if (masterMute)
        {
            masterMute = false;
            MasterToggle.isOn = false;
        }
    }

    public void SetSFXVolume()
    {
        sfxVolume = SFXSlider.value - 30;
        audioMixer.SetFloat("SFXVolume", sfxVolume);
        if (sfxMute)
        {
            sfxMute = false;
            SFXToggle.isOn = false;
        }
    }

    public void MuteMusic()
    {
        if (musicMute)
        {
            audioMixer.SetFloat("MusicVolume", -30 + lastMusicVolume);
            MusicSlider.value = lastMusicVolume;
            musicMute = false;
        }
        else
        {
            lastMusicVolume = musicVolume + 30;
            MusicSlider.value = 0;
            musicVolume = -80;
            audioMixer.SetFloat("MusicVolume", musicVolume);
            musicMute = true;
        }
    }

    public void MuteMaster()
    {
        if (masterMute)
        {
            audioMixer.SetFloat("MasterVolume", -30 + lastMasterVolume);
            MasterSlider.value = lastMasterVolume;
            masterMute = false;
        }
        else
        {
            lastMasterVolume = masterVolume + 30;
            MasterSlider.value = 0;
            masterVolume = -80;
            audioMixer.SetFloat("MasterVolume", masterVolume);
            masterMute = true;
        }
    }

    public void MuteSFX()
    {
        if (sfxMute)
        {
            audioMixer.SetFloat("SFXVolume", -30 + lastSfxVolume);
            SFXSlider.value = lastSfxVolume;
            sfxMute = false;
        }
        else
        {
            lastSfxVolume = sfxVolume + 30;
            SFXSlider.value = 0;
            sfxVolume = -80;
            audioMixer.SetFloat("SFXVolume", sfxVolume);
            sfxMute = true;
        }
    }

    #endregion METHODS
}