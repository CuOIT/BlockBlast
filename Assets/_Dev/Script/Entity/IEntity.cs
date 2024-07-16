using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity 
{
    public void LoadProcess(string gameMode);
    public void SaveProcess(string gameMode);
    public void ResetProcess(string gameMode);
}
