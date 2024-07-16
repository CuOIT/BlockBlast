using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : MonoBehaviour
{
    [SerializeField] List<GameObject> entityGoes;
    [SerializeField] List<IEntity> entities;


    public void Awake()
    {
        entities = new List<IEntity>();
        foreach (var entityGO in entityGoes)
        {
            IEntity entity = entityGO.GetComponent<IEntity>();
            if (entity != null) entities.Add(entity);
        }
    }

    public void Save(string gameMode)
    {
        foreach(var entity in entities)
        {
            entity.SaveProcess(gameMode);
        }
    }

    public void Load(string gameMode) { 
        foreach(var entity in entities)
        {
            entity.LoadProcess(gameMode);
        }
    }

    public void ResetAll(string gameMode)
    {
        foreach( var entity in entities)
        {
            entity.ResetProcess(gameMode);
        }
    }

}
