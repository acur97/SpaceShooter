using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_TimeSlower", menuName = "Gameplay/PowerUps/TimeSlower", order = 6)]
public class PowerUp_TimeSlower : PowerUpBase
{
    [Space]
    [SerializeField, Range(0f, 1f)] private float timePercent = 0.5f;

    public PowerUp_TimeSlower() : base()
    {
        type = PowerUpsManager.PowerUpType.TimeSlower;
        useDuration = true;
        durationRange = new Vector2(10f, 10f);
    }

    public override void OnActivate()
    {
        Time.timeScale = timePercent;
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