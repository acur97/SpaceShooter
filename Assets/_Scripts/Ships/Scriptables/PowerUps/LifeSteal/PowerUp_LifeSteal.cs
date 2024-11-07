using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_LifeSteal", menuName = "Gameplay/PowerUps/LifeSteal", order = 9)]
public class PowerUp_LifeSteal : PowerUpBase
{
    [Space]
    [SerializeField, Range(0f, 1f)] private float maxLifeSteal = 0.1f;

    private int maxPercent;
    private int percent = 0;

    public PowerUp_LifeSteal() : base()
    {
        type = PowerUpsManager.PowerUpType.LifeSteal;
        useDuration = true;
        durationRange = new Vector2(10f, 10f);
    }

    public override void OnActivate()
    {
        maxPercent = (int)(shipBase._properties.health * maxLifeSteal);
    }

    public override void OnDeactivate()
    {

    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {
        if (shipBase.health < shipBase._properties.health)
        {
            shipBase.health++;
            percent++;

            PlayerController.Instance.UpdateHealthUi();

            if (percent >= maxPercent)
            {
                PowerUpsManager.Instance.RemovePowerUp(this);
            }
        }
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