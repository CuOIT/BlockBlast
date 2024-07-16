using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour, IEntity
{
    [SerializeField] Slider slider;
    [SerializeField] float maxTime;

    
    private float currentTime;
    public GameObject readyGO;
    public TextMeshProUGUI delayTxt;
    const string TIME_MANAGER = "TIME_MANAGER";
    private bool isReady;
    [SerializeField] Event TimeUpEvent;
    private void Awake()
    {
        slider.maxValue = maxTime;
        isReady = false;
    }
    public void Update()
    {
        bool isPause = GamePlay.Ins.isPause;
        if ((!isPause) & isReady)
        {
            SetTime(currentTime - Time.deltaTime);
            if (currentTime <= 0)
            {
                TimeUpEvent.RaiseEvent(); 
            }
        }
    }

    public void SetTime(float time)
    {
        currentTime = Mathf.Clamp(time, 0, maxTime);
        slider.value = currentTime;
    }

    [SerializeField] int bonusMulti;
    public void OnClear(Vector2Int rowAndCol)
    {
        int bonusTime = rowAndCol.x + rowAndCol.y;
        bonusTime *= bonusMulti;
        
        SetTime(currentTime+bonusTime);
    }
    public void LoadProcess(string gameMode)
    {
        float time = PlayerPrefs.GetFloat(TIME_MANAGER+gameMode,maxTime);  
        time = Mathf.Clamp(time,0,maxTime);
        SetTime(time);
        StartCoroutine(Countdown(4));
    }

    private IEnumerator Countdown(int num)
    {
        readyGO.SetActive(true);
        while (num > 0)
        {
        delayTxt.SetText(num.ToString());
        yield return new WaitForSeconds(1);
        num--;
        }
        delayTxt.SetText("START!");
        yield return new WaitForSeconds(1);
        isReady = true;
        readyGO.SetActive(false);
    }
    public void SaveProcess(string gameMode)
    {
        PlayerPrefs.SetFloat(TIME_MANAGER+gameMode,currentTime);
    }

    public void ResetProcess(string gameMode)
    {
        isReady = false;
        SetTime(maxTime);
        SaveProcess(gameMode);
        StartCoroutine(Countdown(4));
    }
}
