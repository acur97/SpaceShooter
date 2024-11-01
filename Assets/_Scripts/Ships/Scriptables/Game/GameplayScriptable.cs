using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Gameplay/Settings", order = 0)]
public class GameplayScriptable : ScriptableObject
{
    [Header("Values")]
    public int coinValue = 5;

    [Header("Player Explosion Feedback")]
    public float forceTime = 0.1f;
    public float force = 10;
}