using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public struct AudioPart
{
    public AudioClip audioClip;
    public float audioVolume;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioMixer mixer;

    [Header("Sources")]
    public AudioSource source;
    public AudioSource sourceLoop;
    public AudioSource sourcePowerUps;
    public AudioSource sourcePowerUpsLoop;

    [Header("Main Clips")]
    [SerializeField] private AudioClip clipBoom;
    [SerializeField] private AudioClip clipCoin;
    [SerializeField] private AudioClip clipStart;
    [SerializeField] private AudioClip clipEnd;
    [SerializeField] private AudioClip clipZap;

    [Header("Common Clips")]
    [SerializeField] private AudioClip clip_ui;

    public void Init()
    {
        Instance = this;

        GameManager.GameStart += LoadMainClips;
    }

    private void OnDisable()
    {
        GameManager.GameStart -= LoadMainClips;
    }

    private void LoadMainClips(bool start)
    {
        if (start)
        {
            clipBoom.LoadAudioData();
            clipCoin.LoadAudioData();
            clipStart.LoadAudioData();
            clipEnd.LoadAudioData();
            clipZap.LoadAudioData();
        }
        else
        {
            clipBoom.UnloadAudioData();
            clipCoin.UnloadAudioData();
            clipStart.UnloadAudioData();
            clipEnd.UnloadAudioData();
            clipZap.UnloadAudioData();
        }
    }

    public void PlaySound(Enums.AudioType type, float volume = 1f)
    {
        switch (type)
        {
            case Enums.AudioType.Boom:
                PlaySound(clipBoom, volume);
                break;

            case Enums.AudioType.Coin:
                PlaySound(clipCoin, volume);
                break;

            case Enums.AudioType.Start:
                PlaySound(clipStart, volume);
                break;

            case Enums.AudioType.End:
                PlaySound(clipEnd, volume);
                break;

            case Enums.AudioType.Zap:
                PlaySound(clipZap, volume);
                break;
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f, Enums.SourceType sourceType = Enums.SourceType.Main)
    {
        switch (sourceType)
        {
            case Enums.SourceType.Main:
                source.PlayOneShot(clip, volume);
                break;

            case Enums.SourceType.PowerUps:
                sourcePowerUps.PlayOneShot(clip, volume);
                break;
        }
    }

    public void PlaySoundLoop(AudioClip clip, float volume = 1f, Enums.SourceType sourceType = Enums.SourceType.Main)
    {
        switch (sourceType)
        {
            case Enums.SourceType.MainLoop:
                sourceLoop.clip = clip;
                sourceLoop.volume = volume;
                sourceLoop.Play();
                break;

            case Enums.SourceType.PowerUpsLoop:
                sourcePowerUpsLoop.clip = clip;
                sourcePowerUpsLoop.volume = volume;
                sourcePowerUpsLoop.Play();
                break;
        }
    }

    public async UniTaskVoid PlaySoundLoopDelayed(AudioClip clip, float delay, float volume = 1f, Enums.SourceType sourceType = Enums.SourceType.Main)
    {
        await UniTask.WaitForSeconds(delay);

        switch (sourceType)
        {
            case Enums.SourceType.MainLoop:
                sourceLoop.clip = clip;
                sourceLoop.volume = volume;
                sourceLoop.Play();
                break;

            case Enums.SourceType.PowerUpsLoop:
                sourcePowerUpsLoop.clip = clip;
                sourcePowerUpsLoop.volume = volume;
                sourcePowerUpsLoop.Play();
                break;
        }
    }

    public void StopSource(Enums.SourceType sourceType)
    {
        switch (sourceType)
        {
            case Enums.SourceType.Main:
                source.Stop();
                break;

            case Enums.SourceType.MainLoop:
                sourceLoop.Stop();
                break;

            case Enums.SourceType.PowerUps:
                sourcePowerUps.Stop();
                break;

            case Enums.SourceType.PowerUpsLoop:
                sourcePowerUpsLoop.Stop();
                break;
        }
    }

    public void PlayUiClip()
    {
        PlaySound(clip_ui, 0.5f);
    }
}