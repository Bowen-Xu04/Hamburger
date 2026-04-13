using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public int ID, roomID, roomCameraID;

    public bool isMainCamera = false;
    bool isActive;
    public bool IsActive => isActive;
    bool isInZoominMode;
    public bool IsInZoominMode => isInZoominMode;

    private Camera cam;
    public Camera Camera => cam;
    private AudioListener audioListener;
    public AudioListener AudioListener => audioListener;

    float rotateSpeed = 0.4f, scrollSpeed = 1.5f;
    public float minHorizontalAngle = 100, maxHorizontalAngle = 170, minVerticalAngle = 15, maxVerticalAngle = 45;
    public float minFov = 10, maxFov = 30;

    public float fullScreenFov, zoominFov = 20;
    Quaternion fullScreenRotation, zoominRotation;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<Camera>();
        print(cam != null);
        audioListener = GetComponent<AudioListener>();
        fullScreenRotation = transform.parent.transform.rotation;
        fullScreenFov = cam.fieldOfView;
        zoominRotation = fullScreenRotation;
        //print(zoominRotation.eulerAngles);
        //isActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.OnPause == false && GameManager.OnOpenHelpWindow == false && isActive == true)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SwitchMode(!isInZoominMode);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetZoominMode();
            }

            if (isInZoominMode)
            {
                Rotate();
                Scroll();
            }
        }
    }

    public void Activate(bool isActive)
    {
        this.isActive = isActive;
        print("!!!!!");
        print(cam != null);
        print("hhhhh");
        print(audioListener != null);
        cam.enabled = isActive;
        audioListener.enabled = isActive;

        if (isActive)
        {
            Reset();
        }
    }

    void Reset()
    {
        SwitchMode(false);
    }

    public void ResetZoominMode()
    {
        zoominFov = fullScreenFov;
        zoominRotation = fullScreenRotation;

        if (isInZoominMode)
        {
            cam.fieldOfView = zoominFov;
            transform.parent.transform.rotation = fullScreenRotation;
        }
    }

    void SwitchMode(bool isInZoominMode)
    {
        if (this.isInZoominMode)
        {
            zoominRotation = transform.parent.transform.rotation;
            zoominFov = cam.fieldOfView;
        }

        this.isInZoominMode = isInZoominMode;
        //CameraManager.Instance.SwitchCameraMode(isInZoominMode);
        //print(ID);
        if (isInZoominMode)
        {
            // print("切换到zoom");
            // print(zoominRotation);
            // print(zoominFov);
            transform.parent.transform.rotation = zoominRotation; //Quaternion.Euler(zoominRotation.eulerAngles);
            //print(zoominRotation.eulerAngles);
            cam.fieldOfView = zoominFov;
        }
        else
        {
            // print("切换到full screen");
            // print(transform.rotation);
            // print(cam.fieldOfView);
            transform.parent.transform.rotation = fullScreenRotation;
            cam.fieldOfView = fullScreenFov;
        }

        UIManager.Instance.SwitchMode(isInZoominMode);
    }

    void Rotate()
    {
        float horizontalMove = -Convert.ToInt32(Input.GetKey(KeyCode.A)) + Convert.ToInt32(Input.GetKey(KeyCode.D));
        float verticalMove = -Convert.ToInt32(Input.GetKey(KeyCode.S)) + Convert.ToInt32(Input.GetKey(KeyCode.W));

        if (horizontalMove != 0 && verticalMove != 0)
        {
            horizontalMove *= Mathf.Sqrt(2) / 2;
            verticalMove *= Mathf.Sqrt(2) / 2;
        }

        // transform.RotateAround(transform.position, Vector3.up, rotateSpeed * horizontalMove); //设置水平环绕旋转
        // transform.RotateAround(transform.position, Vector3.right, rotateSpeed * verticalMove); //设置竖直环绕旋转              

        // if (transform.eulerAngles.x < minVerticalAngle || transform.eulerAngles.x > maxVerticalAngle)
        // {   
        //     transform.eulerAngles = ;
        // }

        // print(transform.eulerAngles);
        // print(minVerticalAngle);
        // print(maxVerticalAngle);
        // print(Mathf.Clamp(transform.eulerAngles.x, minVerticalAngle, maxVerticalAngle));
        //print(transform.parent.transform.eulerAngles);
        transform.parent.transform.eulerAngles =
            new Vector3(Mathf.Clamp(transform.parent.transform.eulerAngles.x - rotateSpeed * verticalMove, minVerticalAngle, maxVerticalAngle),
                        Mathf.Clamp(transform.parent.transform.eulerAngles.y + rotateSpeed * horizontalMove, minHorizontalAngle, maxHorizontalAngle),
                        transform.parent.transform.eulerAngles.z);
    }

    void Scroll()
    {
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, minFov, maxFov);
    }
}
