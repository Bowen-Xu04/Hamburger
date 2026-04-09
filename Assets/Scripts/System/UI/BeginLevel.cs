using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginLevel : MonoBehaviour
{
    public string targetLevel, BGSName;

    public void ButtonClick()
    {
        BGSManager.Instance.Play(BGSName);
        SceneManager.LoadScene(targetLevel);
    }
}
