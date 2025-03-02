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
    [SerializeField] private Image icon;
    private float maxValue;

    //public List<PowerUpBase> currentPowerUps = new();
    [ReadOnly] public PowerUpBase currentPowerUp;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Instance = null;
        Player_Shoot = null;
        Player_Damage = null;
        Enemy_Damage = null;
        Enemy_Death = null;
    }

    public void Init()
    {
        Instance = this;

        Player_Shoot += PlayerShoot;
        Player_Damage += PlayerDamage;
        Enemy_Damage += EnemyDamage;
        Enemy_Death += EnemyDeath;
    }

    private void OnDestroy()
    {
        Player_Shoot -= PlayerShoot;
        Player_Damage -= PlayerDamage;
        Enemy_Damage -= EnemyDamage;
        Enemy_Death -= EnemyDeath;
    }

    public float PowerUp_UiFill
    {
        get
        {
            return icon.fillAmount;
        }
        set
        {
            icon.fillAmount = value;
        }
    }

    public void SelectPowerUp(PowerUpBase powerUp)
    {
        gameplayScriptable.selectedPowerUp = powerUp;

        if (powerUp == null)
        {
            icon.gameObject.SetActive(false);
        }
        else
        {
            icon.gameObject.SetActive(true);

            icon.sprite = powerUp.sprite;

            if (powerUp.useDuration)
            {
                maxValue = powerUp.InitDuration();
                icon.fillAmount = powerUp.duration.Remap(0, maxValue, 0, 1);
            }
            else
            {
                maxValue = 1;
                icon.fillAmount = 1;
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
        PlayerProgress.SavePowerUps(true);
        gameplayScriptable.selectedPowerUp = null;

        type.shipBase = PlayerController.Instance;

        if (type.useDuration)
        {
            type.CountDuration().Forget();
        }

        type.OnActivate();

        if (type.clip_activate.audioClip != null)
        {
            AudioManager.Instance.PlaySound(type.clip_activate.audioClip, type.clip_activate.audioVolume, Enums.SourceType.PowerUps);
        }

        if (type.clip_constant.audioClip != null && type.waitForActivate)
        {
            AudioManager.Instance.PlaySoundLoopDelayed(type.clip_constant.audioClip, type.clip_activate.audioClip.length, type.clip_constant.audioVolume, Enums.SourceType.PowerUpsLoop).Forget();
        }
        else if (type.clip_constant.audioClip != null)
        {
            AudioManager.Instance.PlaySoundLoop(type.clip_constant.audioClip, type.clip_constant.audioVolume, Enums.SourceType.PowerUpsLoop);
        }

        if (type.useDuration)
        {
            maxValue = type.duration;
        }
        else
        {
            icon.fillAmount = 1;
        }
    }

    public void RemovePowerUp(PowerUpBase type)
    {
        if (type.clip_deactivate.audioClip != null)
        {
            AudioManager.Instance.PlaySound(type.clip_deactivate.audioClip, type.clip_deactivate.audioVolume, Enums.SourceType.PowerUps);
        }
        if (type.clip_constant.audioClip != null)
        {
            AudioManager.Instance.StopSource(Enums.SourceType.PowerUpsLoop);
        }

        type.OnDeactivate();

        //currentPowerUps.Remove(type);
        currentPowerUp = null;

        type.Dispose();

        icon.fillAmount = 0;
        icon.enabled = false;
    }

    public void PlayerShoot()
    {
        if (currentPowerUp == null)
        {
            return;
        }

        if (currentPowerUp.clip_shoot.audioClip != null)
        {
            AudioManager.Instance.PlaySound(currentPowerUp.clip_shoot.audioClip, currentPowerUp.clip_shoot.audioVolume, Enums.SourceType.PowerUps);
        }

        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnPlayerShoot();
        //}

        currentPowerUp.OnPlayerShoot();
    }

    public void PlayerDamage()
    {
        if (currentPowerUp == null)
        {
            return;
        }

        if (currentPowerUp.clip_playerDamage.audioClip != null)
        {
            AudioManager.Instance.PlaySound(currentPowerUp.clip_playerDamage.audioClip, currentPowerUp.clip_playerDamage.audioVolume, Enums.SourceType.PowerUps);
        }

        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnPlayerDamage();
        //}

        currentPowerUp.OnPlayerDamage();
    }

    public void EnemyDamage(ShipBaseController enemy)
    {
        if (currentPowerUp == null)
        {
            return;
        }

        if (currentPowerUp.clip_enemyDamage.audioClip != null)
        {
            AudioManager.Instance.PlaySound(currentPowerUp.clip_enemyDamage.audioClip, currentPowerUp.clip_enemyDamage.audioVolume, Enums.SourceType.PowerUps);
        }

        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnEnemyDamage(enemy);
        //}

        currentPowerUp.OnEnemyDamage(enemy);
    }

    public void EnemyDeath(ShipBaseController enemy)
    {
        if (currentPowerUp == null)
        {
            return;
        }

        if (currentPowerUp.clip_enemyDeath.audioClip != null)
        {
            AudioManager.Instance.PlaySound(currentPowerUp.clip_enemyDeath.audioClip, currentPowerUp.clip_enemyDeath.audioVolume, Enums.SourceType.PowerUps);
        }

        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnEnemyDeath(enemy);
        //}

        currentPowerUp.OnEnemyDeath(enemy);
    }

    private void Update()
    {
        if (currentPowerUp == null)
        {
            return;
        }

        //for (int i = 0; i < currentPowerUps.Count; i++)
        //{
        //    currentPowerUps[i].OnGameUpdate();
        //}

        currentPowerUp.OnGameUpdate();

        if ((currentPowerUp != null && currentPowerUp.useDuration))
        {
            icon.fillAmount = currentPowerUp.duration.Remap(0, maxValue, 0, 1);
        }
    }
}