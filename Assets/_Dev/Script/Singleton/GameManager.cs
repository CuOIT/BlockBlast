using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private GamePlay _gamePlay;

    [SerializeField]List<GamePlay> gamePlays;

    [SerializeField] Transform gamePos;

    [SerializeField] GameObject lobbyScreen;

    public void PlayMode(int i)
    {
        if (i < gamePlays.Count && i >= 0)
        {
            GamePlay gamePlay = Instantiate(gamePlays[i],gamePos);
            SetGamePlay(gamePlay);
            lobbyScreen.SetActive(false);
        }
    }
    public void PauseGame()
    {
        _gamePlay.isPause=true;
    }
    public void ResumeGame()
    {
        _gamePlay.isPause = false;
    }
    public void SetGamePlay(GamePlay gamePlay)
    {
        _gamePlay = gamePlay;
    }

    public void Replay()
    {
        Loading();
        if(_gamePlay != null )
        {
            _gamePlay.PlayNewGame();
        }
        else
        {
            GamePlay.Ins.PlayNewGame();
        }
    }

    public void Loading()
    {

    }
    public void QuitGameMode()
    {
        Destroy(_gamePlay.gameObject);
        lobbyScreen.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
