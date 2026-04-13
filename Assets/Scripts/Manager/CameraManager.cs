using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using TMPro;
using System;

public class CameraManager : Singleton<CameraManager>
{
    static int totalCamera;
    static int currentCameraID;

    Transform cameraRoot;
    GameObject[] cameras;
    static CameraController[] cameraControllers;
    static public CameraController CurrentCameraController => cameraControllers[currentCameraID];
    static public Camera CurrentCamera => cameraControllers[currentCameraID].Camera;

    //TMP_Text currentCameraIDText, currentCameraModeText;

    protected override void Awake()
    {
        base.Awake();

        cameraRoot = GameObject.Find("CameraRoot").transform;
        totalCamera = cameraRoot.childCount;
        //print(totalCamera);

        cameras = new GameObject[totalCamera];
        cameraControllers = new CameraController[totalCamera];
        for (int i = 0; i < totalCamera; i++)
        {
            cameras[i] = cameraRoot.GetChild(i).GetChild(0).gameObject;
            //print(cameras[i].name);
            cameraControllers[i] = cameras[i].GetComponent<CameraController>();
            //cameraControllers[i].Activate(false);
        }

        //Initialize();
        // currentCameraIDText = GameObject.Find("CurrentCameraID").GetComponent<TMP_Text>();
        // currentCameraModeText = GameObject.Find("CurrentCameraMode").GetComponent<TMP_Text>();
        //print(totalCamera);
    }

    // public void fff()
    // {

    // }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        //print(totalCamera);
        int nowTotalCamera = RoomManager.Instance.RoomDetails[GameManager.Instance.NowLevel].cameraIDOffset +
                             RoomManager.Instance.RoomDetails[GameManager.Instance.NowLevel].cameraNum;
        //print("hhhhhhhhhhhhhhh");
        for (int i = 0; i < totalCamera; i++)
        {
            //cameraControllers[i].ID = i;
            cameraControllers[i].ResetZoominMode();
            cameraControllers[i].ID = i;
            Vector2Int IDs = RoomManager.Instance.GetRoomIDAndRoomCameraID(i);
            cameraControllers[i].roomID = IDs.x;
            cameraControllers[i].roomCameraID = IDs.y;
            //print(i.ToString() + ' ' + cameraControllers[i].roomID.ToString() + ' ' + cameraControllers[i].roomCameraID.ToString());
            //print(cameraControllers[i].roomID);
            if (i == 0)
            {
                currentCameraID = i;
                cameraControllers[currentCameraID].Activate(true);
                //currentCameraIDText.text = "当前相机编号：" + i.ToString();
                //print(i);
            }
            else
            {
                cameraControllers[i].Activate(false);
            }
        }
    }

    public void SwitchCamera(int roomID, int roomCameraID)
    {
        if (roomID < 0 || roomID > GameManager.Instance.NowLevel)
        {
            Debug.Log("Invalid roomID.");
            return;
        }

        if (roomCameraID < 0 || roomCameraID > RoomManager.Instance.RoomDetails[roomID].cameraNum)
        {
            print(roomCameraID);
            print(RoomManager.Instance.RoomDetails[roomID].cameraNum);
            Debug.Log("Invalid roomCameraID.");
        }

        int cameraID = RoomManager.Instance.RoomDetails[roomID].cameraIDOffset + roomCameraID;
        if (cameraID == currentCameraID)
        {
            Debug.Log("Same cameraID.");
            return;
        }

        cameraControllers[currentCameraID].Activate(false);
        cameraControllers[cameraID].Activate(true);

        //activeCamera = cameraControllers[cameraID].cam;
        //currentCameraIDText.text = "当前相机编号：" + cameraID.ToString();
        currentCameraID = cameraID;
        UIManager.Instance.SwitchRoomCamera(roomCameraID);
    }

    public Vector3 GetCrossHairPosition()
    {
        Ray ray = CurrentCamera.ScreenPointToRay(UIManager.CrossHairScreenPosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider != null)
        {
            return hitInfo.point;
        }
        else
        {
            Debug.Log("No collision.");
            return Vector3.zero;
        }
    }

    // public void SwitchCameraMode(bool isInZoominMode)
    // {
    //     //currentCameraModeText.text = "相机模式：" + (isInZoominMode ? "放大模式" : "全屏模式");
    // }
}
