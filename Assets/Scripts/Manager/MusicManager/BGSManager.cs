using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSManager : Singleton<BGSManager>
{
    private AudioSource audioSource;
    // TODO: 定义AudioClip

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        audioSource = gameObject.AddComponent<AudioSource>();

        // TODO: 从Resources/Music/BGS里加载AudioClip
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
}
