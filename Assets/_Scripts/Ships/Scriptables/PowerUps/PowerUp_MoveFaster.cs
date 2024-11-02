using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_MoveFaster", menuName = "Gameplay/PowerUps/MoveFaster", order = 7)]
public class PowerUp_MoveFaster : PowerUpBase
{
    public PowerUp_MoveFaster() : base()
    {
        type = PowerUpsManager.PowerUpType.MoveFaster;
        useDuration = true;
        durationRange = new Vector2(8f, 10f);
    }

    public override void OnActivate()
    {
        PlayerController.Instance.movement.movePowerUp = 2f;
    }

    public override void OnDeactivate()
    {
        PlayerController.Instance.movement.movePowerUp = 1f;
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