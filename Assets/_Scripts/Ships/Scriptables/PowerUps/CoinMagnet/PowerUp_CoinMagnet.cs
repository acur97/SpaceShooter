using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_CoinMagnet", menuName = "Gameplay/PowerUps/CoinMagnet", order = 16)]
public class PowerUp_CoinMagnet : PowerUpBase
{
    [Space]
    [SerializeField] private float range = 3f;
    [SerializeField] private float attractionSpeed = 1f;
    [SerializeField] private GameObject prefab;

    public PowerUp_CoinMagnet() : base()
    {
        type = PowerUpsManager.PowerUpType.CoinMagnet;
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