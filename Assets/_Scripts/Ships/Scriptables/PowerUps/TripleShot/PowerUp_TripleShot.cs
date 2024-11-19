using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_TripleShot", menuName = "Gameplay/PowerUps/TripleShot", order = 2)]
public class PowerUp_TripleShot : PowerUpBase
{
    public PowerUp_TripleShot() : base()
    {
        type = PowerUpsManager.PowerUpType.TripleShot;
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
        BulletsPool.Instance.InitBullet(shipBase.shootRoot2, shipBase._properties, Bullet.TypeBullet.player);
        BulletsPool.Instance.InitBullet(shipBase.shootRoot1, shipBase.shootRoot3, shipBase._properties, Bullet.TypeBullet.player);
    }
}