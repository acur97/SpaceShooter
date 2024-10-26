using UnityEngine;

[CreateAssetMenu(fileName = "Ship ", menuName = "Gameplay/Ship", order = 2)]
public class ShipScriptable : ScriptableObject
{
    [Header("==  Custom  ==")]
    /*[ColorUsage(false, true)]*/ public Color color = Color.white;

    [Header("==  Ship Parameters  ==")]
    public Sprite sprite;
    public float speed = 4;
    public int health = 100;

    [Header("==  Bullet Parameters  ==")]
    public float bulletSpeed = 10;
    public float coolDown = 0.1f;
    [Space]
    public bool _spaceCoolDown = false;
    public float spaceCooldown = -1;

    public enum Attack { continuous, continuousDouble, none }
    [Header("==  Enemy  ==")]
    public Attack attack;
    public bool enemyCollision = true;
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
    [Space]
    public bool _timeToContinue = false;
    public float timeToContinue = -1;

    [HideInInspector] public int spawnIndex;
}