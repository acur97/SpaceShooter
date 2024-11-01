public class PowerUp_TripleShot : PowerUpBase
{
    public PowerUp_TripleShot() : base()
    {
        type = PowerUpsManager.PowerUpType.TripleShot;
        duration = 20f;
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
        BulletsPool.Instance.InitBullet(shipBase.shootRoot, shipBase._properties.bulletSpeed, false, Bullet.TypeBullet.player);
        BulletsPool.Instance.InitBullet(shipBase.shootRoot, shipBase._properties.bulletSpeed, true, Bullet.TypeBullet.player);
    }
}