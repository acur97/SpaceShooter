using UnityEngine;

[CreateAssetMenu(fileName = "Ship ", menuName = "Gameplay/Ship", order = 2)]
public class ShipScriptable : ScriptableObject
{
    [Header("Custom")]
    [ColorUsage(false, true)] public Color color = Color.white;

    [Header("Ship Parameters")]
    public float speed = 4;
    public int health = 100;

    [Header("Bullet Parameters (-1 disable)")]
    public float bulletSpeed = 10;
    public float coolDown = 0.1f;
    public float spaceCooldown = -1;

    [Header("Enemy (-1 disable)")]
    public Sprite sprite;
    public enum Behaviour
    {
        linear,
        direct,
        waves,
        wavesDirect,
        diagonal,
        wave8,
        borders
    }
    public Behaviour behaviour;
    public float behaviourMathfSin = 1.5f;
    public enum Attack { continuous, continuousDouble, none }
    public Attack attack;
    public int timeToContinue = -1;

    [HideInInspector] public int spawnCount;
}