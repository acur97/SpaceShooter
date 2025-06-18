using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Gameplay/Settings", order = 0)]
public class GameplayScriptable : ScriptableObject
{
    [Header("Screen Properties Percentages")]
#if UNITY_EDITOR
    public bool debugEditorLimits = false;
#endif
    public Vector4 innerLimit = Vector4.one;
    public Vector4 playerLimit = Vector4.one;
    public float enemyLine = 0f;
    public Vector4 bulletLimit = Vector4.one;
    public Vector4 boundsLimit = Vector4.one;

    [Header("Player Explosion Feedback")]
    public float forceTime = 0.1f;
    public float force = 10;

    [Header("Common Values")]
    [TextArea(10, 20)] public string wNew;
    public int coinValue = 5;
    public uint numberOfAdRevivals = 1;
    public int numberOfCoinsRevivals = 100;

    [Header("Normal Level Type")]
    public uint playerHealth = 20;

    [Header("Infinite Level Type")]
    public uint playerHealthInfinite = 40;
    public float timeScaleIncrease = 0.1f;
    public float scoreScaleIncrease = 2f;

    [Header("Normal Levels")]
    public LevelScriptable[] levels;
    [Header("Ininite Levels (Only 1)")]
    public LevelScriptable[] infiniteLevels;

    [Header("Borders")]
    public ShipScriptable bordersShip;
    public float timeInBorder;
    public float borderLimit;
    [HideInInspector] public float countBorders = 0f;

    [Header("Customs")]
    public List<ShipScriptable> customs;
    public ShipScriptable selectedCustoms;

    [Header("Power Ups")]
    public List<PowerUpBase> powerUps;
    public PowerUpBase selectedPowerUp;

    [Header("Player progress")]
    public PlayerProgress.Player_Progress progress;
}