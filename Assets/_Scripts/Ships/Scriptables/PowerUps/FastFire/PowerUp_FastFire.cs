using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_FastFire", menuName = "Gameplay/PowerUps/FastFire", order = 8)]
public class PowerUp_FastFire : PowerUpBase
{
    [Space]
    [SerializeField] private float multiplier = 2f;

    private float prevBulletSpeed;

    public PowerUp_FastFire() : base()
    {
        type = PowerUpsManager.PowerUpType.FastFire;
        useDuration = true;
        durationRange = new Vector2(10f, 10f);
    }

    public override void OnActivate()
    {
        prevBulletSpeed = shipBase._properties.bulletSpeed;
        shipBase._properties.bulletSpeed *= multiplier;
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

    public override void Dispose()
    {
        shipBase._properties.bulletSpeed = prevBulletSpeed;
        base.Dispose();
    }
}