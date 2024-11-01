public class PowerUp_FastFire : PowerUpBase
{
    private float prevBulletSpeed;

    public PowerUp_FastFire() : base()
    {
        type = PowerUpsManager.PowerUpType.FastFire;
        duration = 10f;
    }

    public override void OnActivate()
    {
        prevBulletSpeed = shipBase._properties.bulletSpeed;
        shipBase._properties.bulletSpeed *= 2f;
    }

    public override void OnDeactivate()
    {
        shipBase._properties.bulletSpeed = prevBulletSpeed;
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