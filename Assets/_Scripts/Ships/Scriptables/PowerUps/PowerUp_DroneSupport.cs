using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_DroneSupport : PowerUpBase
{
    GameObject dronObject;
    PlayerController playerDron;

    public PowerUp_DroneSupport() : base()
    {
        type = PowerUpsManager.PowerUpType.DroneSupport;
        duration = 40f;
    }

    public override void OnActivate()
    {
        LoadPrefab().Forget();
    }

    private async UniTaskVoid LoadPrefab()
    {
        dronObject = Object.Instantiate(await Resources.LoadAsync("Ships/Player Dron 1") as GameObject);
        playerDron = dronObject.GetComponent<PlayerController>();

        playerDron.Init(true);
        playerDron.shoot.Init(playerDron, playerDron.controls);
        playerDron.movement.Init(playerDron, playerDron.controls);
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