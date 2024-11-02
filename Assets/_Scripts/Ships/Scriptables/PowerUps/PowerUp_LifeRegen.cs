using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_LifeRegen", menuName = "Gameplay/PowerUps/LifeRegen", order = 10)]
public class PowerUp_LifeRegen : PowerUpBase
{
    [Space]
    [SerializeField] private float regenPercentage = 0.3f;

    public PowerUp_LifeRegen() : base()
    {
        type = PowerUpsManager.PowerUpType.LifeRegen;
    }

    public override void OnActivate()
    {
        if (shipBase.health < shipBase._properties.health)
        {
            shipBase.health += (int)(shipBase._properties.health * regenPercentage);
            shipBase.health = Mathf.Clamp(shipBase.health, 0, shipBase._properties.health);

            PlayerController.Instance.UpdateHealthUi();

            PowerUpsManager.Instance.RemovePowerUp(this);
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