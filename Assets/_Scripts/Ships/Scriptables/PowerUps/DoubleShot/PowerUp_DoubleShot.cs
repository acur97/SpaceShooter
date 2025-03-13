using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_DoubleShot", menuName = "Gameplay/PowerUps/DoubleShot", order = 1)]
public class PowerUp_DoubleShot : PowerUpBase
{
    public PowerUp_DoubleShot() : base()
    {
        type = PowerUpsManager.PowerUpType.DoubleShot;
        useDuration = true;
        durationRange = new Vector2(20f, 20f);
    }

    public override void OnActivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = true;
    }

    public override void OnDeactivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = false;
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
        BulletsPool.Instance.InitBullet(shipBase._properties, Enums.TypeBullet.player,
            (shipBase.shootRoot1.position, shipBase.shootRoot1.rotation),
            (shipBase.shootRoot3.position, shipBase.shootRoot3.rotation));
    }
}