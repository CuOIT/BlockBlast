using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class UIManager : Singleton<UIManager>
{
    Dictionary<Type,UICanvas> canvas =  new Dictionary<Type,UICanvas>();
    [SerializeField] Transform parent;


    public T OpenUI<T> () where T : UICanvas
    {
        T uiCanvas = GetUI<T>();
        uiCanvas.Init();
        uiCanvas.Open();
        return uiCanvas;
    }
    public void CloseUI<T>(float time = 0) where T : UICanvas
    {
        if (IsOpened<T>())
        {
            canvas[typeof(T)].Close(time);
        }
    }
    public bool IsLoaded<T>() where T : UICanvas
    {
        return canvas.ContainsKey(typeof(T)) && canvas[typeof(T)] != null;
    } 
    public bool IsOpened<T>() where T : UICanvas
    {
        return IsLoaded<T>() && canvas[typeof(T)].gameObject.activeSelf;
    }
    public T GetUIPrefab<T>() where T : UICanvas
    {
        return default(T);
    }
    public T GetUI<T>() where T : UICanvas
    {
        if (!IsLoaded<T>())
        {
            T prefab  =  GetUIPrefab<T>();
            Instantiate(prefab, parent);
            canvas[typeof(T)] = prefab;
        }
        return canvas[typeof(T)] as T;
    }

    public void CloseAll()
    {
        foreach(var canv in canvas)
        {
            if (canv.Value != null)
            {
                if (canv.Value.gameObject.activeSelf)
                {
                    canv.Value.Close();
                }
            }
        }
    }
}

public class UICanvas : MonoBehaviour
{
    public virtual void Init()
    {

    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void OnClose()
    {

    }

    private void UnActive()
    {
        gameObject.SetActive(false);
    }
    public void Close(float time = 0)
    {
        OnClose();
        if(time > 0)
        {
            Invoke(nameof(UnActive), time);
        }
        else
        {
            UnActive();
        }
    }
}