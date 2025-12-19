using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Lives : MonoBehaviour
{
    #region Singleton
    private static Lives _instance;
    public static Lives Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        else
            _instance = this;

    }

    #endregion Singleton

    public TextMeshProUGUI textHeart;
    public TextMeshProUGUI textTimer;
    public DateTime nextHeartTime;
    private DateTime lastAddedTime;
    private int restoreDuration = 600;
    public bool restoring = false;

    private void Start()
    {
        Load();
        StartCoroutine(RestoreRoutine());
    }

    public void LoseHeart()
    {
        if (Game.Instance._userDatas.heart + 1 == Game.Instance._userDatas.maxheart)
        {
            nextHeartTime = AddDuration(DateTime.Now, restoreDuration);
        }
        StartCoroutine(RestoreRoutine());
    }  

    public IEnumerator RestoreRoutine()
    {
        UpdateTimer();
        UpdateHeart();
        restoring = true;
        while(Game.Instance._userDatas.heart < Game.Instance._userDatas.maxheart)
        {
            DateTime currentTime = DateTime.Now;
            DateTime counter = nextHeartTime;
            bool isAdding = false;
            while(currentTime > counter)
            {
                if (Game.Instance._userDatas.heart < Game.Instance._userDatas.maxheart)
                {
                    isAdding = true;
                    Game.Instance._userDatas.heart++;                   
                    DateTime timeToAdd = lastAddedTime > counter ? lastAddedTime : counter;
                    counter = AddDuration(timeToAdd, restoreDuration);
                    Game.Instance.SaveData();
                }
                else
                    break;
            }
            if (isAdding)
            {
                lastAddedTime = DateTime.Now;
                nextHeartTime = counter;
            }

            UpdateTimer();
            UpdateHeart();
            Save();
            yield return null;
        }
        restoring = false;
    }

    public void Update()
    {
        UpdateTimer();
        UpdateHeart();
    }

    private void UpdateTimer()
    {
        if (Game.Instance._userDatas.heart >= Game.Instance._userDatas.maxheart)
        {
            textTimer.text = "Full";
            return;
        }
            TimeSpan t = nextHeartTime - DateTime.Now;
            string value = String.Format("{0}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
            textTimer.text = value;    
    }

    public void UpdateHeart()
    {
        textHeart.text = Game.Instance._userDatas.heart.ToString();
    }

    public DateTime AddDuration(DateTime time, int duration)
    {
        return time.AddSeconds(duration);
    }

    public void Load()
    {
        nextHeartTime = StringToDate(PlayerPrefs.GetString("nextHeartTime"));
        lastAddedTime = StringToDate(PlayerPrefs.GetString("lastAddedTime"));
    }

    public void Save()
    {
        PlayerPrefs.SetString("nextHeartTime", nextHeartTime.ToString());
        PlayerPrefs.SetString("lastAddedTime", lastAddedTime.ToString());
    }

    private DateTime StringToDate(string date)
    {
        if (String.IsNullOrEmpty(date))
            return DateTime.Now;
        return DateTime.Parse(date);
    }

    public DateTime Getdatetime()
    {
        return DateTime.Now;
    }    
}
