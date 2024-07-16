using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSfx : MonoBehaviour
{
    [SerializeField] Sprite sfxOn;
    [SerializeField] Sprite sfxOff;
    Image sfxImg;
    bool toggled = false;
    const string SFX = "SFX";
    void Start()
    {
        sfxImg = GetComponent<Image>();
        float vol = AudioManager.Ins.SFXVolume;
        if (vol != 0)
        {
            Toggle(true);
        }
        else{
            Toggle(false);
        }
    }
    public void Toggle()
    {
        Toggle(!toggled);
    }
    public void Toggle(bool toggle)
    {
        toggled = toggle;
        sfxImg.sprite = toggle ? sfxOn : sfxOff;
        AudioManager.Ins.ToggleSfx(toggle);

    }
}
