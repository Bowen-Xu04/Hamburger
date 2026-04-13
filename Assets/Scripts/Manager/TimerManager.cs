using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerManager : Singleton<TimerManager>
{
    TMP_Text timeText;
    DateTime dateTime;

    bool running;
    int remainingTime;
    float time0;

    protected override void Awake()
    {
        base.Awake();

        running = false;
        timeText = GameObject.Find("TimerText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            time0 += Time.deltaTime;
            if (time0 >= 1)
            {
                time0--;
                remainingTime--;
                RenewTimeText();

                if (remainingTime == 0)
                {
                    GameManager.Instance.LevelLose();
                    SetTimingState(false);
                }
            }
        }
    }

    public void StartTiming(int time)
    {
        dateTime = DateTime.Now;

        remainingTime = time;
        time0 = 0;
        RenewTimeText();
        running = true;
    }

    public void SetTimingState(bool state)
    {
        running = state;
    }

    void RenewTimeText()
    {
        string dayText = dateTime.Year.ToString() + '-' + dateTime.Month.ToString("D2") + '-' + dateTime.Day.ToString("D2");

        string secondText;
        if (remainingTime == 0)
        {
            secondText = "12:00:00";
            GameManager.Instance.LevelLose();
        }
        else
        {
            int minute = 59 - (remainingTime - 1) / 60, second = 59 - (remainingTime - 1) % 60;
            secondText = "11:" + minute.ToString("D2") + ':' + second.ToString("D2");
        }

        timeText.text = dayText + "  " + secondText;
    }
}
