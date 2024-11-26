using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Gameplay/Settings", order = 0)]
public class GameplayScriptable : ScriptableObject
{
    [Header("Normal Level Type")]
    public int coinValue = 5;
    public uint playerHealth = 20;

    [Header("Infinite Level Type")]
    public uint playerHealthInfinite = 40;
    public float timeScaleIncrease = 0.1f;
    public float scoreScaleIncrease = 2f;

    [Header("Player Explosion Feedback")]
    public float forceTime = 0.1f;
    public float force = 10;

    [Header("Vibrations")]
    public int vibrationDeath = 500;

    [Header("Levels (only level 0 enabled for now)")]
    public LevelScriptable[] levels;
    public LevelScriptable[] infiniteLevels;

    [Header("Customs")]
    public List<ShipScriptable> customs;
    public ShipScriptable selectedCustoms;

    [Header("Power Ups")]
    public List<PowerUpBase> powerUps;
    public PowerUpBase selectedPowerUp;

    [Header("Player progress")]
    public PlayerProgress.Player_Progress progress;
}