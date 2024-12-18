using UnityEngine;

[CreateAssetMenu(fileName = "PowerUp_DroneOrbital", menuName = "Gameplay/PowerUps/DroneOrbital", order = 12)]
public class PowerUp_DroneOrbital : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float distance = 1f;
    [SerializeField] private float speed = 2f;

    private GameObject dronObject;
    private PlayerController playerDron;

    private float angle;
    private Vector2 newPosition = new Vector3();

    public PowerUp_DroneOrbital() : base()
    {
        type = PowerUpsManager.PowerUpType.DroneOrbital;
        useDuration = true;
        durationRange = new Vector2(30f, 30f);
    }

    public override void OnActivate()
    {
        dronObject = Object.Instantiate(prefab);
        playerDron = dronObject.GetComponent<PlayerController>();

        playerDron.Init(playerDron._properties, true);
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

        angle += speed * Time.deltaTime;
        newPosition.x = shipBase.transform.position.x + distance * Mathf.Cos(angle);
        newPosition.y = shipBase.transform.position.y + distance * Mathf.Sin(angle);
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