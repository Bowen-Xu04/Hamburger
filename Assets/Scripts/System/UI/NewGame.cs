using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    readonly string BGSName;

    public void ButtonClick()
    {
        SaveManager.beginNewGame = true;
        BGSManager.Instance.Play(BGSName);
        SceneManager.LoadScene("Choose");
    }
}
