using UnityEngine;

public class PowerUp_LifeRegen : PowerUpBase
{
    public PowerUp_LifeRegen() : base()
    {
        type = PowerUpsManager.PowerUpType.LifeRegen;
    }

    public override void OnActivate()
    {
        if (shipBase.health < shipBase._properties.health)
        {
            shipBase.health += (int)(shipBase._properties.health * 0.3f);
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