using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int type; // easy, normal, hard
    public Vector3 position; // rotation随机
}

public class RoomPositionDetail
{
    readonly static public int totalPosDiff = 3;

    public int totalPosNum;
    public int[] posNum = new int[totalPosDiff];
    public int[] avPosNum = new int[totalPosDiff];
    //public Transform[] positionBalls;
    public Position[] positions;
    public bool[] occupied;

    public RoomPositionDetail(int _totalPosNum)
    {
        totalPosNum = _totalPosNum;
        //positionBalls = new Transform[_totalPosNum];
        positions = new Position[_totalPosNum];
        occupied = new bool[_totalPosNum];
    }
}

public class RoomDetail // 不随关卡发生变化的信息
{
    public int cameraNum, cameraIDOffset;
    public int[] cameraID;
    public RoomPositionDetail roomPositionDetail;
}

public class RoomManager : Singleton<RoomManager>
{
    readonly static public int totalRoom = 4;
    static int currentRoomID;
    static public int CurrentRoomID => currentRoomID;

    Transform positionBallRoot;
    RoomDetail[] roomDetails;
    public RoomDetail[] RoomDetails => roomDetails;

    protected override void Awake()
    {
        base.Awake();

        // 设置房间信息
        roomDetails = new RoomDetail[totalRoom];

        roomDetails[0] = new()
        {
            cameraNum = 2,
            cameraIDOffset = 0,
            cameraID = new int[] { 0, 1 }
        };
        roomDetails[1] = new()
        {
            cameraNum = 1,
            cameraIDOffset = 2,
            cameraID = new int[] { 2 }
        };
        roomDetails[2] = new()
        {
            cameraNum = 2,
            cameraIDOffset = 3,
            cameraID = new int[] { 3, 4 }
        };
        roomDetails[3] = new()
        {
            cameraNum = 2,
            cameraIDOffset = 5,
            cameraID = new int[] { 5, 6 }
        };

        currentRoomID = 0;

        InitializeRoomPositions();
    }

    public void Initialize()
    {
        ResetRoomPositions();
        UIManager.Instance.SwitchRoom(0);
        UIManager.Instance.SwitchRoomCamera(0);
        //SwitchRoom(0);
    }

    void InitializeRoomPositions()
    {
        positionBallRoot = GameObject.Find("PositionBallRoot").transform;

        for (int i = 0; i < totalRoom; i++)
        {
            Transform positionBallRoomRoot = positionBallRoot.GetChild(i);
            roomDetails[i].roomPositionDetail = new RoomPositionDetail(positionBallRoomRoot.childCount);
            RoomPositionDetail rpd = roomDetails[i].roomPositionDetail;

            for (int j = 0; j < positionBallRoomRoot.childCount; j++)
            {
                Transform positionBall = positionBallRoomRoot.GetChild(j);
                //rpd.positionBalls[j] = positionBall;
                //roomDetails[i].roomPositionDetail.positions[j] = positionBall.position;

                if (positionBall.name.Contains("blue"))
                {
                    rpd.posNum[0]++;
                }
                else if (positionBall.name.Contains("yellow"))
                {
                    rpd.posNum[1]++;
                }
                else if (positionBall.name.Contains("red"))
                {
                    rpd.posNum[2]++;
                }
                else
                {
                    Debug.Log("Unknown difficulty type.");
                }
            }

            int ez = 0, nm = 0, hd = 0;
            for (int j = 0; j < positionBallRoomRoot.childCount; j++)
            {
                Transform positionBall = positionBallRoomRoot.GetChild(j);

                if (positionBall.name.Contains("blue"))
                {
                    rpd.positions[ez] = new()
                    {
                        type = 0,
                        position = positionBall.position
                    };
                    ez++;
                }
                else if (positionBall.name.Contains("yellow"))
                {
                    rpd.positions[rpd.posNum[0] + nm] = new()
                    {
                        type = 1,
                        position = positionBall.position
                    };
                    nm++;
                }
                else
                {
                    rpd.positions[rpd.posNum[0] + rpd.posNum[1] + hd] = new()
                    {
                        type = 2,
                        position = positionBall.position
                    };
                    hd++;
                }

                positionBall.gameObject.SetActive(false);
            }

            print("###=====================");
            print(i);
            print(rpd.totalPosNum);
            print(string.Join(' ', rpd.posNum));
        }

        ResetRoomPositions();
    }

    // 当开始或重新开始关卡时调用
    public void ResetRoomPositions()
    {
        for (int i = 0; i < totalRoom; i++)
        {
            RoomPositionDetail rpd = roomDetails[i].roomPositionDetail;

            for (int j = 0; j < RoomPositionDetail.totalPosDiff; j++)
            {
                rpd.avPosNum[j] = rpd.posNum[j];
            }

            System.Array.Clear(rpd.occupied, 0, rpd.totalPosNum);
        }
    }

    public void SwitchRoom(int roomID)
    {
        if (roomID < 0 || roomID > GameManager.Instance.NowLevel)
        {
            Debug.Log("Invalid roomID.");
            return;
        }

        if (roomID == currentRoomID)
        {
            Debug.Log("Same roomID.");
            return;
        }

        UIManager.Instance.SwitchRoom(roomID);
        CameraManager.Instance.SwitchCamera(roomID, 0);
        currentRoomID = roomID;
    }

    public Vector2Int GetRoomIDAndRoomCameraID(int cameraID)
    {
        //print("================" + cameraID.ToString());
        for (int i = 0; i <= GameManager.Instance.NowLevel; i++)
        {
            for (int j = 0; j < roomDetails[i].cameraNum; j++)
            {
                if (roomDetails[i].cameraID[j] == cameraID)
                {
                    return new(i, j);
                }
            }
        }

        Debug.Log("Invalid cameraID.");
        return Vector2Int.zero;
    }

    public bool CheckRoomAvailable(int roomID)
    {
        bool av = false;
        for (int i = 0; i < RoomPositionDetail.totalPosDiff; i++)
        {
            av |= roomDetails[roomID].roomPositionDetail.avPosNum[i] > 0;
        }

        return av;
    }

    public int ChooseRoomPositionID(int roomID)
    {
        RoomPositionDetail rpd = roomDetails[roomID].roomPositionDetail;
        LevelDetail nld = GameManager.Instance.NowLevelDetail;

        float pos;
        int type, offset;

        while (true)
        {
            pos = Random.value;
            if (pos <= nld.posProb[0] && rpd.avPosNum[0] > 0)
            {
                type = 0;
                offset = 0;
                break;
            }
            else if (pos > nld.posProb[0] && pos <= nld.posProb[0] + nld.posProb[1] && rpd.avPosNum[1] > 0)
            {
                type = 1;
                offset = rpd.posNum[0];
                break;
            }
            else if (pos > nld.posProb[0] + nld.posProb[1] && rpd.avPosNum[2] > 0)
            {
                type = 2;
                offset = rpd.posNum[0] + rpd.posNum[1];
                break;
            }
        }

        int pos1 = Random.Range(0, rpd.avPosNum[type]), nowPos;

        for (nowPos = 0; nowPos < rpd.posNum[type]; nowPos++)
        {
            if (!rpd.occupied[offset + nowPos])
            {
                pos1--;
                if (pos1 < 0)
                {
                    break;
                }
            }
        }

        rpd.avPosNum[type]--;
        //print();
        rpd.occupied[offset + nowPos] = true;
        return offset + nowPos;
    }

    public void VacatePosition(int roomID, int roomPositionID)
    {
        RoomPositionDetail rpd = roomDetails[roomID].roomPositionDetail;
        if (roomPositionID < 0 || roomPositionID > rpd.totalPosNum)
        {
            Debug.Log("Invalid roomPositionID");
        }

        if (rpd.occupied[roomPositionID] == false)
        {
            Debug.Log("Already vacant");
        }

        rpd.occupied[roomPositionID] = false;
        rpd.avPosNum[rpd.positions[roomPositionID].type]++;
    }
}
