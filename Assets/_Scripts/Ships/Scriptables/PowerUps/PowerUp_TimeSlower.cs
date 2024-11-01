using UnityEngine;

public class PowerUp_TimeSlower : PowerUpBase
{
    public PowerUp_TimeSlower() : base()
    {
        type = PowerUpsManager.PowerUpType.TimeSlower;
        duration = 10f;
    }

    public override void OnActivate()
    {
        Time.timeScale = 0.5f;
    }

    public override void OnDeactivate()
    {
        Time.timeScale = 1f;
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