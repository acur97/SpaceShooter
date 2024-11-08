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
        Object.Instantiate(prefab).transform.position = PlayerController.Instance.transform.position;
        PowerUpsManager.Instance.RemovePowerUp(this);
    }

    public override void OnDeactivate()
    {

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