using UnityEngine;

[CreateAssetMenu(fileName = "Ship ", menuName = "Gameplay/Level design/Ship", order = 2)]
public class ShipScriptable : ScriptableObject
{
    // Custom
    public Color color = Color.white;

    // Ship Parameters
    public Sprite sprite;
    public Material material;
    public float speed = 4;
    public int health = 100;

    // Bullet Parameters
    public float bulletSpeed = 10;
    public float coolDown = 0.1f;
    [Space]
    public bool _spaceCoolDown = false;
    public float spaceCooldown = -1;

    // Enemy
    public enum Attack { continuous, continuousDouble, none }
    public Attack attack;
    public bool enemyCollision = true;
    public int deathScore = 8;
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