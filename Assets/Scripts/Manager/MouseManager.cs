using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MouseManager : Singleton<MouseManager>
{
    bool isActive;

    //public EventVector3 OnMouseClick;
    RaycastHit hitInfo;
    BirdController highlightBird;

    protected override void Awake()
    {
        base.Awake();
        isActive = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //print(isActive);
        if (isActive)
        {
            MouseControl();
        }
    }

    public void Activate(bool isActive)
    {
        this.isActive = isActive;
    }

    void MouseControl()
    {
        //print(CameraManager.ActiveCameraController.IsInZoominMode);
        if (CameraManager.CurrentCameraController.IsInZoominMode == false ||
            Vector3.Distance(Input.mousePosition, UIManager.CrossHairScreenPosition) > UIManager.MaskRadius)
        {
            highlightBird?.SetHighlight(false);
            highlightBird = null;
            return;
        }

        Ray ray = CameraManager.CurrentCamera.ScreenPointToRay(Input.mousePosition);

        // if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider != null)
        // {
        //     print(hitInfo.collider.gameObject.name);
        // }

        if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider != null && hitInfo.collider.gameObject.CompareTag("BirdBody"))
        {

            //print("!!!");
            //if (hitInfo.collider.gameObject.CompareTag("Bird"))

            BirdController birdController = hitInfo.collider.GetComponent<BirdController>();

            if (highlightBird == null)
            {
                highlightBird = birdController;
                highlightBird.SetHighlight(true);
            }
            else if (highlightBird != birdController)
            {
                highlightBird.SetHighlight(false);
                highlightBird = birdController;
                highlightBird.SetHighlight(true);
            }

            if (Input.GetMouseButtonDown(0))
            {
                highlightBird.Catch();
            }

        }
        else
        {
            highlightBird?.SetHighlight(false);
            highlightBird = null;
        }
    }
}