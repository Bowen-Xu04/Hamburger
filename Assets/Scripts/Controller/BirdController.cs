//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    MeshRenderer meshRenderer;
    //Material material;

    public int birdID;
    bool caught;
    public bool Caught => caught;
    BirdDetail birdDetail;
    public BirdDetail BirdDetail => birdDetail;

    // Start is called before the first frame update
    void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        //material = skinnedMeshRenderer.material;
    }

    public void SetHighlight(bool isHighlight)
    {
        //skinnedMeshRenderer.renderingLayerMask ^= 2;
        if (isHighlight)
        {
            //print("!!!");
            meshRenderer.renderingLayerMask |= 2;
        }
        else
        {
            //print("...");
            meshRenderer.renderingLayerMask &= uint.MaxValue - 2;
        }
    }

    public void Respond() // 准心对应的场景空间中的坐标
    {
        print("小鸟" + birdID.ToString() + "回应了");
        UIManager.Instance.ShowPaw(true, birdID);
    }

    public void Catch()
    {
        //BirdManager.Instance.
        RoomManager.Instance.VacatePosition(birdDetail.roomID, birdDetail.roomPositionID);
        BirdManager.Instance.CaughtBird();
        print(birdID);
        print(birdDetail.appearance);
        print(GameManager.Instance.NowLevelDetail.birdAppearance[birdID]);
        caught = true;
        gameObject.SetActive(false);
        UIManager.Instance.CaughtBird(birdDetail.appearance, birdID);
    }

    // 当开始或重新开始关卡时调用
    public void Reset(int appearance, int roomID)
    {
        caught = false;
        gameObject.SetActive(true);
        SetHighlight(false);

        birdDetail = new BirdDetail(appearance, roomID)
        {
            roomPositionID = RoomManager.Instance.ChooseRoomPositionID(roomID)
        };
        transform.position = RoomManager.Instance.RoomDetails[roomID].roomPositionDetail.positions[birdDetail.roomPositionID].position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Random.Range(0f, 360f));
    }

    // 当鸟逃跑时调用
    public void Flee()
    {
        int newRoomID = Random.Range(0, GameManager.Instance.NowLevel + 1);
        while (!RoomManager.Instance.CheckRoomAvailable(newRoomID))
        {
            newRoomID = Random.Range(0, GameManager.Instance.NowLevel + 1);
        }

        //RoomPositionDetail rpd = RoomManager.Instance.RoomDetails[newRoomID].roomPositionDetail;
        int newRoomPositionID = RoomManager.Instance.ChooseRoomPositionID(newRoomID);
        //print("选择新ID：" + newRoomPositionID.ToString());
        RoomManager.Instance.VacatePosition(birdDetail.roomID, birdDetail.roomPositionID);
        birdDetail.roomID = newRoomID;
        birdDetail.roomPositionID = newRoomPositionID;
        birdDetail.nowTolerance = birdDetail.maxTolerance;
        transform.position = RoomManager.Instance.RoomDetails[newRoomID].roomPositionDetail.positions[newRoomPositionID].position;
    }
}
