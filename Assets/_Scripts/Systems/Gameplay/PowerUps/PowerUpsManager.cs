using System;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private GameplayScriptable gameplayScriptable;

    [Header("UI")]
    public Slider slider;
    [SerializeField] private Image icon;

    //public List<PowerUpBase> currentPowerUps = new();
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

    public void SelectPowerUp(PowerUpBase powerUp)
    {
        gameplayScriptable.selectedPowerUp = powerUp;

        if (powerUp == null)
        {
            icon.enabled = false;
            slider.enabled = false;
        }
        else
        {
            icon.enabled = true;
            slider.enabled = true;

            icon.sprite = powerUp.sprite;

            if (powerUp.useDuration)
            {
                slider.maxValue = powerUp.InitDuration();
                slider.value = powerUp.duration;
            }
            else
            {
                slider.maxValue = 1;
                slider.value = 1;
            }
        }
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
        gameplayScriptable.selectedPowerUp = null;

        type.shipBase = PlayerController.Instance;

        if (type.useDuration)
        {
            type.CountDuration().Forget();
        }

        type.OnActivate();

        if (type.useDuration)
        {
            slider.maxValue = type.duration;
        }
        else
        {
            slider.maxValue = 1;
        }
    }

    public void RemovePowerUp(PowerUpBase type)
    {
        type.OnDeactivate();

        //currentPowerUps.Remove(type);
        currentPowerUp = null;

        type.Dispose();

        slider.value = 0;
        icon.enabled = false;
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

        if ((currentPowerUp != null && currentPowerUp.useDuration))
        {
            slider.value = currentPowerUp.duration;
        }
    }
}