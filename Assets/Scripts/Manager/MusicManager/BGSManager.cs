using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSManager : Singleton<BGSManager>
{
    //private float volume = 0.5f;
    private AudioSource audioSource;
    // TODO: 定义AudioClip
    private AudioClip click, flap, sing, kada, di, _catch, win, lose;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.volume = 0.5f;

        // TODO: 从Resources/Music/BGS里加载AudioClip
        click = Resources.Load<AudioClip>("Music/BGS/click");
        flap = Resources.Load<AudioClip>("Music/BGS/flap");
        sing = Resources.Load<AudioClip>("Music/BGS/sing");
        kada = Resources.Load<AudioClip>("Music/BGS/kada");
        di = Resources.Load<AudioClip>("Music/BGS/di");
        _catch = Resources.Load<AudioClip>("Music/BGS/catch");
        win = Resources.Load<AudioClip>("Music/BGS/win");
        lose = Resources.Load<AudioClip>("Music/BGS/lose");
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
            case "catch":
                audioSource.clip = _catch;
                break;
            case "win":
                audioSource.clip = win;
                break;
            case "lose":
                audioSource.clip = lose;
                break;
        }

        audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        //volume += delta;
        audioSource.volume = volume;
    }
}
