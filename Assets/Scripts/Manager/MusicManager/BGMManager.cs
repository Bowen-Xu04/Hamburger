using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGMManager : Singleton<BGMManager>
{
    private AudioSource audioSource;
    // TODO: 定义AudioClip

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        // TODO: 从Resources/Music/BGM里加载AudioClip
    }

    public void Play(string name)
    {
        if (name == null || name == "") return;

        // TODO: 根据name选择要播放的BGM
        switch (name)
        {
            case "0":
                break;
        }

        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}
