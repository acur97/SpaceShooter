using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_DroneSupport", menuName = "Gameplay/PowerUps/DroneSupport", order = 11)]
public class PowerUp_DroneSupport : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float randomOffset = 0.6f;

    private GameObject dronObject;
    private PlayerController playerDron;

    public PowerUp_DroneSupport() : base()
    {
        type = PowerUpsManager.PowerUpType.DroneSupport;
        useDuration = true;
        durationRange = new Vector2(40f, 40f);
    }

    public override void OnActivate()
    {
        dronObject = Object.Instantiate(prefab);
        playerDron = dronObject.GetComponent<PlayerController>();

        playerDron.Init(true);
        playerDron.shoot.Init(playerDron, playerDron.controls);
        playerDron.movement.Init(playerDron, playerDron.controls, randomOffset);
    }

    public override void OnDeactivate()
    {
        playerDron = null;
        Object.Destroy(dronObject);
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