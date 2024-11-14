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
        ExtractLife,
        CoinMagnet
    }

    public List<PowerUpBase> powerUps;
    //public List<PowerUpBase> currentPowerUps = new();
    public PowerUpBase selectedPowerUp;
    private PowerUpBase currentPowerUp;

    public void Init()
    {
        Instance = this;

        Player_Shoot += PlayerShoot;
        Player_Damage += PlayerDamage;
        Enemy_Damage += EnemyDamage;
        Enemy_Death += EnemyDeath;
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
        if (currentPowerUp != null)
        {
            return;
        }

        //currentPowerUps.Add(type);
        currentPowerUp = type;
        currentPowerUp.currentAmount--;
        selectedPowerUp = null;

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

        //currentPowerUps.Remove(type);
        currentPowerUp = null;

        type.Dispose();
    }

    public void PlayerShoot()
    {
        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnPlayerShoot();
        //}

        currentPowerUp?.OnPlayerShoot();
    }

    public void PlayerDamage()
    {
        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnPlayerDamage();
        //}

        currentPowerUp?.OnPlayerDamage();
    }

    public void EnemyDamage(ShipBaseController enemy)
    {
        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnEnemyDamage(enemy);
        //}

        currentPowerUp?.OnEnemyDamage(enemy);
    }

    public void EnemyDeath(ShipBaseController enemy)
    {
        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnEnemyDeath(enemy);
        //}

        currentPowerUp?.OnEnemyDeath(enemy);
    }

    private void Update()
    {
        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnGameUpdate();
        //}

        currentPowerUp?.OnGameUpdate();
    }
}