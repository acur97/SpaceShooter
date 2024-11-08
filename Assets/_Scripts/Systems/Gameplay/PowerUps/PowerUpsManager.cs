using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsManager : MonoBehaviour
{
    public static PowerUpsManager Instance;

    public static Action Player_Shoot;
    public static Action Player_Damage;
    public static Action<ShipBaseController> Enemy_Damage;
    public static Action<ShipBaseController> Enemy_Death;

    public enum PowerUpType
    {
        None,
        DoubleShot,
        TripleShot,
        Missile,
        Shield,
        ElectricField,
        TimeSlower,
        MoveFaster,
        FastFire,
        LifeSteal,
        LifeRegen,
        DroneSupport,
        DroneOrbital,
        LaserBurst,
        OrbitalLaser,
        ExtractLife
    }

    [SerializeField] private PowerUpBase powerUpStartTest;
    [SerializeField] private List<PowerUpBase> powerUps;
    public List<PowerUpBase> currentPowerUps = new();

    public void Init()
    {
        Instance = this;

        Player_Shoot += PlayerShoot;
        Player_Damage += PlayerDamage;
        Enemy_Damage += EnemyDamage;
        Enemy_Death += EnemyDeath;
    }

    public async UniTaskVoid Start()
    {
        await UniTask.WaitForSeconds(10f);

        AddPowerUp(powerUpStartTest);
    }

    private void OnDisable()
    {
        Player_Shoot -= PlayerShoot;
        Player_Damage -= PlayerDamage;
        Enemy_Damage -= EnemyDamage;
        Enemy_Death -= EnemyDeath;
    }

    public void AddPowerUp(PowerUpBase type)
    {
        currentPowerUps.Add(type);
        type.shipBase = PlayerController.Instance;

        if (type.useDuration)
        {
            type.CountDuration().Forget();
        }

        type.OnActivate();
    }

    public void RemovePowerUp(PowerUpBase type)
    {
        type.OnDeactivate();
        currentPowerUps.Remove(type);
        type.Dispose();
    }

    public void PlayerShoot()
    {
        for (int i = 0; i < currentPowerUps.Count; i++)
        {
            currentPowerUps[i].OnPlayerShoot();
        }
    }

    public void PlayerDamage()
    {
        for (int i = 0; i < currentPowerUps.Count; i++)
        {
            currentPowerUps[i].OnPlayerDamage();
        }
    }

    public void EnemyDamage(ShipBaseController enemy)
    {
        for (int i = 0; i < currentPowerUps.Count; i++)
        {
            currentPowerUps[i].OnEnemyDamage(enemy);
        }
    }

    public void EnemyDeath(ShipBaseController enemy)
    {
        for (int i = 0; i < currentPowerUps.Count; i++)
        {
            currentPowerUps[i].OnEnemyDeath(enemy);
        }
    }

    private void Update()
    {
        for (int i = 0; i < currentPowerUps.Count; i++)
        {
            currentPowerUps[i].OnGameUpdate();
        }
    }
}