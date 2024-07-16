using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShower : MonoBehaviour
{
    [SerializeField] Ease easeIn;
    [SerializeField] Ease easeOut;
    public void Show()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).SetEase(easeIn);
    }

    public void UnShow()
    {
        transform.DOScale(Vector3.zero, 0.2f).SetEase(easeOut).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

 
}
