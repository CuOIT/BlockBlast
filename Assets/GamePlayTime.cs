using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayTime : GamePlay
{
    public void OnTimeup()
    {
        EndGame();
    }
}
