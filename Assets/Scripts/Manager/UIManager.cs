using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class UIManager : Singleton<UIManager>
{
    static readonly float showTime = 1.5f;
    bool hasMsg = false, hasEx = false, hasPaw = false;
    float msgExistingTime = 0, exExistingTime = 0, pawExistingTime = 0;
    int pawBirdID;

    static readonly int caughtMsgCnt = 3;
    static readonly string[] caughtMsg = { "抓到我了", "算你厉害", "被你发现了" };

    private Transform backgroundImage, map;
    private GameObject pauseGamePanel, helpPanel, endGamePanel, cameraVoicePanel, maskPanel, messagePanel;
    private GameObject victory, defeat;
    private Transform[] rooms;
    private Transform paw;
    private Transform camera1ImageTransform, camera2ImageTransform, voiceImageTransform;
    public Transform VoiceImageTransform => voiceImageTransform;
    private RectTransform voiceCDRectTransform;
    private Transform messagePanelImageTransform;
    private TMP_Text messagePanelText, remainingBirdText;

    private Transform chooseLevelImage;


    private Transform startGameImage;


    private Transform about;

    //private Transform crossHair;
    static public Vector3 CrossHairScreenPosition => new(Screen.width / 2, Screen.height / 2, 0);
    static public float MaskRadius = 422;

    protected override void Awake()
    {
        base.Awake();

        backgroundImage = GameObject.Find("Canvas").transform.Find("BackgroundImage");

        // Transform[] tt = canvasBackgroundImage.GetComponentsInChildren<Transform>();
        // print(tt.Length);
        // foreach (var transform in tt)
        // {
        //     print(transform.gameObject.name);
        // }
        //print(canvasBackgroundImage.Find("PauseGamePanel") != null);
        maskPanel = backgroundImage.Find("MaskPanel").gameObject;
        pauseGamePanel = backgroundImage.Find("PauseGamePanel").gameObject;
        helpPanel = backgroundImage.Find("HelpPanel").gameObject;
        endGamePanel = backgroundImage.Find("EndGamePanel").gameObject;
        cameraVoicePanel = backgroundImage.Find("CameraVoicePanel").gameObject;
        messagePanel = backgroundImage.Find("MessagePanel").gameObject;
        map = backgroundImage.Find("Map");
        remainingBirdText = backgroundImage.Find("SmallMenu").Find("RemainingBirdText").GetComponent<TMP_Text>();

        paw = maskPanel.transform.Find("Paw");
        camera1ImageTransform = cameraVoicePanel.transform.Find("Camera1");
        camera2ImageTransform = cameraVoicePanel.transform.Find("Camera2");
        voiceImageTransform = cameraVoicePanel.transform.Find("Voice");
        voiceCDRectTransform = voiceImageTransform.GetChild(0).GetComponent<RectTransform>();
        rooms = new Transform[] { map.GetChild(0), map.GetChild(1), map.GetChild(2), map.GetChild(3) };
        messagePanelImageTransform = messagePanel.transform.Find("Image");
        messagePanelText = messagePanel.transform.Find("Text").GetComponent<TMP_Text>();
        victory = endGamePanel.transform.GetChild(0).gameObject;
        defeat = endGamePanel.transform.GetChild(1).gameObject;

        paw.GetComponent<Image>().enabled = false;
        maskPanel.SetActive(false);
        pauseGamePanel.SetActive(false);
        helpPanel.SetActive(false);
        victory.SetActive(false);
        defeat.SetActive(false);
        messagePanel.SetActive(false);

        startGameImage = GameObject.Find("Canvas").transform.Find("StartGameImage");
        chooseLevelImage = GameObject.Find("Canvas").transform.Find("ChooseLevelImage");
        about = startGameImage.Find("AboutI");
        about.gameObject.SetActive(false);
    }

    void Start()
    {
        startGameImage.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(false);
        chooseLevelImage.gameObject.SetActive(false);
        BGMManager.Instance.Play("bgm");
    }

    void Update()
    {
        RenewMessage();
        RenewEx();
        RenewPaw();
    }

    void RenewMessage()
    {
        if (hasMsg)
        {
            msgExistingTime += Time.deltaTime;
            if (msgExistingTime >= showTime)
            {
                hasMsg = false;
                msgExistingTime = 0;
                messagePanel.SetActive(false);
                print("msg end");
            }
        }
    }

    void RenewEx()
    {
        if (hasEx)
        {
            exExistingTime += Time.deltaTime;
            if (exExistingTime >= showTime)
            {
                hasEx = false;
                exExistingTime = 0;
                ShowEx(0);
            }
        }
    }

    void RenewPaw()
    {
        if (hasPaw)
        {
            pawExistingTime += Time.deltaTime;
            if (pawExistingTime >= showTime)
            {
                hasPaw = false;
                pawExistingTime = 0;
                ShowPaw(false);
            }
            else
            {
                ShowPaw(true, pawBirdID);
            }
        }
    }

    public void StartGame(int nowLevel)
    {
        // pauseGamePanel.SetActive(false);
        // helpPanel.SetActive(false);
        victory.SetActive(false);
        defeat.SetActive(false);
        messagePanel.SetActive(false);

        rooms[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/map/Ui_0_2");
        rooms[0].GetChild(0).GetComponent<Image>().enabled = false;
        for (int i = 1; i <= nowLevel; i++)
        {
            rooms[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/map/Ui_" + i.ToString() + "_1");
            rooms[i].GetChild(0).GetComponent<Image>().enabled = false;
            Color color = rooms[i].GetComponent<Image>().color;
            color.a = 1f;
            rooms[i].GetComponent<Image>().color = color;
        }
        for (int i = nowLevel + 1; i < RoomManager.totalRoom; i++)
        {
            rooms[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/map/Ui_" + i.ToString() + "_1");
            rooms[i].GetChild(0).GetComponent<Image>().enabled = false;
            Color color = rooms[i].GetComponent<Image>().color;
            color.a = 0.25f;
            rooms[i].GetComponent<Image>().color = color;
        }
    }

    public void EndGame()
    {
        victory.SetActive(false);
        defeat.SetActive(false);
        messagePanel.SetActive(false);
        chooseLevelImage.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(false);
    }

    public void SetPauseState(bool state)
    {
        pauseGamePanel.SetActive(state);
    }

    public void OpenHelpWindow(bool state)
    {
        helpPanel.SetActive(state);
    }

    public void LevelEnd(bool win)
    {
        if (win)
        {
            victory.SetActive(true);
        }
        else
        {
            //print("dshdsjhdsjhsd");
            defeat.SetActive(true);
            int rd = UnityEngine.Random.Range(0, BirdDetail.totalAppearance);
            defeat.transform.Find("DfBgImg").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/fail/Ui_b" + (rd + 1).ToString() + '_');
        }
    }

    public bool Win()
    {
        return victory.activeSelf;
    }

    public void SwitchRoom(int roomID)
    {
        print("SwitchRoom");
        print(roomID);
        for (int i = 0; i <= GameManager.Instance.NowLevel; i++)
        {
            rooms[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/map/Ui_" + i.ToString() + "_2");
        }
        rooms[roomID].GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/map/Ui_" + roomID.ToString() + "_1");

        if (RoomManager.Instance.RoomDetails[roomID].cameraNum == 1)
        {
            camera2ImageTransform.GetComponent<Image>().enabled = false;
        }
        else
        {
            camera2ImageTransform.GetComponent<Image>().enabled = true;
        }
    }

    public void ShowEx(int num)
    {
        for (int i = 0; i < RoomManager.totalRoom; i++)
        {
            rooms[i].GetChild(0).GetComponent<Image>().enabled = Convert.ToBoolean(num & (1 << i));
        }

        if (num > 0)
        {
            hasEx = true;
        }
    }

    float CalculateAngle()
    {
        Camera cam = CameraManager.CurrentCamera;
        Vector2 screenPos = (Vector2)(cam.WorldToScreenPoint(BirdManager.Instance.BirdControllers[pawBirdID].transform.position) - CrossHairScreenPosition);

        // if (screenPos.x == 0)
        // {
        //     return screenPos.y >= 0 ? 90 : 270;
        // }

        float angle = Mathf.Atan2(screenPos.y, screenPos.x) * 180 / Mathf.PI;
        return angle;
    }

    public void ShowPaw(bool state, int birdID = -1)
    {
        pawBirdID = birdID;
        paw.GetComponent<Image>().enabled = state;
        hasPaw = state;
        if (state)
        {
            paw.rotation = Quaternion.Euler(0, 0, CalculateAngle());
            Color color = paw.GetComponent<Image>().color;
            color.a = Mathf.Max(1, BirdManager.Instance.CalculateCosAngle(birdID) * 0.5f + 0.6f);
            paw.GetComponent<Image>().color = color;
        }
    }

    public void SetRemainingBird(int remainingBird, int totalBird)
    {
        remainingBirdText.text = remainingBird.ToString() + '/' + totalBird.ToString();
    }

    public void ShowMessage(int birdAppearance, string msg)
    {
        //print(msg);
        messagePanelImageTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/mini/Ui_b" + (birdAppearance + 1).ToString() + "_mini");
        messagePanelText.text = msg;
        messagePanel.SetActive(true);

        if (!hasMsg)
        {
            hasMsg = true;
        }
        else
        {
            msgExistingTime = 0;
        }
    }

    public void SetVoiceCD(float percent)
    {
        if (percent == 1)
        {
            voiceImageTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Ui_Vo2");
            voiceCDRectTransform.sizeDelta = new(voiceCDRectTransform.sizeDelta.x, 0);
        }
        else if (percent == 0)
        {
            voiceImageTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Ui_Vo1");
            voiceCDRectTransform.sizeDelta = new(voiceCDRectTransform.sizeDelta.x, 0);
        }
        else
        {
            voiceCDRectTransform.sizeDelta = new(voiceCDRectTransform.sizeDelta.x, 49 * percent);
        }
    }

    public void SwitchMode(bool isInZoominMode)
    {
        if (hasPaw)
        {
            hasPaw = false;
            pawExistingTime = 0;
        }
        //ShowPaw(false);
        maskPanel.SetActive(isInZoominMode);
    }

    public void SwitchRoomCamera(int roomCameraID)
    {
        //print(roomCameraID);
        if (roomCameraID == 0)
        {
            camera1ImageTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/cam/UI_Ca1_1");
            camera2ImageTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/cam/UI_Ca2_2");
        }
        else
        {
            camera1ImageTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/cam/UI_Ca1_2");
            camera2ImageTransform.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/cam/UI_Ca2_1");
        }

        if (hasPaw)
        {
            hasPaw = false;
            pawExistingTime = 0;
            ShowPaw(false);
        }
    }

    public void CaughtBird(int birdAppearance, int birdID)
    {
        //string msg = "抓住了" + birdID.ToString();
        string msg = caughtMsg[UnityEngine.Random.Range(0, caughtMsgCnt)];
        if (hasPaw == true && birdID == pawBirdID)
        {
            //print("!!!");
            ShowPaw(false);
            pawExistingTime = 0;
        }
        ShowMessage(birdAppearance, msg);
    }

    public void StartGame()
    {
        print("hhh");
        startGameImage.gameObject.SetActive(false);
        chooseLevelImage.gameObject.SetActive(true);
        BGSManager.Instance.Play("click");
    }

    public void ChooseLevel(int level)
    {
        chooseLevelImage.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(true);
        GameManager.Instance.StartGame(level, false);
        BGSManager.Instance.Play("click");
    }

    public void Quit()
    {
        BGSManager.Instance.Play("click");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public bool OnAbout()
    {
        return about.gameObject.activeSelf;
    }

    public void About(bool state)
    {
        about.gameObject.SetActive(state);
        BGSManager.Instance.Play("click");
    }
}
