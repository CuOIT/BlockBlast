using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    private const string MUSIC_VOLUME = "MUSIC_VOLUME";
    private const string SFX_VOLUME = "SFX_VOLUME";
    void Start()
    {
        musicSource.volume = GetMusicVol();
        sfxSource.volume = GetSFXVol();
    }
    private float GetMusicVol()
    {
        return Mathf.Clamp01(PlayerPrefs.GetFloat(MUSIC_VOLUME, 0.5f));
    }
    private float GetSFXVol()
    {
        return Mathf.Clamp01(PlayerPrefs.GetFloat(SFX_VOLUME, 0.5f));
    }
    public void ToggleSfx(bool toggle)
    {
        if(toggle)
        {
            sfxSource.volume = 0.5f;
            PlayerPrefs.SetFloat("SFX", 0.5f);
        }
        else
        {
            sfxSource.volume = 0;
            PlayerPrefs.SetFloat("SFX", 0f); 
        }
    }
    public float MusicVolume => GetMusicVol();
    public float SFXVolume => GetSFXVol();
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
