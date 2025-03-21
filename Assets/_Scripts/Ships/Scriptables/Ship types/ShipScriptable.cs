using UnityEngine;

[CreateAssetMenu(fileName = "Ship ", menuName = "Gameplay/Level design/Ship", order = 2)]
public class ShipScriptable : ScriptableObject
{
    // Editor
    public bool editorCustom = true;
    public bool editorStore = true;
    public bool editorShip = true;
    public bool editorBullet = true;
    public bool editorEnemy = true;
    public bool editorNotes = true;
    public string notes;

    // Custom
    public Color color = Color.white;
    public new string name;
    public string description;
    public bool owned = false;
    public uint cost = 0;

    // Ship Parameters
    public Sprite sprite;
    public Vector3 spriteScale = Vector3.one;
    public float hue = 0;
    public float speed = 4;
    public int health = 100;

    // Bullet Parameters
    public float bulletSpeed = 10;
    public float fireTime = 0.5f;
    public float coolDown = 1;
    [Space]
    public bool _spaceCoolDown = false;
    public float spaceCooldown = -1;
    [Space]
    public bool _bulletTime = false;
    public float bulletTime = -1;

    // Enemy
    public Enums.Attack attack;
    public float startRotation = 0;
    public float rotationSpeed = 0;
    public bool enemyCollision = true;
    public int deathScore = 8;
    public Enums.Behaviour behaviour;
    public bool directBehaviour = false;
    public float directLine = -0.9f;
    public float behaviourMathfSin = 1.5f;
    public bool behaviourMathfInverted = false;
    [Space]
    public bool _timeToContinue = false;
    public float timeToContinue = -1;

    [Header("Internal")]
    public bool countForGroup = true;
    public int spawnIndex;
}