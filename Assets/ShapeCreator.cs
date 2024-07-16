using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public class ShapeCreator : MonoBehaviour
{
    [ShowInInspector]
    public GameObject BoxPrefab;
    [SerializeField] int boxSize;
    [Range(1, 10)]
    public int size = 5; // Board size, adjustable in the inspector

    [HideInInspector]
    public bool[,] checkboxes;
    public void OnValidate()
    {
        if (checkboxes == null || checkboxes.GetLength(0) != size || checkboxes.GetLength(1) != size)
        {
            checkboxes = new bool[size, size];
        }
    }
    [Button]
    public void CreateShape()
    {
        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        int maxWidth = 0;
        int maxHeight = 0;
        for(int i=0; i < size; i++)
        {
            for(int j=0; j < size; j++)
            {
                if (checkboxes[i,j])
                {
                    maxHeight = Mathf.Max(maxHeight, i);
                    maxWidth = Mathf.Max(maxWidth, j);
                    GameObject box = (GameObject)PrefabUtility.InstantiatePrefab(BoxPrefab, transform);
                    RectTransform rect = box.GetComponent<RectTransform>();
                    rect.anchoredPosition=new Vector2(j,i)*boxSize;
                    box.name = "BOX-" + j + "-" + i;
                }
            }
        }
        GetComponent<RectTransform>().anchoredPosition =  new Vector2(maxWidth, maxHeight)*-boxSize/2;
    }
}


[CustomEditor(typeof(ShapeCreator))]
public class ShapeCreatorEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        ShapeCreator ShapeCreator = (ShapeCreator)target;
        ShapeCreator.OnValidate();
        DrawDefaultInspector();

        if (ShapeCreator.checkboxes != null)
        {
            for (int i = 0; i < ShapeCreator.size; i++)
            {
                int reverse = ShapeCreator.size - i-1;
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < ShapeCreator.size; j++)
                {
                    ShapeCreator.checkboxes[reverse, j] = EditorGUILayout.Toggle(ShapeCreator.checkboxes[reverse, j], GUILayout.Width(20));
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
