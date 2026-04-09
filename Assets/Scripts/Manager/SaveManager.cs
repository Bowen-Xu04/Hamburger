using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SaveManager : Singleton<SaveManager>
{
    public static bool beginNewGame = false, canContinue = true;

    private GameObject backgroundPanel;
    private List<Transform> levelButtonList = new();

    private List<Sprite> levelImages = new();

    //public PlayerStats levelProgress;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

        levelImages.Clear();
        levelImages.Add(Resources.Load<Sprite>("Images/猫猫漫画/选关目录/关卡一-未触发"));
        levelImages.Add(Resources.Load<Sprite>("Images/猫猫漫画/选关目录/关卡一-触发"));
        levelImages.Add(Resources.Load<Sprite>("Images/猫猫漫画/选关目录/关卡二-未触发"));
        levelImages.Add(Resources.Load<Sprite>("Images/猫猫漫画/选关目录/关卡二-触发"));
        levelImages.Add(Resources.Load<Sprite>("Images/猫猫漫画/选关目录/关卡三-未触发"));
        levelImages.Add(Resources.Load<Sprite>("Images/猫猫漫画/选关目录/关卡三-触发"));
    }

    void Start()
    {

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Choose")
        {
            backgroundPanel = GameObject.Find("Canvas").transform.Find("Background Panel").gameObject;
            //print(backgroundPanel.name);
            levelButtonList.Clear();// = GameObject.Find("Level Buttons").transform.GetChild;
            LoadProgress();
        }
    }

    public void SaveProgress()
    {
        Save(GameManager.Instance.playerStats, "Progress");
    }

    public void LoadProgress()
    {
        if (beginNewGame == true)
        {
            GameManager.Instance.playerStats.progress = 1;
            Save(GameManager.Instance.playerStats, "Progress");
            beginNewGame = false;
        }

        Load(GameManager.Instance.playerStats, "Progress");

        //int nowProgress = GameManager.Instance.playerStats.progress;
        for (int i = 0; i < GameManager.Instance.totalLevel; i++)
        {
            //print(backgroundPanel.transform.Find("Level" + (i + 1).ToString()) != null);
            levelButtonList.Add(backgroundPanel.transform.Find("Level" + (i + 1).ToString()).transform);
            if (i < GameManager.Instance.playerStats.progress)
            {
                levelButtonList[i].GetComponent<Button>().enabled = true;
                levelButtonList[i].GetComponent<Image>().sprite = levelImages[2 * i + 1];
            }
            else
            {
                levelButtonList[i].GetComponent<Button>().enabled = false;
                levelButtonList[i].GetComponent<Image>().sprite = levelImages[2 * i];
            }
        }
        //text.text = "当前进度：关卡" + GameManager.Instance.playerStats.progress.ToString();
    }

    public void Save(Object data, string key)
    {
        //print(level);
        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }

    public void RefreshProgress()
    {

    }

    public void ProgressAdd1()
    {
        GameManager.Instance.playerStats.progress++;
    }
}
