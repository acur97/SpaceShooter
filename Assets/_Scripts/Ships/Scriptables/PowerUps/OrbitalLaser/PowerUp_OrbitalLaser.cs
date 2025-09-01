using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_OrbitalLaser", menuName = "Gameplay/PowerUps/OrbitalLaser", order = 14)]
public class PowerUp_OrbitalLaser : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;

    public PowerUp_OrbitalLaser() : base()
    {
        type = PowerUpsManager.PowerUpType.OrbitalLaser;
    }

    public override void OnActivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = true;

        Object.Instantiate(prefab, PlayerController.Instance.transform);
        PowerUpsManager.Instance.RemovePowerUp(this);
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

    }
}