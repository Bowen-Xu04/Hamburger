using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public struct LevelDetail
{

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
    public int totalLevel, nowLevel;

    public PlayerStats_SO playerStats;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        onGame = false;
        SceneManager.sceneLoaded += OnSceneLoaded;

        totalLevel = 3;
        nowLevel = 0;
        levels = new LevelDetail[totalLevel];

        playerStats = Resources.Load<PlayerStats_SO>("Data/Data");

        // TODO: 设置关卡数据信息（LevelDetail）
    }

    void Update()
    {
        InputDetect();
    }

    void InputDetect()
    {
        if (onGame == true && Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame(false);
            SceneManager.LoadScene("Choose");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name.Contains("Level") && onGame == false)
        {
            StartGame(SceneManager.GetActiveScene().name[5] - '1');
        }
    }

    public void StartGame(int level)
    {
        nowLevel = level;
        onGame = true;
        onPause = false;

        Application.targetFrameRate = 60;

#if !UNITY_EDITOR
        Cursor.visible = false;
#endif

        BGMManager.Instance.Play((nowLevel + 1).ToString());
        UIManager.Instance.StartGame();

        // TODO: 执行关卡开始时事件（如讲述剧情等）
        switch (nowLevel)
        {
            case 0:
                break;
        }
    }

    public void EndGame(bool finished)
    {
        onPause = false;
        onGame = false;
        UIManager.Instance.EndGame();

#if !UNITY_EDITOR
        Cursor.visible = true;
#endif

        if (finished == true)
        {
            playerStats.progress = Math.Max(playerStats.progress, nowLevel + 2);
            SaveManager.Instance.SaveProgress();
        }
        else
        {
            BGSManager.Instance.Play(escapeBGSName);
        }

        BGMManager.Instance.Stop();
        SceneManager.LoadScene("Choose");
    }

    public void SetMessage(string msg)
    {
        UIManager.Instance.SetMessage(msg);
    }

    void LevelWin()
    {

    }

    public void SetPauseState(bool state, bool playBGS = true)
    {
        onPause = state;
#if !UNITY_EDITOR
    Cursor.visible = state;
#endif
        if (playBGS == true)
        {
            BGSManager.Instance.Play(escapeBGSName);
        }
        UIManager.Instance.SetPauseState(state);
    }

    public void OpenHelpWindow(bool state, bool playBGS = true)
    {
        onOpenHelpWindow = state;
#if !UNITY_EDITOR
    Cursor.visible = state;
#endif
        if (playBGS == true)
        {
            BGSManager.Instance.Play(escapeBGSName);
        }
        UIManager.Instance.OpenHelpWindow(state);
    }

    public int GetNowLevel()
    {
        return nowLevel;
    }
}