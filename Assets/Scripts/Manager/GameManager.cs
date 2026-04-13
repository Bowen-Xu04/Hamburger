using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public struct LevelDetail
{
    public int time;
    public int totalBird;
    public float[] posProb;
    //public int[] maxToleranceByRoom;
    public int[] birdOriginalRoomID;
    public int[] birdAppearance;
}

public class GameManager : Singleton<GameManager>
{
    private static bool onGame, onPause, onOpenHelpWindow;
    readonly string escapeBGSName;

    public static bool OnGame
    {
        get { return onGame; }
    }

    public static bool OnPause
    {
        get { return onPause; }
    }

    public static bool OnOpenHelpWindow
    {
        get { return onOpenHelpWindow; }
    }

    LevelDetail[] levels;
    public LevelDetail[] Levels => levels;
    readonly static public int totalLevel = 4;
    private int nowLevel;
    public int NowLevel => nowLevel;
    public LevelDetail NowLevelDetail => levels[nowLevel];

    public PlayerStats_SO playerStats;
    VoiceController voiceController;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        onGame = false;
        //SceneManager.sceneLoaded += OnSceneLoaded;

        playerStats = Resources.Load<PlayerStats_SO>("Data/Data");
        //winText = GameObject.Find("Win").GetComponent<TMP_Text>();
        //winText.enabled = false;

        // TODO: 设置关卡数据信息（LevelDetail）
        nowLevel = 0;
        levels = new LevelDetail[totalLevel];

        levels[0].totalBird = 2;
        levels[0].time = 90;
        levels[0].posProb = new float[] { 0.7f, 0.3f, 0.0f };
        levels[0].birdOriginalRoomID = new int[] { 0, 0 };
        levels[0].birdAppearance = new int[] { 0, 1 };

        levels[1].totalBird = 4;
        levels[1].time = 120;
        levels[1].posProb = new float[] { 0.4f, 0.4f, 0.2f };
        levels[1].birdOriginalRoomID = new int[] { 0, 0, 1, 1 };
        levels[1].birdAppearance = new int[] { 0, 1, 0, 1 };

        levels[2].totalBird = 6;
        levels[2].time = 150;
        levels[2].posProb = new float[] { 0.3f, 0.4f, 0.3f };
        levels[2].birdOriginalRoomID = new int[] { 0, 0, 1, 1, 2, 2 };
        levels[2].birdAppearance = new int[] { 0, 1, 2, 3, 0, 2 };

        levels[3].totalBird = 8;
        levels[3].time = 180;
        levels[3].posProb = new float[] { 0.2f, 0.3f, 0.5f };
        levels[3].birdOriginalRoomID = new int[] { 0, 0, 1, 1, 2, 2, 3, 3 };
        levels[3].birdAppearance = new int[] { 0, 1, 2, 3, 0, 2, 1, 3 };
        // levels[3].birdDetails = new BirdDetail[levels[3].totalBird];
        // levels[3].birdDetails[0] = new BirdDetail(0, 0);
        // levels[3].birdDetails[1] = new BirdDetail(0, 0);
        // levels[3].birdDetails[2] = new BirdDetail(0, 0);
        // levels[3].birdDetails[3] = new BirdDetail(0, 0);

        // for (int i = 0; i < totalLevel; i++)
        // {
        //     levels[i].birdAppearance = new int[levels[i].totalBird];
        //     for (int j = 0; j < levels[i].totalBird; j++)
        //     {
        //         levels[i].birdAppearance[j] = UnityEngine.Random.Range(0, BirdDetail.totalAppearance);
        //     }
        // }
    }

    void Update()
    {
        InputDetect();
    }

    void InputDetect()
    {
        if (onGame == true)
        {
            if (OnOpenHelpWindow == false && Input.GetKeyDown(KeyCode.Escape) == true)
            {
                SetPauseState(!onPause);
            }
            if (onPause == false && Input.GetKeyDown(KeyCode.H) == true)
            {
                OpenHelpWindow(!onOpenHelpWindow);
            }

            if (onPause == false && OnOpenHelpWindow == false && Input.GetKeyDown(KeyCode.Space) == true)
            {
                //print("t1");
                voiceController.ButtonClick();
                //print("t2");
            }
        }
        else
        {
            if (UIManager.Instance.Win() && Input.GetKeyDown(KeyCode.Return))
            {
                if (nowLevel < totalLevel)
                {
                    StartGame(nowLevel + 1, false);
                }
                else
                {
                    EndGame(true);
                }
            }
            else if (UIManager.Instance.OnAbout() && Input.GetKeyDown(KeyCode.Return))
            {
                UIManager.Instance.About(false);
            }
        }
    }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     // if (SceneManager.GetActiveScene().name.Contains("Level") && onGame == false)
    //     // {
    //     //     //StartGame(SceneManager.GetActiveScene().name[5] - '1', false);
    //     //     StartGame(0, false);
    //     // }
    // }

    public void StartGame(int level, bool reset)
    {
        nowLevel = level;
        onGame = true;
        SetPauseState(false);
        OpenHelpWindow(false);
        MouseManager.Instance.Activate(true);

        //BGMManager.Instance.Play((nowLevel + 1).ToString());
        UIManager.Instance.StartGame(level);
        CameraManager.Instance.Initialize();
        RoomManager.Instance.Initialize();
        TimerManager.Instance.StartTiming(levels[nowLevel].time);
        voiceController = UIManager.Instance.VoiceImageTransform.GetComponent<VoiceController>();
        //voiceController = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Voice").GetComponent<VoiceController>();

        // TODO: 执行关卡开始时事件（如讲述剧情等）
        if (!reset)
        {
            BirdManager.Instance.StartGame(nowLevel, levels[nowLevel].totalBird);
        }
        else
        {
            BirdManager.Instance.ResetBirds();
        }

        if (level == 0)
        {
            OpenHelpWindow(true);
        }

        //levels[nowLevel].remainingBird = levels[nowLevel].totalBird;
        // switch (nowLevel)
        // {
        //     case 0:
        //         break;
        // }
    }

    public void RestartGame()
    {
        StartGame(nowLevel, true);
    }

    public void EndGame(bool finished)
    {
        onPause = false;
        onGame = false;
        UIManager.Instance.EndGame();
        MouseManager.Instance.Activate(false);

        if (finished == true)
        {
            //playerStats.progress = Math.Max(playerStats.progress, nowLevel + 2);
            //SaveManager.Instance.SaveProgress();
        }
        else
        {
            //BGSManager.Instance.Play(escapeBGSName);
        }

        //BGMManager.Instance.Stop();
        //SceneManager.LoadScene("Choose");
    }

    public void LevelWin()
    {
        onGame = false;
        UIManager.Instance.LevelEnd(true);
        //winText.enabled = true;
    }

    public void LevelLose()
    {
        onGame = false;
        UIManager.Instance.LevelEnd(false);
    }

    public void SetPauseState(bool state)
    {
        onPause = state;
        TimerManager.Instance.SetTimingState(!state);
        MouseManager.Instance.Activate(!state);
        //BGSManager.Instance.Play(escapeBGSName); 
        UIManager.Instance.SetPauseState(state);
    }

    public void SetPauseStateButton()
    {
        if (onPause == false && onOpenHelpWindow == false)
        {
            SetPauseState(true);
        }
    }

    public void OpenHelpWindow(bool state)
    {
        onOpenHelpWindow = state;
        TimerManager.Instance.SetTimingState(!state);
        MouseManager.Instance.Activate(!state);
        //BGSManager.Instance.Play(escapeBGSName);
        UIManager.Instance.OpenHelpWindow(state);
    }

    public void OpenHelpWindowButton()
    {
        if (onPause == false && onOpenHelpWindow == false)
        {
            OpenHelpWindow(true);
        }
    }

    public void SwitchRoom(int roomID)
    {
        if (roomID <= nowLevel)
        {
            RoomManager.Instance.SwitchRoom(roomID);
        }
        else
        {
            print("Room locked.");
        }
    }

    public void SwitchRoomCamera(int roomCameraID)
    {
        CameraManager.Instance.SwitchCamera(RoomManager.CurrentRoomID, roomCameraID);
    }
}