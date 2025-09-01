using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_LaserBurst", menuName = "Gameplay/PowerUps/LaserBurst", order = 13)]
public class PowerUp_LaserBurst : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float timeToStart = 3f;
    [SerializeField] private uint damage = 25;

    private PowerUp_LaserBurstBehaviour laserInstance;

    public PowerUp_LaserBurst() : base()
    {
        type = PowerUpsManager.PowerUpType.LaserBurst;
        useDuration = true;
        durationRange = new Vector2(6f, 9f);
    }

    public override void OnActivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = true;

        laserInstance = Object.Instantiate(prefab).GetComponent<PowerUp_LaserBurstBehaviour>();
        laserInstance.Init(timeToStart, damage, shipBase._properties.color, duration).Forget();
    }

    public override void OnDeactivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = false;
        laserInstance.Deactivate().Forget();
    }

    public override void OnEnemyDamage(ShipBaseController enemy)
    {

    }

    public override void OnEnemyDeath(ShipBaseController enemy)
    {

    }

    public override void OnGameUpdate()
    {
        laserInstance.transform.position = PlayerController.Instance.transform.position;
    }

    public override void OnPlayerDamage()
    {

    }

    public override void OnPlayerShoot()
    {

    }
}