using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGMManager : Singleton<BGMManager>
{
    //private float volume = 0.5f;
    private AudioSource audioSource;
    // TODO: 定义AudioClip
    private AudioClip bgm;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0.5f;

        // TODO: 从Resources/Music/BGM里加载AudioClip
        bgm = Resources.Load<AudioClip>("Music/BGM/bgm");
    }

    public void Play(string name)
    {
        if (name == null || name == "") return;

        // TODO: 根据name选择要播放的BGM
        switch (name)
        {
            case "bgm":
                audioSource.clip = bgm;
                audioSource.volume = 1f;
                break;
        }

        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
