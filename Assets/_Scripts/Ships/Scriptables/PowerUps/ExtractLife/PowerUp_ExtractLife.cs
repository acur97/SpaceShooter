using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_ExtractLife", menuName = "Gameplay/PowerUps/ExtractLife", order = 15)]
public class PowerUp_ExtractLife : PowerUpBase
{
    [Space]
    [SerializeField] private uint steals = 1;

    private uint stolen = 0;

    public PowerUp_ExtractLife() : base()
    {
        type = PowerUpsManager.PowerUpType.ExtractLife;
    }

    public override void OnActivate()
    {

    }

    public override void OnDeactivate()
    {

    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {

    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {
        if (shipBase.health < shipBase._properties.health)
        {
            shipBase.health += enemy._properties.health;
            shipBase.health = Mathf.Clamp(shipBase.health, 0, shipBase._properties.health);

            PlayerController.Instance.UpdateHealthUi();
        }

        stolen++;
        if (stolen >= steals)
        {
            PowerUpsManager.Instance.RemovePowerUp(this);
        }
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