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

    [ReadOnly] public PowerUpBase currentPowerUp;
    private bool fromStore = true;
    private PowerUpBase lastPowerUp;

    [Space]
    [SerializeField] private ControlsManager controls;

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

    private void OnDisable()
    {
        ResetPowerUp();
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

    public void SelectPowerUp(PowerUpBase powerUp, bool _fromStore = true)
    {
        if (GameManager.Instance.isPlaying && currentPowerUp != null)
        {
            RemovePowerUp(currentPowerUp);
        }

        fromStore = _fromStore;
        if (fromStore)
        {
            lastPowerUp = powerUp;
        }
        else
        {
            lastPowerUp = gameplayScriptable.selectedPowerUp;
            controls.powerBtn.gameObject.SetActive(true);
        }
        gameplayScriptable.selectedPowerUp = powerUp;

        if (powerUp == null)
        {
            icon.enabled = false;
        }
        else
        {
            icon.enabled = true;

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

        AnalyticsManager.Log_SelectPowerUp(powerUp.type, _fromStore);
    }

    public void ResetPowerUp()
    {
        gameplayScriptable.selectedPowerUp = lastPowerUp;
    }

    public void AddPowerUp(PowerUpBase powerUp)
    {
        if (currentPowerUp != null)
        {
            return;
        }

        currentPowerUp = powerUp;
        if (fromStore)
        {
            currentPowerUp.currentAmount--;
            PlayerProgress.SavePowerUps(true);
        }
        gameplayScriptable.selectedPowerUp = null;

        powerUp.shipBase = PlayerController.Instance;

        if (powerUp.useDuration)
        {
            powerUp.CountDuration().Forget();
        }

        powerUp.OnActivate();

        if (powerUp.clip_activate.audioClip != null)
        {
            AudioManager.Instance.PlaySound(powerUp.clip_activate.audioClip, powerUp.clip_activate.audioVolume, Enums.SourceType.PowerUps);
        }

        if (powerUp.clip_constant.audioClip != null && powerUp.waitForActivate)
        {
            AudioManager.Instance.PlaySoundLoopDelayed(powerUp.clip_constant.audioClip, powerUp.clip_activate.audioClip.length, powerUp.clip_constant.audioVolume, Enums.SourceType.PowerUpsLoop).Forget();
        }
        else if (powerUp.clip_constant.audioClip != null)
        {
            AudioManager.Instance.PlaySoundLoop(powerUp.clip_constant.audioClip, powerUp.clip_constant.audioVolume, Enums.SourceType.PowerUpsLoop);
        }

        if (powerUp.useDuration)
        {
            maxValue = powerUp.duration;
        }
        else
        {
            icon.fillAmount = 1;
        }

        controls.powerBtn.gameObject.SetActive(false);

        AnalyticsManager.Log_UsePowerUp(powerUp.type);
    }

    public void RemovePowerUp(PowerUpBase powerUp)
    {
        if (powerUp.clip_deactivate.audioClip != null)
        {
            AudioManager.Instance.PlaySound(powerUp.clip_deactivate.audioClip, powerUp.clip_deactivate.audioVolume, Enums.SourceType.PowerUps);
        }
        if (powerUp.clip_constant.audioClip != null)
        {
            AudioManager.Instance.StopSource(Enums.SourceType.PowerUpsLoop);
        }

        powerUp.OnDeactivate();

        currentPowerUp = null;

        powerUp.Dispose();

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

        currentPowerUp.OnEnemyDeath(enemy);
    }

    private void Update()
    {
        if (currentPowerUp == null)
        {
            return;
        }

        currentPowerUp.OnGameUpdate();

        if (currentPowerUp != null && currentPowerUp.useDuration)
        {
            icon.fillAmount = currentPowerUp.duration.Remap(0, maxValue, 0, 1);
        }
    }

    public void InstantiatePowerUp(PowerUpBase powerUp)
    {
        PowerUpsPool.Instance.InitPowerUp(powerUp);
    }
}