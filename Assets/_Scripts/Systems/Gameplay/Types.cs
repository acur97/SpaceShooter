using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public class Tags
{
    public const string Enemy = "Enemy";
    public const string Bullet = "Bullet";
    public const string Collectable = "Collectable";
    public const string Ad = "Ad";
}

[Preserve]
public class Layers
{
    public const int Player = 6;
    public const int Enemy = 7;
    public const int Bullet = 8;
    public const int Collectable = 9;
    public const int PowerUp = 10;
}

[Preserve]
public class MaterialProperties
{
    public static readonly int Color = Shader.PropertyToID("_Color");
    public static readonly int ColorCapsule = Shader.PropertyToID("_ColorCapsule");

    public static readonly int Offset = Shader.PropertyToID("_Offset");

    public static readonly int Invert = Shader.PropertyToID("_invert");
    public static readonly int Frecuency = Shader.PropertyToID("_frecuency");
}

[Preserve]
public class UiCommonTexts
{
    public const string Free = "free";
    public const string Owned = "owned";
}

[Preserve]
public class Inputs
{
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
    public const string Fire = "Fire1";
    public const string Jump = "Jump";
    public const string Cancel = "Cancel";
    public const string Pause = "Pause";
}

[Preserve]
public class MixerParameters
{
    public const string MasterVolume = "MasterVolume";
    public const string MusicPitch = "MusicPitch";
}

[Preserve]
public class Enums
{
    public enum SpawnType
    {
        Random,
        RandomPoint,
        Center,
        AllAtOnce,
        Specific
    }

    public enum Behaviour
    {
        Linear,
        Direct,
        Waves,
        WavesDirect,
        Diagonal,
        Wave8,
        Borders,
        Chase
    }

    public enum Attack
    {
        Continuous,
        ContinuousDouble,
        None
    }

    public enum SourceType
    {
        Main,
        MainLoop,
        PowerUps,
        PowerUpsLoop
    }

    public enum AudioType
    {
        Boom,
        Coin,
        Start,
        End,
        Zap
    }
}