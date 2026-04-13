using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSManager : Singleton<BGSManager>
{
    private AudioSource audioSource;
    // TODO: 定义AudioClip
    private AudioClip click, flap, sing, kada, di;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;

        // TODO: 从Resources/Music/BGS里加载AudioClip
        click = Resources.Load<AudioClip>("Music/BGS/click");
        flap = Resources.Load<AudioClip>("Music/BGS/flap");
        sing = Resources.Load<AudioClip>("Music/BGS/sing");
        kada = Resources.Load<AudioClip>("Music/BGS/kada");
        di = Resources.Load<AudioClip>("Music/BGS/di");
    }

    public void Play(string name)
    {
        if (name == null || name == "") return;

        // TODO: 根据name选择要播放的BGM
        switch (name)
        {
            case "click":
                audioSource.clip = click;
                break;
            case "flap":
                audioSource.clip = flap;
                break;
            case "sing":
                audioSource.clip = sing;
                break;
            case "kada":
                audioSource.clip = kada;
                break;
            case "di":
                audioSource.clip = di;
                break;
        }

        audioSource.Play();
    }
}
