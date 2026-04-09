using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : Singleton<UIManager>
{
    private Image messageImage;
    private GameObject pauseGamePanel, helpPanel;

    private readonly float messageElapseTime = 2, messageAppearRate = 0.002f, messageDisappearRate = 0.01f;
    private readonly int finalOpacity;
    private float messageNowElapseTime;
    private bool endAppear;
    private bool startDisappear;
    private int nowOpacity;

    private Action OnDisappeared = null;

    void OnEnable()
    {

    }

    void FixedUpdate()
    {
        RenewMessage();
    }

    // TODO: 刷新信息
    void RenewMessage()
    {
        if (endAppear == true && startDisappear == false)
        {
            messageNowElapseTime += Time.fixedDeltaTime;
            if (messageNowElapseTime > messageElapseTime)
            {
                startDisappear = true;
                // TODO: 执行信息开始消失时事件
            }
        }
    }

    public void StartGame()
    {

    }

    public void EndGame()
    {

    }

    public void SetMessage(string message, Action _OnDisappeared = null)
    {

    }

    public void SetPauseState(bool state)
    {
        pauseGamePanel.SetActive(state);
    }

    public void OpenHelpWindow(bool state)
    {
        helpPanel.SetActive(state);
    }

    // TODO: 设置canvasGroup的不透明度
    void SetAlpha(CanvasGroup canvasGroup, float alpha)
    {

    }

    // TODO：信息出现
    IEnumerator Appear(CanvasGroup canvasGroup, int beginTransparency)
    {
        nowOpacity = beginTransparency;

        do
        {
            nowOpacity++;
            SetAlpha(canvasGroup, nowOpacity / 100f);
            yield return new WaitForSeconds(messageAppearRate);
        } while (nowOpacity < finalOpacity);

        endAppear = true;
        yield return null;
    }

    // TODO: 信息消失
    IEnumerator Disappear(CanvasGroup canvasGroup)
    {
        do
        {
            nowOpacity--;
            SetAlpha(canvasGroup, nowOpacity / 100f);
            yield return new WaitForSeconds(messageDisappearRate);
        } while (nowOpacity > 0);

        messageImage.enabled = false;
        startDisappear = false;
        endAppear = false;
        OnDisappeared?.Invoke();
        yield return null;
    }
}
