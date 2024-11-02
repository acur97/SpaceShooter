using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_LifeSteal", menuName = "Gameplay/PowerUps/LifeSteal", order = 9)]
public class PowerUp_LifeSteal : PowerUpBase
{
    public PowerUp_LifeSteal() : base()
    {
        type = PowerUpsManager.PowerUpType.LifeSteal;
        useDuration = true;
        durationRange = new Vector2(10f, 10f);
    }

    public override void OnActivate()
    {

    }

    public override void OnDeactivate()
    {

    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {
        if (shipBase.health < shipBase._properties.health)
        {
            shipBase.health++;

            PlayerController.Instance.UpdateHealthUi();
        }
    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {
        // optional
        // regenerate the player's health with the total enemy health

        //if (shipBase.health < shipBase._properties.health)
        //{
        //    shipBase.health += enemy._properties.health;
        //    shipBase.health = Mathf.Clamp(shipBase.health, 0, shipBase._properties.health);

        //    PlayerController.Instance.UpdateHealthUi();
        //}
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