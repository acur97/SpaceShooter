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
        BulletsPool.Instance.InitBullet(shipBase.shootRoot1, shipBase.shootRoot3, shipBase._properties.bulletSpeed, Bullet.TypeBullet.player);
    }
}