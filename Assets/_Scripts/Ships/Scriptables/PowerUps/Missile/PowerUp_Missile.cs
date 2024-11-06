using UnityEngine;
using Object = UnityEngine.Object;

[CreateAssetMenu(fileName = "PowerUp_Missile", menuName = "Gameplay/PowerUps/Missile", order = 3)]
public class PowerUp_Missile : PowerUpBase
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private uint count = 2;
    private int currentCount = 0;

    [Space]
    [SerializeField] private int damage = 10;
    [SerializeField] private float initialSpeed = 0.1f;
    [SerializeField] private float speedUp = 0.05f;

    private PowerUp_MissileBehaviour missileInstance;

    public PowerUp_Missile() : base()
    {
        type = PowerUpsManager.PowerUpType.Missile;
    }

    public override void OnActivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = true;
        currentCount = (int)count;
    }

    public override void OnDeactivate()
    {
        PlayerController.Instance.shoot.shootPowerUp = false;
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
        missileInstance = Object.Instantiate(prefab).GetComponent<PowerUp_MissileBehaviour>();
        PoolsUpdateManager.PoolUpdate += missileInstance.OnUpdate;
        missileInstance.transform.position = shipBase.shootRoot.position;

        missileInstance.Init(damage, initialSpeed, speedUp);

        currentCount--;
        if (currentCount <= 0)
        {
            PowerUpsManager.Instance.RemovePowerUp(this);
        }
    }
}