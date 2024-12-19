using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_LifeRegen", menuName = "Gameplay/PowerUps/LifeRegen", order = 10)]
public class PowerUp_LifeRegen : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField, Range(0f, 1f)] private float regenPercentage = 0.3f;

    private int lifeToAdd;

    public PowerUp_LifeRegen() : base()
    {
        type = PowerUpsManager.PowerUpType.LifeRegen;
    }

    public override void OnActivate()
    {
        switch (RoundsController.Instance.levelType)
        {
            case RoundsController.LevelType.Normal:
                lifeToAdd = (int)GameManager.Instance.gameplayScriptable.playerHealth;
                break;

            case RoundsController.LevelType.Infinite:
                lifeToAdd = (int)GameManager.Instance.gameplayScriptable.playerHealthInfinite;
                break;
        }

        shipBase.health += (int)(lifeToAdd * regenPercentage);
        shipBase.health = Mathf.Min(shipBase.health, lifeToAdd);

        PlayerController.Instance.UpdateHealthUi();
        Object.Instantiate(prefab, PlayerController.Instance.transform);
        PowerUpsManager.Instance.RemovePowerUp(this);
    }

    public override void OnDeactivate()
    {

    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {

    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {

    }

    public override void OnGameUpdate()
    {

    }

    public override void OnPlayerDamage()
    {

    }

    public override void OnPlayerShoot()
    {

    }
}