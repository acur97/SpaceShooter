using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_ExtractLife", menuName = "Gameplay/PowerUps/ExtractLife", order = 15)]
public class PowerUp_ExtractLife : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private uint steals = 1;

    private uint stolen = 0;
    private int lifeToAdd;

    public PowerUp_ExtractLife() : base()
    {
        type = PowerUpsManager.PowerUpType.ExtractLife;
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
    }

    public override void OnDeactivate()
    {

    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {

    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {
        shipBase.health += enemy._properties.health;
        shipBase.health = Mathf.Min(shipBase.health, lifeToAdd);

        PlayerController.Instance.UpdateHealthUi();
        Object.Instantiate(prefab, PlayerController.Instance.transform);

        stolen++;
        if (stolen >= steals)
        {
            PowerUpsManager.Instance.RemovePowerUp(this);
        }
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