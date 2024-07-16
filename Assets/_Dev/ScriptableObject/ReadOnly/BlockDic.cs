using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName ="BlockDic",menuName ="BlockDic")]
public class BlockDic : ScriptableObject
{
    [SerializeField] List<Block> blocks;

    private void OnEnable()
    {
        
    }
    public List<Block> GetBlockList()
    {
        for (int i = 0; i < blocks.Count; i++)
        {
            blocks[i].ID = i;
        }
        return blocks; 
    }
#if UNITY_EDITOR
    [Button] 
    public void InitBlock(string prefabFolderPath)
    {
        blocks.Clear();

        if (string.IsNullOrEmpty(prefabFolderPath))
        {
            Debug.LogWarning("Prefab folder path is not set!");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Debug.Log(prefab.name);
            if (prefab != null)
            {
                Block block = prefab.GetComponent<Block>();
                if(block!=null)
                {
                    blocks.Add(block);
                }
            }
        }

        Debug.Log("Prefabs loaded from folder: " + prefabFolderPath);
    }
#endif
}
