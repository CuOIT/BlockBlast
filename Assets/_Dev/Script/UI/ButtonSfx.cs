using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSfx : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    public void OnClick()
    {
        AudioManager.Ins.PlaySFX(clip);
    }
}
