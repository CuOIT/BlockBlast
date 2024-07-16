using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="ColorDic",menuName ="ColorDic")]
public class ColorDic : ScriptableObject
{
    [SerializeField] List<Sprite> sprites;

    public Sprite GetSprite(int index)
    {
        if(index<0 || index >= sprites.Count)
        {
            return sprites[0];
        }
        else
        {
            return sprites[index];
        }
    }
    public int GetRandomColor()
    {
        return Random.Range(1, sprites.Count-1);
    }
}
