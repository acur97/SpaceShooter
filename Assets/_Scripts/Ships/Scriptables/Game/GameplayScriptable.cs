using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Gameplay/Settings", order = 0)]
public class GameplayScriptable : ScriptableObject
{
    [Header("Values")]
    public int coinValue = 5;

    [Header("Player Explosion Feedback")]
    public float forceTime = 0.1f;
    public float force = 10;

    [Header("Levels (only level 0 enabled for now)")]
    public LevelScriptable[] levels;

    [Header("Customs")]
    public List<ShipScriptable> customs;
    public ShipScriptable selectedCustoms;

    [Header("Power Ups")]
    public List<PowerUpBase> powerUps;
    public PowerUpBase selectedPowerUp;

    [Header("Player progress")]
    public PlayerProgress.Player_Progress progress;
}