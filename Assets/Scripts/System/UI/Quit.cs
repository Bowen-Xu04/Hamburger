using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit : MonoBehaviour
{
    readonly string BGSName;

    public void ButtonClick()
    {
        //BGSManager.Instance.Play(BGSName);
        Invoke("QuitGame", 0.25f);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif        
    }
}
