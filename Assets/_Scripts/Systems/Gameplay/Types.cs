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
    public static readonly int Player = LayerMask.GetMask("Player");
    public static readonly int Enemy = LayerMask.GetMask("Enemy");
    public static readonly int Bullet = LayerMask.GetMask("Bullet");
    public static readonly int Collectable = LayerMask.GetMask("Collectable");
    public static readonly int PowerUp = LayerMask.GetMask("PowerUp");
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
    public const string PriceFormat = "${0}";
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

    public enum TypeBullet
    {
        player,
        enemy
    }
}