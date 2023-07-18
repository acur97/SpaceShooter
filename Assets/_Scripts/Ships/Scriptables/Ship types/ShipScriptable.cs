using UnityEngine;

[CreateAssetMenu(fileName = "Ship", menuName = "ScriptableObjects/Ship properties", order = 0)]
public class ShipScriptable : ScriptableObject
{
    [Header("Custom")]
    [ColorUsage(false, true)] public Color color;

    [Header("Parameters")]
    public float speed = 4;
    public float bulletSpeed = 10;
    public float coolDown = 0.1f;

    [Space]
    public float health = 100;

    [Header("Enemy")]
    public Sprite sprite;
    //public enum behaviour { li}
}