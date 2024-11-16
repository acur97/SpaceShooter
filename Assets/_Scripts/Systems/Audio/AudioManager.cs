using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource source;

    [Space]
    [SerializeField] private AudioClip clipBoom;
    [SerializeField] private AudioClip clipCoin;
    [SerializeField] private AudioClip clipStart;
    [SerializeField] private AudioClip clipEnd;
    [SerializeField] private AudioClip clipZap;

    public enum AudioType
    {
        Boom,
        Coin,
        Start,
        End,
        Zap
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
                source.PlayOneShot(clipBoom, volume);
                break;

            case AudioType.Coin:
                source.PlayOneShot(clipCoin, volume);
                break;

            case AudioType.Start:
                source.PlayOneShot(clipStart, volume);
                break;

            case AudioType.End:
                source.PlayOneShot(clipEnd, volume);
                break;

            case AudioType.Zap:
                source.PlayOneShot(clipZap, volume);
                break;
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        source.PlayOneShot(clip, volume);
    }
}