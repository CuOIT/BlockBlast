using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Ins
    {
        get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    instance = go.AddComponent<T>();
                    go.name = typeof(T).Name;
                }
            }
            return instance;
        }
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

}
