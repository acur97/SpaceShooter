using Cysharp.Threading.Tasks;
using UnityEngine;

public class PowerUp_DroneOrbital : PowerUpBase
{
    GameObject dronObject;
    PlayerController playerDron;

    private float angle;
    private Vector2 newPosition = new Vector3();

    public PowerUp_DroneOrbital() : base()
    {
        type = PowerUpsManager.PowerUpType.DroneOrbital;
        duration = 30f;
    }

    public override void OnActivate()
    {
        LoadPrefab().Forget();
    }

    private async UniTaskVoid LoadPrefab()
    {
        dronObject = Object.Instantiate(await Resources.LoadAsync("Ships/Player Dron 2") as GameObject);
        playerDron = dronObject.GetComponent<PlayerController>();

        playerDron.Init(true);
        playerDron.shoot.Init(playerDron, playerDron.controls);
        //playerDron.movement.Init(playerDron, playerDron.controls);
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
        if (dronObject == null)
        {
            return;
        }

        angle += playerDron._properties.customFloat2 * Time.deltaTime;
        newPosition.x = shipBase.transform.position.x + playerDron._properties.customFloat1 * Mathf.Cos(angle);
        newPosition.y = shipBase.transform.position.y + playerDron._properties.customFloat1 * Mathf.Sin(angle);
        dronObject.transform.position = newPosition;

        playerDron.movement.ClampPosition();
    }

    public override void OnPlayerDamage()
    {

    }

    public override void OnPlayerShoot()
    {

    }
}