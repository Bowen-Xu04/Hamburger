using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEditor;

public class BirdDetail
{
    readonly static public int totalAppearance = 4;
    readonly static public int[] maxToleranceByAppearance = { 2, 3, 4, 5 };

    public int appearance, maxTolerance, nowTolerance; //positionID;
    public int roomID, roomPositionID;

    public BirdDetail(int _appearance, int originalRoom)
    {
        appearance = _appearance;
        roomID = originalRoom;
        maxTolerance = nowTolerance = maxToleranceByAppearance[_appearance];
    }
}

public class BirdManager : Singleton<BirdManager>
{
    int nowLevel;
    int totalBird, remainingBird;
    //TMP_Text remainingBirdText;

    Transform birdRoot;
    GameObject[] birds;
    BirdController[] birdControllers;
    public BirdController[] BirdControllers => birdControllers;

    protected override void Awake()
    {
        base.Awake();
        birdRoot = GameObject.Find("Birds").transform;
    }

    public void StartGame(int nowLevel, int totalBird)
    {
        this.nowLevel = nowLevel;
        this.totalBird = totalBird;
        //remainingBirdText = GameObject.Find("RemainingBird").GetComponent<TMP_Text>();

        //InitializePositions(nowLevel);
        InitializeBirds();
    }

    public void InitializeBirds()
    {
        birds = new GameObject[totalBird];
        birdControllers = new BirdController[totalBird];
        for (int i = 0; i < totalBird; i++)
        {
            birds[i] = Instantiate(Resources.Load<GameObject>("Prefabs/Birds/Char_Bird" + (GameManager.Instance.NowLevelDetail.birdAppearance[i] + 1).ToString()));
            birds[i].tag = "BirdBody";
            birds[i].AddComponent<MeshCollider>();
            birds[i].transform.parent = birdRoot;
            birdControllers[i] = birds[i].AddComponent<BirdController>();
            // print(birds[i].transform.position);
            // print(birds[i].name);
            // print(birdControllers[i] != null);
            //birdControllers[i].Reset();
        }

        ResetBirds();
    }

    public void CaughtBird()
    {
        remainingBird--;
        UIManager.Instance.SetRemainingBird(remainingBird, totalBird);
        if (remainingBird == 0)
        {
            GameManager.Instance.LevelWin();
        }
    }

    // 当开始或重新开始关卡时调用
    public void ResetBirds()
    {
        for (int i = 0; i < totalBird; i++)
        {
            birdControllers[i].birdID = i;
            print(i);
            print(birdControllers[i] != null);
            birdControllers[i].Reset(GameManager.Instance.NowLevelDetail.birdAppearance[i], GameManager.Instance.NowLevelDetail.birdOriginalRoomID[i]);
        }

        remainingBird = totalBird;
        UIManager.Instance.SetRemainingBird(remainingBird, totalBird);
    }

    public float CalculateCosAngle(int birdID)
    {
        return Vector3.Dot(CameraManager.CurrentCameraController.transform.forward,
                            Vector3.Normalize(birdControllers[birdID].transform.position - CameraManager.CurrentCameraController.transform.position));
    }

    int GetNearestBirdID()
    {
        int nearestBirdID = -1;
        float maxCosAngle = -2;
        for (int i = 0; i < totalBird; i++)
        {
            if (birdControllers[i].Caught == false &&
                birdControllers[i].BirdDetail.roomID == CameraManager.CurrentCameraController.roomID)
            {
                float cosAngle = CalculateCosAngle(i);
                if (cosAngle > maxCosAngle)
                {
                    nearestBirdID = i;
                    maxCosAngle = cosAngle;
                }
            }
        }

        return nearestBirdID;
    }

    public void Voice()
    {
        print("Voice=====================");
        int birdFleeNum = 0, fleeBirdAppearance = 0;
        if (CameraManager.CurrentCameraController.IsInZoominMode)
        {
            //Vector3 point = CameraManager.Instance.GetCrossHairPosition();
            // if (nearestBirdID != -1)
            // {
            //     birdControllers[nearestBirdID].Respond(point);
            // }

            int nearestBirdID;

            while (true)
            {
                nearestBirdID = GetNearestBirdID();

                if (nearestBirdID != -1)
                {
                    birdControllers[nearestBirdID].BirdDetail.nowTolerance--;
                    print("放大模式：小鸟" + nearestBirdID.ToString() + "回应  nowTolerance=" + birdControllers[nearestBirdID].BirdDetail.nowTolerance.ToString());
                    if (birdControllers[nearestBirdID].BirdDetail.nowTolerance == 0)
                    {
                        print("小鸟" + nearestBirdID.ToString() + "飞走");
                        birdFleeNum++;
                        fleeBirdAppearance = birdControllers[nearestBirdID].BirdDetail.appearance;
                        birdControllers[nearestBirdID].Flee();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            print("放大模式发声");
            if (nearestBirdID != -1)
            {
                birdControllers[nearestBirdID].Respond();
                BGSManager.Instance.Play("sing");
            }
            else
            {
                print("无小鸟回应");
                BGSManager.Instance.Play("di");
            }
        }
        else
        {
            int num = 0;

            for (int i = 0; i < totalBird; i++)
            {
                if (birdControllers[i].Caught)
                {
                    continue;
                }

                birdControllers[i].BirdDetail.nowTolerance--;
                print("全屏模式：小鸟" + i.ToString() + "回应  nowTolerance=" + birdControllers[i].BirdDetail.nowTolerance.ToString());
                if (birdControllers[i].BirdDetail.nowTolerance == 0)
                {
                    print("小鸟" + i.ToString() + "飞走");
                    birdFleeNum++;
                    fleeBirdAppearance = birdControllers[i].BirdDetail.appearance;
                    birdControllers[i].Flee();
                }

                num |= 1 << birdControllers[i].BirdDetail.roomID;
            }

            print("全屏模式发声");
            UIManager.Instance.ShowEx(num);
            BGSManager.Instance.Play("di");
        }

        if (birdFleeNum != 0)
        {
            string msg = birdFleeNum.ToString() + "只小鸟貌似不耐烦地飞走了";
            UIManager.Instance.ShowMessage(fleeBirdAppearance, msg);
            BGSManager.Instance.Play("flap");
        }

        for (int i = 0; i < 4; i++)
        {
            RoomPositionDetail rpd = RoomManager.Instance.RoomDetails[i].roomPositionDetail;
            print("-----------------");
            print(i);
            print(rpd.totalPosNum);
            print(string.Join(' ', rpd.posNum));
            print(string.Join(' ', rpd.avPosNum));
            print(string.Join(' ', rpd.occupied));
        }
    }
}
