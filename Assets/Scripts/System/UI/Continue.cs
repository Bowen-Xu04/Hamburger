using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Continue : MonoBehaviour
{
    readonly string BGSName;

    public void ButtonClick()
    {
        if (SaveManager.canContinue == true)
        {
            SaveManager.beginNewGame = false;
            //BGSManager.Instance.Play(BGSName);
            SceneManager.LoadScene("Choose");
        }
    }
}
