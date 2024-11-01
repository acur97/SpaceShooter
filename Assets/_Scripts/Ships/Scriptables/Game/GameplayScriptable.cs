using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Gameplay/Settings", order = -1)]
public class GameplayScriptable : ScriptableObject
{
    [Header("Values")]
    public int coinValue = 5;
}