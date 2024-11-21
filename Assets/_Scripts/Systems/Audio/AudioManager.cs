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

    public enum AudioType
    {
        Boom,
        Coin,
        Start,
        End,
        Zap
    }

    public enum SourceType
    {
        Main,
        Main_Loop,
        PowerUps,
        PowerUps_Loop
    }

    public void Init()
    {
        Instance = this;
    }

    public void PlaySound(AudioType type, float volume = 1f)
    {
        switch (type)
        {
            case AudioType.Boom:
                PlaySound(clipBoom, volume);
                break;

            case AudioType.Coin:
                PlaySound(clipCoin, volume);
                break;

            case AudioType.Start:
                PlaySound(clipStart, volume);
                break;

            case AudioType.End:
                PlaySound(clipEnd, volume);
                break;

            case AudioType.Zap:
                PlaySound(clipZap, volume);
                break;
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f, SourceType sourceType = SourceType.Main)
    {
        switch (sourceType)
        {
            case SourceType.Main:
                source.PlayOneShot(clip, volume);
                break;

            case SourceType.PowerUps:
                sourcePowerUps.PlayOneShot(clip, volume);
                break;
        }
    }

    public void PlaySoundLoop(AudioClip clip, float volume = 1f, SourceType sourceType = SourceType.Main)
    {
        switch (sourceType)
        {
            case SourceType.Main_Loop:
                sourceLoop.clip = clip;
                sourceLoop.volume = volume;
                sourceLoop.Play();
                break;

            case SourceType.PowerUps_Loop:
                sourcePowerUpsLoop.clip = clip;
                sourcePowerUpsLoop.volume = volume;
                sourcePowerUpsLoop.Play();
                break;
        }
    }

    public async UniTaskVoid PlaySoundLoopDelayed(AudioClip clip, float delay, float volume = 1f, SourceType sourceType = SourceType.Main)
    {
        await UniTask.WaitForSeconds(delay);

        switch (sourceType)
        {
            case SourceType.Main_Loop:
                sourceLoop.clip = clip;
                sourceLoop.volume = volume;
                sourceLoop.Play();
                break;

            case SourceType.PowerUps_Loop:
                sourcePowerUpsLoop.clip = clip;
                sourcePowerUpsLoop.volume = volume;
                sourcePowerUpsLoop.Play();
                break;
        }
    }

    public void StopSource(SourceType sourceType)
    {
        switch (sourceType)
        {
            case SourceType.Main:
                source.Stop();
                break;

            case SourceType.Main_Loop:
                sourceLoop.Stop();
                break;

            case SourceType.PowerUps:
                sourcePowerUps.Stop();
                break;

            case SourceType.PowerUps_Loop:
                sourcePowerUpsLoop.Stop();
                break;
        }
    }

    public void PlayUiClip()
    {
        PlaySound(clip_ui);
    }
}