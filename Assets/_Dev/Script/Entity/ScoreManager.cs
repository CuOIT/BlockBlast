using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour,IEntity
{
    [SerializeField] TextMeshProUGUI scoreTxt;
    int _score;
    [SerializeField] TextMeshProUGUI highScoreTxt;
    int _highScore;
    const string HIGHSCORE="HIGHSCORE";
    const string SCORE="SCORE";
    public void Start()
    {
        
    }

    public void SetScore(int score)
    {
        _score = score;
        scoreTxt.SetText(score.ToString());
    }

    public void SetHighScore(int score)
    {
        _highScore = score;
        highScoreTxt.SetText(score.ToString());
    }
    [SerializeField] float duration;
    IEnumerator PlayScoreAsync(int score)
    {
        float t = 0;
        int startScore = _score;
        _score = score;
        int currentScore=0;
        while (t < duration)
        {
            t += Time.deltaTime;
            currentScore = (int)Mathf.Lerp(startScore, _score,t/duration);
            scoreTxt.SetText(currentScore.ToString());
            yield return null;
        }
        SetScore(score);
    }

    public void SetScoreAsync(int score)
    {
        if(score > _highScore)
        {
            SetHighScore(score);
        }
        StartCoroutine(PlayScoreAsync(score));
    }
    public void OnPlace(int num)
    {
        SetScoreAsync(_score+num);
    }

    [SerializeField] int baseScore;
    public void OnRowOrColumnClear(Vector2Int rowAndCol)
    {
        int num = rowAndCol.x + rowAndCol.y;

        int bonus = baseScore * ((num * (num + 1)) / 2);

        SetScoreAsync(_score+bonus);
;    }

    public void LoadProcess(string gameMode)
    {
        SetScore(PlayerPrefs.GetInt(SCORE + gameMode, 0));
        SetHighScore(PlayerPrefs.GetInt(HIGHSCORE + gameMode, 0));

    }

    public void SaveProcess(string gameMode)
    {
        PlayerPrefs.SetInt(SCORE + gameMode, _score);
        PlayerPrefs.SetInt(HIGHSCORE + gameMode, _highScore);
    }

    public void ResetProcess(string gameMode)
    {
        SetScore(0);
        SaveProcess(gameMode);
    }
}
