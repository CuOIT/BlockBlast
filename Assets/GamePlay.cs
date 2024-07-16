using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : Singleton<GamePlay>
{
    public bool isPause;

    private enum GameMode
    {
        CLASSIC,
        TIMER,
        BLAST,
    }
    [SerializeField] GameMode gameMode;
    [SerializeField] GameProcess gameProcess;

    [SerializeField] AudioClip loseSfx;

    public void OnNoBlockToPlace()
    {
        EndGame();
    }
    public void OnBlockPlace()
    {
        gameProcess.Save(gameMode.ToString());

    }

    private void Start()
    {
        Init();
        
        Invoke("Startgame", 0.1f);
    }
    public virtual void Init()
    {
        LoadGame();
    }

    public virtual void StartGame()
    {
    }
    public void PlayNewGame()
    {
        ResetGameState();
        LoadGame();

    }

    public void SaveGame()
    {
        gameProcess.Save(gameMode.ToString());
    }
    public void LoadGame()
    {
        gameProcess.Load(gameMode.ToString());
    }
    public void ResetGameState()
    {
        gameProcess.ResetAll(gameMode.ToString());
    }

    public virtual void EndGame()
    {
        AudioManager.Ins.PlaySFX(loseSfx);
    }


}
