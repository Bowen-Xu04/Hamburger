using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VoiceController : MonoBehaviour
{
    readonly static float CD = 2;
    float existingTime;
    bool inCD;

    // Start is called before the first frame update
    void Start()
    {
        existingTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (inCD)
        {
            existingTime += Time.deltaTime;
            if (existingTime >= CD)
            {
                inCD = false;
                existingTime = 0;
                UIManager.Instance.SetVoiceCD(1);
            }
            else
            {
                UIManager.Instance.SetVoiceCD(existingTime / CD);
            }
        }
    }

    public void ButtonClick()
    {
        if (GameManager.OnGame == true && GameManager.OnPause == false && GameManager.OnOpenHelpWindow == 0)
        {
            if (!inCD)
            {
                print("Voice");
                BirdManager.Instance.Voice();
                inCD = true;
                UIManager.Instance.SetVoiceCD(0);
            }
            else
            {
                BGSManager.Instance.Play("kada");
            }
        }
    }
}
