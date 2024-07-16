using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Box : MonoBehaviour
{
    [SerializeField] Image _img;

    private int _orginColor;
    public int OrginColor=>_orginColor;

    [SerializeField] ColorDic _colorDic;
    public void InitColor(int color)
    {
        _orginColor = color;
        SetColor(color);
    }
    public int GetRandomColor()
    {
        return _colorDic.GetRandomColor();
    }
    [SerializeField, Range(0, 1)] float opacity;
    public void Blur()
    {
        Color color = _img.color;
        color.a=opacity;
        _img.color=color;
    }

    public Vector2Int GetLocalPos()
    {
        string boxName = gameObject.name;
        string[] part = boxName.Split("-");
        if (part.Length == 3)
        {
            int x = int.Parse(part[1]);
            int y = int.Parse(part[2]);
            return new Vector2Int(x, y);
        }
        return Vector2Int.zero;
    }
    public void SetColor(int color)
    {
        _img.sprite = _colorDic.GetSprite(color);
    }
    public void ReverseToOriginColor()
    {
        SetColor(_orginColor);
    }

    [SerializeField] float x;
    [ContextMenu("TestDisappear")]
    public void Disappear()
    {
        transform.DOScale(Vector3.zero, x).SetEase(Ease.Linear).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
